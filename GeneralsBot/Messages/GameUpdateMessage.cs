using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace GeneralsBot {
    public class GameUpdateMessage {
        public IList<GameUpdateMessageScore> Scores;
        public int Turn;
        public int AttackIndex;
        public IList<int> Generals;
        [JsonProperty("map_diff")]
        public IList<int> MapDiff;
        [JsonProperty("cities_diff")]
        public IList<int> CitiesDiff;
    }

    public class GameUpdateMessageScore {
        public int Total;
        public int Tiles;
        public int I;
        public bool Dead;
    }
}
