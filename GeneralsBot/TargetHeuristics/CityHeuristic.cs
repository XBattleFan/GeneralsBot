using System;
using System.Collections.Generic;

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
                            desireds.Add((Math.Min(totalArmies - tile.Units, 50), position));
                        }
                    }
                }
            }
            
            return desireds;
        }
    }
}
