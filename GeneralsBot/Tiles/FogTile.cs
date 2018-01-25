using System;

namespace GeneralsBot {
    public class FogTile : GameTile {
        public readonly bool Seen;
        public readonly Type KnownType;
        
        public FogTile(bool seen = false, Type knownType = typeof(GameTile)) {
            Seen = seen;
            KnownType = knownType;
        }

        public override void PrettyPrint() {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.Write("  ~  ");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
