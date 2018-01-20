using System;

namespace GeneralsBot {
    public class ObstacleFogTile : FogTile {
        public override void PrettyPrint() {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.Write("  ?  ");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
