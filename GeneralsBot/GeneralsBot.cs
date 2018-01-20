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
                Environment.Exit(0);
            });

            _socket.On(Socket.EVENT_CONNECT, () => {
                Console.WriteLine("Connected");

                _socket.Emit("join_private",    "mtpe", _userId);
                _socket.Emit("set_force_start", "mtpe", _userId);
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
