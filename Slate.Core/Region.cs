using System;
using System.Collections;
using System.Collections.Generic;

namespace Slate.Core
{
    public class Region : IEnumerable<Point>, IEquatable<Region>
    {
        public static Region Empty { get; } = new Region(Point.Zero, Point.Zero);

        public Point TopLeft { get; }
        public Point BottomRight { get; }

        public bool IsEmpty  => TopLeft.X == BottomRight.X || TopLeft.Y == BottomRight.Y;

        public Region(Point topLeft, Point bottomRight)
        {
            TopLeft = new Point(
                Math.Min(topLeft.X, bottomRight.X),
                Math.Min(topLeft.Y, bottomRight.Y));

            BottomRight = new Point(
                Math.Max(topLeft.X, bottomRight.X),
                Math.Max(topLeft.Y, bottomRight.Y));
        }

        public static Region FromBottomRight(Point bottomRight)
        {
            return new Region(Point.Zero, bottomRight);
        }

        public static Region FromCell(Point at)
        {
            return new Region(at, new Point(at.X+1, at.Y+1));
        }

        public Region IntersectionWith(Region other)
        {
            if(TopLeft.X >= other.BottomRight.X 
                || TopLeft.Y >= other.BottomRight.Y
                || other.TopLeft.X >= BottomRight.X
                || other.TopLeft.Y >= BottomRight.Y)
                return Empty;

            return new Region(
                new Point(
                    Math.Max(TopLeft.X, other.TopLeft.X),
                    Math.Max(TopLeft.Y, other.TopLeft.Y)),
                 new Point(
                    Math.Min(BottomRight.X, other.BottomRight.X),
                    Math.Min(BottomRight.Y, other.BottomRight.Y)));
        }

        public Region Translate(Point offset)
        {
            return new Region(TopLeft + offset, BottomRight + offset);
        }

        public IEnumerator<Point> GetEnumerator()
        {
            for(int x = TopLeft.X; x < BottomRight.X; x++)
            {
                for(int y = TopLeft.Y; y < BottomRight.Y; y++)
                {
                    yield return new Point(x, y);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<Point>).GetEnumerator();
        }

        public bool Equals(Region other)
        {
            if(other == null) return false;
            return TopLeft.Equals(other.TopLeft) && BottomRight.Equals(other.BottomRight);
        }

        public override string ToString()
        {
            return $"{TopLeft}->{BottomRight}";
        }
    }
}