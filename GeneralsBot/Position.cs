using System;
using System.Collections.Generic;
using System.Linq;

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
        
        public override int GetHashCode() {
            unchecked {
                return (X * 397) ^ Y;
            }
        }

        public IList<Position> Surrounding(GameMap map, bool include = false) {
            IList<Position> positions = new List<Position>();
            
            for (int x = Math.Max(X - 1, 0); x <= Math.Min(X + 1, map.Width - 1); x++) {
                for (int y = Math.Max(Y - 1, 0); y <= Math.Min(Y + 1, map.Height - 1); y++) {
                    if (!include && new Position(x, y).Equals(this)) continue;
                    positions.Add(new Position(x, y));
                }
            }

            return positions;
        }

        public int NaiveMoveDistance(Position other) {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        public IEnumerable<Position> SurroundingMoveable(GameMap map) {
            return Surrounding(map).Where(p => (p.X != X) != (p.Y != Y));
        }
    }
}
