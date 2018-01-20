using System;
using System.Collections.Generic;

namespace GeneralsBot.TargetHeuristics {
    public class ExpandHeuristic : ITargetHeuristic {
        public IList<(int, Position)> Calculate(GameMap map, int playerIndex) {
            IList<(int, Position)> desireds = new List<(int, Position)>();
            
            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    if (map[x, y] is OccupiedTile tile && tile.Faction == playerIndex) {
                        Position current = new Position(x, y);
                        
                        foreach (Position p in current.SurroundingMoveable(map)) {
                            if (map[p.X, p.Y] is MountainTile || map[p.X, p.Y] is CityTile
                                || map[p.X, p.Y] is OccupiedTile occ && occ.Faction == playerIndex) continue;

                            int points = - p.NaiveMoveDistance(map.GeneralPosition(playerIndex));
                            if (map[p.X, p.Y] is OccupiedTile occupied) points -= occupied.Units;
                            desireds.Add((points, p));
                        }
                    }
                }
            }
            
            return desireds;
        }
    }
}
