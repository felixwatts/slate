using System;

namespace Slate.Core
{
    public class Point : IEquatable<Point>
    {
        public int X { get; }
        public int Y { get; }

        public static Point Zero { get; } = new Point(0, 0);

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }

        public static Point operator -(Point left, Point right)
        {
            return new Point(left.X - right.X, left.Y - right.Y);
        }

        public static Point operator -(Point point)
        {
            return new Point(-point.X, -point.Y);
        }

        public static bool operator ==(Point left, Point right)
        {
            if((object)left == null) return (object)right == null;
            return left.Equals(right);
        }

        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Point);
        }

        public bool Equals(Point other)
        {
            if((object)other == null) return false;
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + X.GetHashCode();
                hash = hash * 31 + Y.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
