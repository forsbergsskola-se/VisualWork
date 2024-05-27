using System;
using Collections;
using Grids;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Grid = Grids.Grid;
using Object = UnityEngine.Object;

namespace Tests
{
    public class PriorityQueueTests
    {
        [Test]
        public void DequeuesSmallestIntegerFirst()
        {
            // given
            PriorityQueue<string> queue = new();
            // when
            queue.Enqueue("Hello", 5);
            queue.Enqueue("World", 4);
            queue.Enqueue("C#", 6);
            
            // then
            Assert.That(queue.Dequeue(), Is.EqualTo("World"));
        }
    }
}