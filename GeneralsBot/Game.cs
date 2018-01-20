using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using GeneralsBot.TargetHeuristics;

namespace GeneralsBot {
    public class Game {
        private          int           _turn = 0;
        private readonly IList<string> _usernames;
        private readonly IList<int>    _teams;
        private readonly string        _replayId;
        private readonly string        _chatRoom;
        private readonly string        _teamChatRoom;
        private readonly string        _gameType;
        private readonly int           _me;
        private          IList<int>    _cities;
        private          IList<int>    _rawMap;
        private          GameMap       _map;
        private readonly IList<ITargetHeuristic> _targetHeuristics;

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
            _chatRoom     = chatRoom;
            _teamChatRoom = teamChatRoom;
            _gameType     = gameType;
            _targetHeuristics = targetHeuristics;
        }

        public static Game FromMessage(GameStartMessage gameStartMessage) {
            return new Game(gameStartMessage.PlayerIndex, gameStartMessage.Usernames, gameStartMessage.Teams,
                            gameStartMessage.ReplayId, gameStartMessage.ChatRoom, gameStartMessage.TeamChatRoom,
                            gameStartMessage.GameType,
                            new List<ITargetHeuristic>() { new ExpandHeuristic() });
        }

        public void ApplyUpdate(GameUpdateMessage message) {
            _cities = Patch(_cities, message.CitiesDiff);
            _rawMap = Patch(_rawMap, message.MapDiff);

            _map = GameMap.FromRawLists(_rawMap, _cities, message.Generals);
            _map.PrettyPrint();
        }

        public (int start, int end, bool is50) GetAttack() {
            List<(int priority, Position dest)> desiredTargets = new List<(int, Position)>();
            
            Console.WriteLine(desiredTargets.Count);
            
            foreach (var heuristic in _targetHeuristics) {
                desiredTargets.AddRange(heuristic.Calculate(_map, _me));
            }
            
            Console.WriteLine(desiredTargets.Count);

            var grouped = from desired in desiredTargets
                          group (desired.priority, desired.dest) by desired.dest;

            (int priority, Position dest) = desiredTargets.GroupBy(x => x.dest)
                                                          .Select(g => (g.Sum(x => x.priority), g.Key))
                                                          .OrderByDescending<(int priority, Position dest), int>
                                                              (k => k.priority).First();
            
            Console.WriteLine($"AI desires a move with priority {priority} to <{dest.X}, {dest.Y}>");
            
            // Who shall we send? TODO: Move to a heuristic system
            Queue<Position> queuedPositions = new Queue<Position>();
            HashSet<Position> testedPositions = new HashSet<Position> { dest };
            Dictionary<Position, Position> referers = new Dictionary<Position, Position> { { dest, dest } };
            Dictionary<Position, int> destValue = new Dictionary<Position, int> { { dest, 0 } };
            Dictionary<Position, int> points = new Dictionary<Position, int> { { dest, Int32.MinValue } };
            
            foreach (Position position in dest.SurroundingMoveable(_map)) {
                queuedPositions.Enqueue(position);
                testedPositions.Add(position);
                referers[position] = dest;
            }
            
            while (queuedPositions.Count > 0) {
                Position next = queuedPositions.Dequeue();
                
                if (_map[next] is FogTile || _map[next] is MountainTile) continue;

                if (_map[referers[next]] is OccupiedTile referredMapTile && referredMapTile.Faction == _me) {
                    destValue[next] = referredMapTile.Units;
                } else {
                    destValue[next] = 0;
                }
                
                if (_map[next] is OccupiedTile tile && tile.Faction == _me && tile.Units > 1) {
                    destValue[next] += tile.Units - 1;
                } else {
                    points[next] = Int32.MinValue;
                }

                if (!points.ContainsKey(next)) {
                    points[next] = destValue[next] - next.NaiveMoveDistance(dest) * next.NaiveMoveDistance(dest);
                }

                foreach (Position p in next.SurroundingMoveable(_map)) {
                    if (testedPositions.Contains(p)) continue;
                    
                    queuedPositions.Enqueue(p);
                    testedPositions.Add(p);
                    referers[p] = next;
                }
            }

            Position src = points.OrderByDescending(pair => pair.Value).Select(pair => pair.Key).FirstOrDefault();
            Console.WriteLine($"Issued move from {src.X} {src.Y} to {referers[src].X} {referers[src].Y}");
            return (_map.UCoord(src.X, src.Y), _map.UCoord(referers[src].X, referers[src].Y), false);
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
