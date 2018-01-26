using System.Collections.Generic;
using System.Linq;

namespace GeneralsBot.TargetHeuristics {
    public class CapturePlayerHeuristic : ITargetHeuristic {
        public IList<(int, Position)> Calculate(GameMap map, int playerIndex) {
            IList<(int, Position)>    desireds = new List<(int, Position)>();
            for (int x = 0; x     < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    if (map[x, y] is GeneralTile tile && tile.Faction >= 0 && tile.Faction != playerIndex
                        || map[x, y] is FogTile f && f.Seen && f.KnownType == typeof(GeneralTile)) {
                        desireds.Add((100000000, new Position(x, y)));
                    }
                    
                    if (map[x, y] is CityTile cTile && cTile.Faction >= 0 && cTile.Faction != playerIndex
                     || map[x, y] is FogTile cF && cF.Seen && cF.KnownType == typeof(CityTile)) {
                        desireds.Add((100, new Position(x, y)));
                    }
                }
            }
            
            return desireds;
        }

    }
}
