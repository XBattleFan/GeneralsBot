using System;

namespace GeneralsBot {
    public class ObstacleFogTile : FogTile {
        public override void PrettyPrint() {
            Console.Write("  ?  ");
        }
    }
}
