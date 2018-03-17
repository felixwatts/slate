using System;

namespace Slate.Core
{
    public class Point : IEquatable<Point>
    {
        public long X { get; }
        public long Y { get; }

        public Point(long x, long y)
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

        public bool Equals(Point other)
        {
            throw new NotImplementedException();
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
    }
}
