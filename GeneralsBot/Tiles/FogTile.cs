using System;

namespace GeneralsBot {
    public class FogTile : GameTile {
        public override void PrettyPrint() {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("  ~  ");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
