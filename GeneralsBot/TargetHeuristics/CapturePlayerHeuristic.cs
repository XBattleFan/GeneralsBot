using System.Collections.Generic;
using System.Linq;

namespace GeneralsBot.TargetHeuristics {
    public class CapturePlayerHeuristic : ITargetHeuristic {
        public IList<(int, Position)> Calculate(GameMap map, int playerIndex) {
            IList<(int, Position)>    desireds = new List<(int, Position)>();
            for (int x = 0; x     < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    if (map[x, y] is GeneralTile tile && tile.Faction >= 0 && tile.Faction != playerIndex) {
                        
                    }
                }
            }
            
            return desireds;
        }

    }
}
