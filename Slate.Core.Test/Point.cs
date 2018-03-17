using NUnit.Framework;

namespace Slate.Core.Test
{
    public class PointTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(0, 0, 0, 0, 0, 0)]        
        [TestCase(0, 0, 1, 0, 1, 0)]
        [TestCase(0, 0, 0, 1, 0, 1)]
        [TestCase(1, 0, 0, 0, 1, 0)]
        [TestCase(0, 1, 0, 0, 0, 1)]
        [TestCase(1, 0, 1, 0, 2, 0)]
        [TestCase(0, 1, 0, 1, 0, 2)]
        [TestCase(1, 0, 0, 1, 1, 1)]
        [TestCase(1, 0, -1, 0, 0, 0)]
        [TestCase(-1, 0, 1, 0, 0, 0)]
        [TestCase(-1, 0, -1, 0, -2, 0)]
        [TestCase(0, 1, 0, -1, 0, 0)]
        [TestCase(0, -1, 0, 1, 0, 0)]
        [TestCase(0, -1, 0, -1, 0, -2)]
        public void Add(long xLeft, long yLeft, long xRight, long yRight, long expectedX, long expectedY)
        {
            var left = new Point(xLeft, yLeft);
            var right = new Point(xRight, yRight);

            var actual = left + right;

            Assert.AreEqual(expectedX, actual.X);
            Assert.AreEqual(expectedY, actual.Y);
        }

        [TestCase(0, 0, 0, 0, 0, 0)]        
        [TestCase(0, 0, 1, 0, -1, 0)]
        [TestCase(0, 0, 0, 1, 0, -1)]
        [TestCase(1, 0, 0, 0, 1, 0)]
        [TestCase(0, 1, 0, 0, 0, 1)]
        [TestCase(1, 0, 1, 0, 0, 0)]
        [TestCase(0, 1, 0, 1, 0, 0)]
        [TestCase(1, 0, 0, 1, 1, -1)]
        [TestCase(1, 0, -1, 0, 2, 0)]
        [TestCase(-1, 0, 1, 0, -2, 0)]
        [TestCase(-1, 0, -1, 0, 0, 0)]
        [TestCase(0, 1, 0, -1, 0, 2)]
        [TestCase(0, -1, 0, 1, 0, -2)]
        [TestCase(0, -1, 0, -1, 0, 0)]
        public void Subtract(long xLeft, long yLeft, long xRight, long yRight, long expectedX, long expectedY)
        {
            var left = new Point(xLeft, yLeft);
            var right = new Point(xRight, yRight);

            var actual = left - right;

            Assert.AreEqual(expectedX, actual.X);
            Assert.AreEqual(expectedY, actual.Y);
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(1, 0, -1, 0)]
        [TestCase(0, 1, 0, -1)]
        [TestCase(1, 1, -1, -1)]
        [TestCase(-1, -1, 1, 1)]
        public void Negate(long x, long y, long expectedX, long expectedY)
        {
            var point = new Point(x, y);

            var actual = - point;

            Assert.AreEqual(expectedX, actual.X);
            Assert.AreEqual(expectedY, actual.Y);
        }
    }
}