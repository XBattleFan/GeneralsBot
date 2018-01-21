using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Quobject.EngineIoClientDotNet.ComponentEmitter;
using Quobject.SocketIoClientDotNet.Client;

namespace GeneralsBot {
    public class GeneralsBot {
        private const    string                 Username      = "[Bot] Ashley_Bot";
        private readonly string                 _userId       = File.ReadAllText("user_id");
        private          Game                   _game;
        private          Socket                 _socket;

        public void Start() {
            _socket = Connect();

            _socket.On(Socket.EVENT_DISCONNECT, () => {
                Console.WriteLine("Disconnected from game server");
                _socket.Connect();
            });

            _socket.On(Socket.EVENT_CONNECT, () => {
                Console.WriteLine("Connected");

                _socket.Emit("play", _userId);
                //_socket.Emit("join_private",    "mtpe", _userId);
                _socket.Emit("set_force_start", 0, true);
            });

            _socket.On("game_won", () => {
                Console.WriteLine("Won the game!");
                File.AppendAllLines("log", new []{ $"Won against {_game.Usernames}"});
                _socket.Emit("chat_message", _game.ChatRoom, "Woo! Good game! :) If you'd like to give any feedback about how I played, please do so at https://goo.gl/forms/mCBjHBCDR3Ot96Gp2 ! This bot is actively developed so any feedback is appreciated :) (It doesn't currently monitor game chat)");
                _socket.Emit("play", _userId);
                //_socket.Emit("join_private",    "mtpe", _userId);
                _socket.Emit("set_force_start", 0, true);
            });
            
            _socket.On("game_lost", () => {
                Console.WriteLine("Lost the game!");
                File.AppendAllLines("log", new []{ $"Lost against {_game.Usernames}" });
                _socket.Emit("chat_message", _game.ChatRoom, "Good game! If you'd like to give any feedback about how I played, please do so at https://goo.gl/forms/mCBjHBCDR3Ot96Gp2 ! This bot is actively developed so any feedback is appreciated :) (It doesn't currently monitor game chat)");
                _socket.Emit("play", _userId);
                //_socket.Emit("join_private",    "mtpe", _userId);
                _socket.Emit("set_force_start", 0, true);
            });
            
            _socket.On("game_start",  GameStarted);
            _socket.On("game_update", GameUpdated);

            _socket.On(Socket.EVENT_ERROR, data => { Console.WriteLine($"Error {data}"); });
        }

        private void GameUpdated(object data) {
            try {
                GameUpdateMessage message = JsonConvert.DeserializeObject<GameUpdateMessage>(data.ToString());
                _game.ApplyUpdate(message);
                (int start, int end, bool is50) = _game.GetAttack();
                _socket.Emit("attack", start, end, is50);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        private void GameStarted(object data) {
            GameStartMessage message = JsonConvert.DeserializeObject<GameStartMessage>(data.ToString());
            _game = Game.FromMessage(message);
        }

        private Socket Connect() {
            return IO.Socket("http://botws.generals.io");
        }
    }
}
