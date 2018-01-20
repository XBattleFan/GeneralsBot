using System;
using System.Collections.Generic;

namespace GeneralsBot {
    public class Position {
        public readonly int X;
        public readonly int Y;
        
        public Position(int x, int y) {
            X = x;
            Y = y;
        }
        
        public static Position operator+(Position a, Position b) {
            return new Position(a.X + b.X, a.Y + b.Y);
        }
        
        public static Position operator-(Position a, Position b) {
            return new Position(a.X - b.X, a.Y - b.Y);
        }

        public override bool Equals(object obj) {
            if (!(obj is Position p)) return false;

            return p.X == X && p.Y == Y;

        }

        public static IList<Position> Surrounding(Position p, GameMap map, bool include = false) {
            IList<Position> positions = new List<Position>();
            
            for (int x = Math.Max(p.X - 1, 0); x <= Math.Min(p.X + 1, map.Width - 1); x++) {
                for (int y = Math.Max(p.Y - 1, 0); y <= Math.Min(p.Y + 1, map.Height - 1); y++) {
                    if (!include && new Position(x, y).Equals(p)) continue;
                    positions.Add(new Position(x, y));
                }
            }

            return positions;
        }
    }
}
