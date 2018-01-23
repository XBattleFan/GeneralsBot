using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralsBot.TargetHeuristics {
    public class CityHeuristic : ITargetHeuristic {
        public IList<(int, Position)> Calculate(GameMap map, int playerIndex) {
            IList<(int, Position)> desireds = new List<(int, Position)>();
            
            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    if (map[x, y] is CityTile tile && tile.Faction != playerIndex) {
                        Position position = new Position(x, y);
                        int totalArmies = map.TotalArmies(playerIndex);
                        if (totalArmies > 80) {
                            desireds.Add((totalArmies - tile.Units, position));
                        }

                        if (position.SurroundingMoveable(map)
                                    .Any(p => map[p] is OccupiedTile t && t.Units      > tile.Units
                                                                       && t.Faction    == playerIndex
                                                                       && tile.Faction == 0)) {
                            desireds.Add((1000, position));
                        }
                    }
                }
            }
            
            return desireds;
        }
    }
}
