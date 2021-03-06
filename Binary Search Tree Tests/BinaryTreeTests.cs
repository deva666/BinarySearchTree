﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MarkoDevcic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class BinaryTreeTests
    {
        [TestMethod]
        public void TestSortedValues()
        {
            var tree = new BinarySearchTree<int>();

            var size = 100000;
            var numbers = new int[size];
            var rand = new Random();
            for (int i = 0; i < size; i++)
            {
                numbers[i] = rand.Next(0, size);
                tree.Add(numbers[i]);
            }

            Array.Sort(numbers);
            var sortedTreeValues = tree.GetSortedValues().ToList();
            for (int i = 0; i < size; i++)
            {
                Assert.AreEqual(numbers[i], sortedTreeValues[i]);
            }
        }

        [TestMethod]
        public void TestTreeCount()
        {
            var tree = new BinarySearchTree<int>();
            var size = 100;

            Node<int> node = null;
            for (int i = 0; i < size; i++)
            {
                node = tree.Add(i);
            }

            Assert.AreEqual(tree.Count, size);

            tree.Remove(node);

            Assert.AreEqual(tree.Count, size - 1);

            tree.Add(101);

            Assert.AreEqual(tree.Count, size);
        }

        [TestMethod]
        public void TestIsEmpty()
        {
            var tree = new BinarySearchTree<string>();
            Assert.IsTrue(tree.IsEmpty);

            tree.Add("a");

            Assert.IsFalse(tree.IsEmpty);
        }

        [TestMethod]
        public void TestGetMaximum()
        {
            var tree = new BinarySearchTree<int>();

            try
            {
                tree.GetMaximum();
                Assert.Fail("Should not get here if tree is empty");
            }
            catch (InvalidOperationException)
            {

            }

            var size = 100;
            for (int i = 0; i < size; i++)
            {
                tree.Add(i);
            }

            Assert.AreEqual(tree.GetMaximum(), size - 1);
        }

        [TestMethod]
        public void TestGetMinimum()
        {
            var tree = new BinarySearchTree<int>();

            try
            {
                tree.GetMinimum();
                Assert.Fail("Should not get here if tree is empty");
            }
            catch (InvalidOperationException)
            {

            }

            var size = 100;
            for (int i = 0; i < size; i++)
            {
                tree.Add(i);
            }

            Assert.AreEqual(tree.GetMinimum(), 0);
        }

        [TestMethod]
        public void TestGetValues()
        {
            var tree = new BinarySearchTree<int>();

            var size = 10000;
            var random = new Random();
            var numbers = new int[size];
            for (int i = 0; i < size; i++)
            {
                numbers[i] = random.Next();
                tree.Add(numbers[i]);
            }

            var values = tree.ToList();

            Assert.AreEqual(values.Count, numbers.Length);

            foreach (var n in numbers)
            {
                Assert.IsTrue(values.Contains(n));
                values.Remove(n);
            }

            Assert.AreEqual(values.Count, 0);
        }

        [TestMethod]
        public void TestDelete()
        {
            var tree = new BinarySearchTree<int>();
            var size = 10;
            for (int i = 0; i < size; i++)
            {
                tree.Add(i);
            }

            Assert.AreEqual(tree.Count, size);

            Node<int> node;
            for (int i = 0; i < size; i++)
            {
                tree.TryGetNode(i, out node);
                tree.Remove(node);
            }

            Assert.AreEqual(tree.Count, 0);
            Assert.IsTrue(tree.IsEmpty);
        }

        [TestMethod]
        public void TestTryGet()
        {
            var tree = new BinarySearchTree<int>();
            var size = 10;
            for (int i = 0; i < size; i++)
            {
                tree.Add(i);
            }

            Node<int> node;
            for (int i = 0; i < size; i++)
            {
                Assert.IsTrue(tree.TryGetNode(i, out node));
            }

            Assert.IsFalse(tree.TryGetNode(1000, out node));
        }

        [TestMethod]
        public void TestContains()
        {
            var tree = new BinarySearchTree<int>();
            var size = 10;
            for (int i = 0; i < size; i++)
            {
                tree.Add(i);
            }

            for (int i = 0; i < size; i++)
            {
                Assert.IsTrue(tree.Contains(i));
            }

            Assert.IsFalse(tree.Contains(1000));
        }

        [TestMethod]
        public void TestGetSuccessor()
        {
            var tree = new BinarySearchTree<int>();
            var size = 10;
            Node<int> node = null;
            for (int i = 0; i < size; i++)
            {
                node = tree.Add(i);
            }

            for (int i = 0; i < size; i++)
            {
                tree.TryGetNode(i, out node);
                var next = tree.Successor(node);
                if (i < size - 1)
                    Assert.AreEqual(node.Value + 1, next.Value);
                else
                    Assert.IsNull(next);
            }
        }

        [TestMethod]
        public void TestGetPredecessor()
        {
            var tree = new BinarySearchTree<int>();
            var size = 10;
            Node<int> node = null;
            for (int i = 0; i < size; i++)
            {
                node = tree.Add(i);
            }

            for (int i = size - 1; i >= 0; i--)
            {
                tree.TryGetNode(i, out node);
                var prev = tree.Predecessor(node);
                if (i > 0)
                    Assert.AreEqual(node.Value - 1, prev.Value);
                else
                    Assert.IsNull(prev);
            }
        }

        [TestMethod]
        public void TestClear()
        {
            var tree = new BinarySearchTree<int>();
            var size = 10;
            for (int i = 0; i < size; i++)
            {
                tree.Add(i);
            }

            tree.Clear();
            Assert.AreEqual(tree.Count, 0);
            Assert.IsTrue(tree.IsEmpty);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestCollectionWasModified()
        {
            var tree = new BinarySearchTree<int>();
            var size = 10000;
            var random = new Random();
            for (int i = 0; i < size; i++)
            {
                tree.Add(i);
            }

            Task.Run(() => { tree.Add(1); });

            foreach (var item in tree)
            {
                for (int i = 0; i < item; i++)
                {
                    i *= i;
                }
            }
        }

        [TestMethod]
        public void TestGetHeight()
        {
            var tree = new BinarySearchTree<int>();
            var size = 10000;
            var random = new Random();

            //building a search tree from sorted values will give unbalanced tree, size should equal to height
            for (int i = 0; i < size; i++)
            {
                tree.Add(i);
            }

            Assert.AreEqual(tree.GetHeight(), size);
        }

        [TestMethod]
        public void TestNodeCompare()
        {
            var lowerNode = new Node<int>(1);
            var lowerNode2 = new Node<int>(1);
            var higherNode = new Node<int>(2);

            Assert.AreEqual(-1, lowerNode.CompareTo(higherNode));
            Assert.AreEqual(1, higherNode.CompareTo(lowerNode));
            Assert.AreEqual(-1, lowerNode.CompareTo(null));
            Assert.AreEqual(0, lowerNode.CompareTo(lowerNode2));
        }
    }
}
