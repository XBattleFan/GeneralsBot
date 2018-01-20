using System;

namespace GeneralsBot {
    public class OccupiedTile : GameTile {
        public readonly int Faction;
        public readonly int Units;
        
        public OccupiedTile(int faction, int units) {
            Faction = faction;
            Units = units;
        }

        public override void PrettyPrint() {
            int padding = (int) Math.Ceiling((5 - Units.ToString().Length) / (decimal) 2);
            Console.Write(Units.ToString().PadLeft(padding + Units.ToString().Length).PadRight(5));
        }
    }
}
