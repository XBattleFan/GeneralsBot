using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GeneralsBot {
    internal class GameStartMessage {
        public int PlayerIndex;
        public string ReplayId;
        public string ChatRoom;
        public string TeamChatRoom;
        public IList<string> Usernames;
        public IList<int> Teams;
        public string GameType;
        public IList<int> Swamps;
    }
}
