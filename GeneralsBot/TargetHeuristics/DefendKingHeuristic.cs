using System.Collections.Generic;

namespace GeneralsBot.TargetHeuristics {
    public class DefendKingHeuristic : ITargetHeuristic {
        public IList<(int, Position)> Calculate(GameMap map, int playerIndex) {
            // Check if king is under threat
            Position king = map.GeneralPosition(playerIndex);
            int threat = 0;
            foreach (Position p in king.Surrounding(map, 5)) {
                if (map[p] is OccupiedTile occup && occup.Faction != playerIndex && occup.Faction >= 0) {
                    threat += occup.Units;
                }
            }

            threat -= ((OccupiedTile) map[king]).Units;
            
            return new List<(int, Position)> { (threat * 1000, king) };
        }
    }
}
