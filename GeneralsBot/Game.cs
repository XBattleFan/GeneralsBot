using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using GeneralsBot.MoveHeuristics;

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
        private readonly IList<IMoveHeuristic> _moveHeuristics;

        public Game(int           playerIndex,
                    IList<string> usernames,
                    IList<int>    teams,
                    string        replayId,
                    string        chatRoom,
                    string        teamChatRoom,
                    string        gameType,
                    IList<IMoveHeuristic> moveHeuristics) {
            _me           = playerIndex;
            _usernames    = usernames;
            _teams        = teams;
            _replayId     = replayId;
            _chatRoom     = chatRoom;
            _teamChatRoom = teamChatRoom;
            _gameType     = gameType;
            _moveHeuristics = moveHeuristics;
        }

        public static Game FromMessage(GameStartMessage gameStartMessage) {
            return new Game(gameStartMessage.PlayerIndex, gameStartMessage.Usernames, gameStartMessage.Teams,
                            gameStartMessage.ReplayId, gameStartMessage.ChatRoom, gameStartMessage.TeamChatRoom,
                            gameStartMessage.GameType,
                            new List<IMoveHeuristic>() { new ExpandHeuristic() });
        }

        public void ApplyUpdate(GameUpdateMessage message) {
            _cities = Patch(_cities, message.CitiesDiff);
            _rawMap = Patch(_rawMap, message.MapDiff);

            _map = GameMap.FromRawLists(_rawMap, _cities, message.Generals);
            _map.PrettyPrint();
        }

        public (int start, int end, bool is50) GetAttack() {
            List<(int priority, Attack move)> desiredMoves = new List<(int, Attack)>();
            
            Console.WriteLine(desiredMoves.Count);
            
            foreach (var heuristic in _moveHeuristics) {
                desiredMoves.AddRange(heuristic.Calculate(_map, _me));
            }
            
            Console.WriteLine(desiredMoves.Count);

            var grouped = from desired in desiredMoves
                          group (desired.priority, desired.move) by desired.move;

            (int priority, Attack move) = desiredMoves.GroupBy(x => x.move).Select(g => (g.Sum(x => x.priority), g.Key))
                                                      .OrderByDescending<(int priority, Attack move), int>
                                                          (k => k.priority).First();
            
            Console.WriteLine($"Issuing move with priority {priority} <{move.From.X}, {move.From.Y}> to <{move.To.X}, {move.To.Y}>");
            
            return (_map.UCoord(move.From),
                    _map.UCoord(move.To),
                    move.Is50);
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
