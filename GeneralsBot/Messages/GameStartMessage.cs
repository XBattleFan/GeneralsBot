using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GeneralsBot {
    public class GameStartMessage {
        public int           PlayerIndex;
        [JsonProperty("replay_id")]
        public string        ReplayId;
        [JsonProperty("chat_room")]
        public string        ChatRoom;
        [JsonProperty("team_chat_room")]
        public string        TeamChatRoom;
        public IList<string> Usernames;
        public IList<int>    Teams;
        [JsonProperty("game_type")]
        public string        GameType;
        public IList<int>    Swamps;
    }
}
