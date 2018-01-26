using System;
using System.Collections.Generic;
using System.Linq;
using GeneralsBot.TargetHeuristics;

namespace GeneralsBot {
    public class Game {
        public  readonly string        ChatRoom;
        private          int           _turn = 0;
        private readonly IList<string> _usernames;
        private readonly IList<int>    _teams;
        private readonly string        _replayId;
        private readonly string        _teamChatRoom;
        private readonly string        _gameType;
        private readonly int           _me;
        private          IList<int>    _cities;
        private          IList<int>    _rawMap;
        private          GameMap       _map;
        private readonly IList<ITargetHeuristic> _targetHeuristics;
        private readonly IList<int> _lastSeenArmies = new List<int>();
        private readonly HashSet<int> _generals = new HashSet<int>();
        private readonly HashSet<int> _allCities = new HashSet<int>();
        private readonly HashSet<int> _seen = new HashSet<int>();

        public Game(int           playerIndex,
                    IList<string> usernames,
                    IList<int>    teams,
                    string        replayId,
                    string        chatRoom,
                    string        teamChatRoom,
                    string        gameType,
                    IList<ITargetHeuristic> targetHeuristics) {
            _me           = playerIndex;
            _usernames    = usernames;
            _teams        = teams;
            _replayId     = replayId;
            ChatRoom     = chatRoom;
            _teamChatRoom = teamChatRoom;
            _gameType     = gameType;
            _targetHeuristics = targetHeuristics;
        }

        public static Game FromMessage(GameStartMessage gameStartMessage) {
            return new Game(gameStartMessage.PlayerIndex, gameStartMessage.Usernames, gameStartMessage.Teams,
                            gameStartMessage.ReplayId, gameStartMessage.ChatRoom, gameStartMessage.TeamChatRoom,
                            gameStartMessage.GameType,
                            new List<ITargetHeuristic> {
                                new ExpandHeuristic(), new CityHeuristic(), new AttackHeuristic(),
                                new CapturePlayerHeuristic()
                             //  new DefendKingHeuristic()
                            });
        }

        public void ApplyUpdate(GameUpdateMessage message) {
            _cities = Patch(_cities, message.CitiesDiff);
            _rawMap = Patch(_rawMap, message.MapDiff);

            foreach (var g in message.Generals) _generals.Add(g);
            foreach (var g in _generals) if (!message.Generals.Contains(g)) message.Generals.Add(g);
            
            foreach (int city in _cities) {
                _allCities.Add(city);
            }
            
            _map = GameMap.FromRawLists(_rawMap, _allCities, message.Generals, _seen, _lastSeenArmies);
            _map.PrettyPrint();
        }

        public (int start, int end, bool is50) GetAttack() {
            List<(int priority, Position dest)> desiredTargets = new List<(int, Position)>();
            
            Console.WriteLine(desiredTargets.Count);
            
            foreach (var heuristic in _targetHeuristics) {
                desiredTargets.AddRange(heuristic.Calculate(_map, _me));
            }
            
            Console.WriteLine($"{desiredTargets.Count} potential targets");

            var grouped = from desired in desiredTargets
                          group (desired.priority, desired.dest) by desired.dest;

            var descendingTargets = desiredTargets.GroupBy(x => x.dest)
                                                  .Select(g => (g.Sum(x => x.priority), g.Key))
                                                  .OrderByDescending<(int priority, Position dest), int>
                                                      (k => k.priority).ToList();

            Position src, toward;
            
            while (true) {
                (int priority, Position dest) = descendingTargets.First();
                descendingTargets.Remove((priority, dest));

                Console.WriteLine($"AI desires a move with priority {priority} to <{dest.X}, {dest.Y}>");

                (src, toward) = CalculateBestMoveTowards(dest);

                if (src != null || toward != null) break;
                if (descendingTargets.Count == 0) throw new Exception("Ran out of possible moves");
            }
            

            Console.WriteLine($"Issued move from {src.X} {src.Y} to {toward.X} {toward.Y}");
            Console.WriteLine($"Playing with {Usernames}");
            return (_map.UCoord(src.X, src.Y), _map.UCoord(toward.X, toward.Y), false);
        }

