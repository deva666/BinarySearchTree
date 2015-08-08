using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;


namespace MarkoDevcic
{
    [DebuggerDisplay("Count = {Count}")]
    public class BinarySearchTree<T> : ICollection<T> where T : IComparable<T>
    {
        private Int32 size;
        private volatile Int32 version;

        private Node<T> root;

        public bool IsEmpty
        {
            get { return root == null; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public Int32 Count
        {
            get { return size; }
        }

        void ICollection<T>.Add(T value)
        {
            Add(value);
        }

        public Node<T> Add(T value)
        {
            var node = new Node<T>(value);
            var current = root;
            var parent = default(Node<T>);

            version++;
            size++;

            while (current != null)
            {
                parent = current;
                if (value.CompareTo(current.Value) < 0)
                    current = current.Left;
                else
                    current = current.Right;
            }

            node.Parent = parent;
            if (parent == null)
            {
                root = node;
                return root;
            }

            if (value.CompareTo(parent.Value) < 0)
            {
                parent.Left = node;
            }
            else
            {
                parent.Right = node;
            }


            return node;
        }


        public bool Remove(T value)
        {
            var node = IterativeTreeSearch(value);
            if (node != null)
            {
                Remove(node);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Remove(Node<T> node)
        {
            if (node == null)
                throw new ArgumentNullException("null");

            var deleteNode = default(Node<T>);
            var next = default(Node<T>);

            if (node.Left == null || node.Right == null)
                deleteNode = node;
            else
                deleteNode = Successor(node);

            if (deleteNode.Left != null)
                next = deleteNode.Left;
            else
                next = deleteNode.Right;

            if (next != null)
                next.Parent = deleteNode.Parent;

            if (deleteNode.Parent == null)
                root = next;
            else if (deleteNode.Parent.Left == deleteNode)
                deleteNode.Parent.Left = next;
            else
                deleteNode.Parent.Right = next;

            if (deleteNode != node)
                node.Value = deleteNode.Value;

            size--;
            version++;
        }

        public void Clear()
        {
            root = null;
            size = 0;
            version++;
        }

        public IEnumerable<T> GetSortedValues()
        {
            var values = new List<T>();
            InorderTreeWalk(root, values, version);
            return values;
        }

        private void InorderTreeWalk(Node<T> current, ICollection<T> output, Int32 currentVersion)
        {
            if (current != null)
            {
                CheckVersion(currentVersion);

                InorderTreeWalk(current.Left, output, currentVersion);
                output.Add(current.Value);
                InorderTreeWalk(current.Right, output, currentVersion);
            }
        }

        private IEnumerator<T> IterativePreorderTreeWalk(Int32 currentVersion)
        {
            if (root == null)
                yield break;


            var stack = new Stack<Node<T>>();
            var current = root;
            var finished = false;
            while (finished == false)
            {
                CheckVersion(currentVersion);

                if (current != null)
                {
                    yield return current.Value;
                    stack.Push(current);
                    current = current.Left;
                }
                else
                {
                    if (stack.Count == 0)
                    {
                        finished = true;
                    }
                    else
                    {
                        current = stack.Pop();
                        current = current.Right;
                    }
                }
            }
        }

        private IEnumerator<T> IterativeInorderTreeWalk()
        {
            if (root == null)
                yield break;

            var stack = new Stack<Node<T>>();
            var current = root;
            var finished = false;
            while (finished == false)
            {
                if (current != null)
                {
                    stack.Push(current);
                    current = current.Left;
                }
                else
                {
                    if (stack.Count == 0)
                    {
                        finished = true;
                    }
                    else
                    {
                        current = stack.Pop();
                        yield return current.Value;
                        current = current.Right;
                    }
                }
            }
        }

        public int GetHeight()
        {
            return Height(root);
        }

        private int Height(Node<T> node)
        {
            if (node == null)
                return 0;

            return 1 + Math.Max(Height(node.Left), Height(node.Right));
        }

        public T GetMinimum()
        {
            var node = Minimum(root);
            if (node == null)
                throw new InvalidOperationException("Tree is empty");
            return node.Value;
        }

        private Node<T> Minimum(Node<T> node)
        {
            if (node == null)
                return null;

            var current = node;
            while (current.Left != null)
            {
                current = current.Left;
            }
            return current;
        }

        public T GetMaximum()
        {
            var max = Maximum(root);
            if (max == null)
                throw new InvalidOperationException("Tree is empty");

            return max.Value;
        }

        private Node<T> Maximum(Node<T> node)
        {
            if (node == null)
                return null;

            var current = node;
            while (current.Right != null)
            {
                current = current.Right;
            }
            return current;
        }


        public bool Contains(T value)
        {
            return IterativeTreeSearch(value) != null;
        }

        public bool TryGetNode(T value, out Node<T> node)
        {
            node = TreeSearch(root, value);
            return node != null;
        }

        private Node<T> TreeSearch(Node<T> startNode, T value)
        {
            if (startNode == null)
                return null;

            if (startNode.Value.CompareTo(value) == 0)
                return startNode;

            if (value.CompareTo(startNode.Value) < 0)
                return TreeSearch(startNode.Left, value);
            else
                return TreeSearch(startNode.Right, value);
        }

        private Node<T> IterativeTreeSearch(T value)
        {
            var current = root;
            while (current != null)
            {
                if (current.Value.CompareTo(value) == 0)
                    break;

                if (value.CompareTo(current.Value) < 0)
                    current = current.Left;
                else
                    current = current.Right;
            }

            return current;
        }

        public Node<T> Successor(Node<T> node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.Right != null)
                return Minimum(node.Right);

            var parent = node.Parent;
            var rightChild = node;

            while (parent != null && rightChild == parent.Right)
            {
                rightChild = parent;
                parent = parent.Parent;
            }

            return parent;
        }

        public Node<T> Predecessor(Node<T> node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node.Left != null)
                return Maximum(node.Left);

            var parent = node.Parent;
            var leftChild = node;

            while (parent != null && parent.Left == leftChild)
            {
                leftChild = parent;
                parent = parent.Parent;
            }

            return parent;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return IterativePreorderTreeWalk(version);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return IterativePreorderTreeWalk(version);
        }


        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");

            if (array.Length - arrayIndex < size)
                throw new InvalidOperationException("array not big enough");

            foreach (var value in this)
            {
                array[arrayIndex++] = value;
            }
        }

        private void CheckVersion(Int32 currentVersion)
        {
            if (version != currentVersion)
            {
                throw new InvalidOperationException("Collection was modified");
            }
        }
    }

    public sealed class Node<TValue> : IComparable<Node<TValue>>
        where TValue : IComparable<TValue>
    {
        public Node<TValue> Left { get; internal set; }

        public Node<TValue> Right { get; internal set; }

        public Node<TValue> Parent { get; internal set; }

        public TValue Value { get; internal set; }

        internal Node(TValue value)
        {
            Value = value;
        }

        public int CompareTo(Node<TValue> other)
        {
            if (other == null)
            {
                return -1;
            }

            return this.Value.CompareTo(other.Value);
        }
    }
}
