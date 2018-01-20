using System;

namespace GeneralsBot {
    public class EmptyTile : GameTile {
        public override void PrettyPrint() {
            Console.Write("     ");
        }
    }
}
