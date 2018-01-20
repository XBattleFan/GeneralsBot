namespace GeneralsBot {
    public class Attack {
        public readonly Position From;
        public readonly Position To;
        public readonly bool Is50;

        public Attack(Position from, Position to, bool is50) {
            From = from;
            To = to;
            Is50 = is50;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            return Equals((Attack) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (From                  != null ? From.GetHashCode() : 0);
                hashCode     = (hashCode * 397) ^ (To != null ? To.GetHashCode() : 0);
                hashCode     = (hashCode * 397) ^ Is50.GetHashCode();
                return hashCode;
            }
        }
    }
}
