using System;

namespace GeneralsBot {
    public class CityTile : OccupiedTile {
        public CityTile(int faction, int units) : base(faction, units) { }

        public override void PrettyPrint() {
            Console.BackgroundColor = ConsoleColor.Yellow;
            int padding = (int) Math.Ceiling((5 - Units.ToString().Length) / (decimal) 2);
            Console.Write(Units.ToString().PadLeft(padding + Units.ToString().Length).PadRight(5));
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
