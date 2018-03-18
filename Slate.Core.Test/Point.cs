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
        public void Add(int xLeft, int yLeft, int xRight, int yRight, int expectedX, int expectedY)
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
        public void Subtract(int xLeft, int yLeft, int xRight, int yRight, int expectedX, int expectedY)
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
        public void Negate(int x, int y, long expectedX, long expectedY)
        {
            var point = new Point(x, y);

            var actual = - point;

            Assert.AreEqual(expectedX, actual.X);
            Assert.AreEqual(expectedY, actual.Y);
        }

        [Test]
        public void Equality()
        {
            Point left = null, right = null;

            // self equality
            Assert.IsTrue(left == left);
            left = Point.Zero;
            Assert.IsTrue(left == left);
            Assert.IsTrue(left.Equals(left));
            left = new Point(int.MaxValue, int.MaxValue);
            Assert.IsTrue(left == left);
            Assert.IsTrue(left.Equals(left));

            // null equality
            left = null; right = null;
            Assert.IsFalse(left == right);
            Assert.IsFalse(right == left);

            // null/non null inequality
            left = Point.Zero; right = null;
            Assert.IsFalse(left == right);
            Assert.IsFalse(right == left);
            Assert.IsFalse(left.Equals(right));

            // same object equality
            left = Point.Zero; right = Point.Zero;
            Assert.IsTrue(left == right);
            Assert.IsTrue(right == left);
            Assert.IsTrue(left.Equals(right));
            Assert.IsTrue(right.Equals(left));

            // same value equality
            left = Point.Zero; right = new Point(0, 0);
            Assert.IsTrue(left == right);
            Assert.IsTrue(right == left);
            Assert.IsTrue(left.Equals(right));

            left = new Point(42, 42); right = new Point(42, 42);
            Assert.IsTrue(left == right);
            Assert.IsTrue(right == left);
            Assert.IsTrue(left.Equals(right));

            left = new Point(int.MaxValue, int.MaxValue); right = new Point(int.MaxValue, int.MaxValue);
            Assert.IsTrue(left == right);
            Assert.IsTrue(right == left);
            Assert.IsTrue(left.Equals(right));

            // different value inequality
            left = new Point(1, 0); right = new Point(0, 0);
            Assert.IsFalse(left == right);
            Assert.IsFalse(right == left);
            Assert.IsFalse(left.Equals(right));

            left = new Point(1, 0); right = new Point(0, 1);
            Assert.IsFalse(left == right);
            Assert.IsFalse(right == left);
            Assert.IsFalse(left.Equals(right));
        }
    }
}