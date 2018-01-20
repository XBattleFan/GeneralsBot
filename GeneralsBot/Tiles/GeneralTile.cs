using System;

namespace GeneralsBot {
    public class GeneralTile : CityTile {
        public GeneralTile(int faction, int units) : base(faction, units) { }
        
        public override void PrettyPrint() {
            Console.BackgroundColor = ConsoleColor.Blue;
            int padding             = (int) Math.Ceiling((5 - Units.ToString().Length) / (decimal) 2);
            Console.Write(Units.ToString().PadLeft(padding  + Units.ToString().Length).PadRight(5));
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