        private (Position, Position) CalculateBestMoveTowards(Position dest) {
            
            // Quick override for if you can capture it immediately..
            if (_map[dest] is OccupiedTile enemyOcc && enemyOcc.Faction != _me) {
                foreach (Position p in dest.SurroundingMoveable(_map)) {
                    if (_map[p] is OccupiedTile occ && occ.Faction == _me && occ.Units > enemyOcc.Units + 1) {
                        return (p, dest);
                    }
                }
            } else if (_map[dest] is EmptyTile emp) {
                foreach (Position p in dest.SurroundingMoveable(_map)) {
                    if (_map[p] is OccupiedTile occ && occ.Faction == _me && occ.Units > 1) {
                        return (p, dest);
                    }
                }
            }

            IList<Position> queuedPositions = new List<Position>();
            HashSet<Position> testedPositions = new HashSet<Position>();
            Dictionary<Position, Position> referers = new Dictionary<Position, Position>();
            Dictionary<Position, int> destValue = new Dictionary<Position, int>();
            Dictionary<Position, int> points = new Dictionary<Position, int>();

            Position current     = dest;
            referers[current]    = current;
            GameTile currentTile = _map[dest];
            queuedPositions.Add(current);
            points[current]    = 0;
            destValue[current] = MyUnitEquivalenceOn(currentTile);

            while (queuedPositions.Count > 0) {
                testedPositions.Add(current);
                
                foreach (Position neighbor in current.SurroundingMoveable(_map)) {
                    if (_map[neighbor] is MountainTile) continue;

                    if (!testedPositions.Contains(neighbor) && !queuedPositions.Contains(neighbor)) {
                        queuedPositions.Add(neighbor);
                    } else {
                        continue;
                    }

                    var unitEquivalenceNeighbor = MyUnitEquivalenceOn(_map[neighbor]) - 1;
                    var neighborVal             = destValue[current]                  + unitEquivalenceNeighbor;
                    var pointsVal               =
                        points[current] + 10 * unitEquivalenceNeighbor - _turn / 10 - 5;
                                        
                    if (_map[dest] is OccupiedTile occup && occup.Faction != _me
                                                         && destValue[current] > occup.Units + 1
                        || _map[dest] is FogTile && destValue[current] > _map.EstUnitsAt(dest) * 1.2f) {
                        pointsVal -= 60 + (_turn / 10) * Distance(referers, neighbor);
                    }

                    if (_map[dest] is EmptyTile && _turn < 100 && destValue[current] > 1) {
                        pointsVal -= 50;
                    }

                    if (_map[dest] is CityTile cTile && cTile.Faction == 0
                                                     && dest.Surrounding(_map)
                                                            .All(c => !(_map[c] is OccupiedTile occT && occT.Faction > 0
                                                                                                     && occT.Faction
                                                                                                     != _me))) {
                        if (destValue[current] > cTile.Units + 2) {
                            pointsVal -= 5000;
                        }
                    }

                    if (!points.ContainsKey(neighbor) || points[neighbor] < pointsVal) {
                        destValue[neighbor] = neighborVal;
                        points[neighbor]    = pointsVal;
                        referers[neighbor]  = current;
                    }
                }

                queuedPositions.Remove(current);

                int maxPoints = Int32.MinValue;
                current       = null;

                foreach (Position queuedPosition in queuedPositions) {
                    if (points.ContainsKey(queuedPosition) && maxPoints < points[queuedPosition]) {
                        maxPoints = points[queuedPosition];
                        current   = queuedPosition;
                    }
                }

                if (current == null) break;
            }
            
            Console.WriteLine("Calculated path data towards position");

            points[dest] = Int32.MinValue;

            Position src = BestMove(points);

            while (PointlessMove(src, referers[src]) && points.Count > 0) {
                points.Remove(src);
                src = BestMove(points);
                if (src == null) break;
            }

            if (points.Count == 0) return (null, null);
            
            return (src, referers[src]);
        }

        private int Distance(Dictionary<Position, Position> referers, Position neighbor) {
            if (!referers.ContainsKey(neighbor) || referers[neighbor].Equals(neighbor)) return 0;
            
            return 1 + Distance(referers, referers[neighbor]);
        }

        private bool PointlessMove(Position src, Position dest) {
            return src.Equals(dest) || MyUnitEquivalenceOn(_map[src]) <= 1;
        }

        private static Position BestMove(Dictionary<Position, int> points) {
            Position            src = points.OrderByDescending(pair => pair.Value).Select(pair => pair.Key).FirstOrDefault();
            return src;
        }

        private int MyUnitEquivalenceOn(GameTile currentTile) {
            return currentTile is OccupiedTile occTile
                       ? (occTile.Faction == _me ? occTile.Units : -occTile.Units)
                       : 0;
        }

        public string Usernames {
            get { return _usernames.Aggregate((c, s) => c + " " + s); }
        }

        private IList<int> Patch(IList<int> existing, IList<int> delta) {
            List<int> next = new List<int>();

            for (int i = 0; i < delta.Count;) {
                if (delta[i] > 0) {
                    next.AddRange(existing.Skip(next.Count).Take(delta[i]));
                }

                i++;
                
                if (i < delta.Count && delta[i] > 0) {
                    next.AddRange(delta.Skip(i + 1).Take(delta[i]));
                    i += delta[i];
                }
                
                i++;
            }

            return next;
        }
    }
}
