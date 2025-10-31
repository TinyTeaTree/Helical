using System;

namespace ChessRaid
{
    [System.Serializable]
    public struct Coord
    {
        public int X;
        public int Y;

        // Constructor
        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        // Implicit conversion from (int, int) to Coord
        public static implicit operator Coord((int x, int y) tuple)
        {
            return new Coord(tuple.x, tuple.y);
        }

        // Override ToString() for better string representation
        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        // Override Equals() and GetHashCode() for value comparison
        public override bool Equals(object obj)
        {
            if (obj is Coord)
            {
                Coord other = (Coord)obj;
                return X == other.X && Y == other.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        // Operator overloads for arithmetic operations
        public static Coord operator +(Coord a, Coord b)
        {
            return new Coord(a.X + b.X, a.Y + b.Y);
        }

        public static Coord operator -(Coord a, Coord b)
        {
            return new Coord(a.X - b.X, a.Y - b.Y);
        }

        public static bool operator ==(Coord a, Coord b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Coord a, Coord b)
        {
            return !a.Equals(b);
        }
    }
}