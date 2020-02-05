using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class TestDataStructure
    {
        [Test]
        public void TestPriorityQueue()
        {
            var q = new PriorityQueue<int>(new IntC());
            q.Enqueue(9523);
            Assert.IsTrue(q.Peek() == 9523);
            q.Enqueue(4152);
            Assert.IsTrue(q.Peek() == 4152);
            q.Enqueue(7772);
            Assert.IsTrue(q.Peek() == 4152);
            q.Enqueue(10240);
            Assert.IsTrue(q.Peek() == 4152);
            q.Enqueue(1);
            Assert.IsTrue(q.Peek() == 1);
            q.Enqueue(-2);
            Assert.IsTrue(q.Peek() == -2);
            q.Enqueue(233);
            Assert.IsTrue(q.Peek() == -2);

            Assert.IsTrue(q.Dequeue() == -2);
            Assert.IsTrue(q.Peek() == 1);
            Assert.IsTrue(q.Dequeue() == 1);
            Assert.IsTrue(q.Peek() == 233);
            Assert.IsTrue(q.Dequeue() == 233);
            Assert.IsTrue(q.Peek() == 4152);
            Assert.IsTrue(q.Dequeue() == 4152);
            Assert.IsTrue(q.Peek() == 7772);
            Assert.IsTrue(q.Dequeue() == 7772);
            Assert.IsTrue(q.Peek() == 9523);
            Assert.IsTrue(q.Dequeue() == 9523);
            Assert.IsTrue(q.Peek() == 10240);
            Assert.IsTrue(q.Dequeue() == 10240);
            Assert.Catch<InvalidOperationException>(() => q.Dequeue());
            Assert.Catch<InvalidOperationException>(() => q.Peek());
        }

        private class IntC : Comparer<int>
        {
            public override int Compare(int x, int y)
            {
                if (x < y)
                {
                    return 1;
                }

                if (x == y)
                {
                    return 0;
                }

                return -1;
            }
        }
    }
}