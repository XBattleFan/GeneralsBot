using System;

namespace GeneralsBot {
    public class MountainTile : GameTile {
        public override void PrettyPrint() {
            Console.BackgroundColor = ConsoleColor.Cyan;
            Console.Write("  ^  ");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
