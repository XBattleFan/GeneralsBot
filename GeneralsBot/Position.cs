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
    }
}
