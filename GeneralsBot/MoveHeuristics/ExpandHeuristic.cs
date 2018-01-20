using System;
using System.Collections.Generic;

namespace GeneralsBot.MoveHeuristics {
    public class ExpandHeuristic : IMoveHeuristic {
        public IList<(int, Attack)> Calculate(GameMap map, int playerIndex) {
            IList<(int, Attack)> desireds = new List<(int, Attack)>();
            
            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    if (map[x, y] is OccupiedTile tile && tile.Faction == playerIndex && tile.Units > 1) {
                        Position current = new Position(x, y);
                        
                        foreach (Position p in Position.Surrounding(current, map)) {
                            if (!(map[p.X, p.Y] is EmptyTile)) continue;
                            if (p.X != x && p.Y != y) continue;
                            desireds.Add((2, new Attack(current, p, true)));
                        }
                    }
                }
            }
            
            return desireds;
        }
    }
}
