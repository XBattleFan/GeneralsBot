using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralsBot.TargetHeuristics {
    public class AttackHeuristic : ITargetHeuristic {
        public IList<(int, Position)> Calculate(GameMap map, int playerIndex) {
            IList<(int, Position)> desireds = new List<(int, Position)>();
            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    if (map[x, y] is OccupiedTile tile && tile.Faction >= 0 && tile.Faction != playerIndex) {
                        Position position = new Position(x, y);
                        int desire = map.TotalArmies(playerIndex) - map.TotalArmies(tile.Faction);
                        desire *= 3;
                        if (map.UnitsAt(position) <= 2 && desire >= 0) desire *= 2;
                        else if (map.UnitsAt(position) <= 2) desire /= 2;
                        if (position.NaiveMoveDistance(map.GeneralPosition(playerIndex)) < 5) {
                            desire += 1000;
                        }

                        desire += 10 * position.SurroundingMoveable(map).Count(p => map[p] is FogTile);
                        
                        desireds.Add((desire, position));
                    }
                }
            }
            
            return desireds;
        }
    }
}
