using System;

namespace Slate.Core
{
    public class Region
    {
        public Point TopLeft { get; }
        public Point BottomRight { get; }

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
    }
}