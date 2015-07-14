using System;
using System.Collections.Generic;

namespace MarkoDevcic
{
    public class BinaryTree<T> where T : IComparable<T>
    {
        private Int32 size;

        private Node<T> root;

        public bool IsEmpty
        {
            get { return root == null; }
        }

        public Int32 Count { get { return size; } }

        public Node<T> Insert(T value)
        {
            size++;

            var node = new Node<T>(value);
            var current = root;
            var parent = default(Node<T>);
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
                parent.Left = node;
            else
                parent.Right = node;

            return node;
        }


        public void Delete(Node<T> node)
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
        }


        public IEnumerable<T> GetValues()
        {
            var values = new List<T>();
            PreorderTreeWalk(root, values);
            return values;
        }

        private void PreorderTreeWalk(Node<T> current, ICollection<T> output)
        {
            if (current != null)
            {
                output.Add(current.Value);
                PreorderTreeWalk(current.Left, output);
                PreorderTreeWalk(current.Right, output);
            }
        }

        public IEnumerable<T> GetSortedValues()
        {
            var values = new List<T>();
            InorderTreeWalk(root, values);
            return values;
        }

        private void InorderTreeWalk(Node<T> current, ICollection<T> output)
        {
            if (current != null)
            {
                InorderTreeWalk(current.Left, output);
                output.Add(current.Value);
                InorderTreeWalk(current.Right, output);
            }
        }

        private IEnumerable<T> IterativePreorderTreeWalk()
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
                    yield return current.Value;
                    stack.Push(current);
                    current = current.Left;
                }
                else
                {
                    if (stack.Count == 0)
                        finished = true;

                    current = stack.Pop();
                    current = current.Right;
                }
            }
        }

        private IEnumerable<T> IterativeInorderTreeWalk()
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
                        finished = true;
                    else
                    {
                        current = stack.Pop();
                        yield return current.Value;
                        current = current.Right;
                    }
                }
            }
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

        private Node<T> TreeSearch(Node<T> node, T value)
        {
            if (node == null)
                return null;

            if (node.Value.CompareTo(value) == 0)
                return node;

            if (value.CompareTo(node.Value) < 0)
                return TreeSearch(node.Left, value);
            else
                return TreeSearch(node.Right, value);
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
            if (root == null)
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
    }

    public sealed class Node<TValue> : IComparable<Node<TValue>> where TValue : IComparable<TValue>
    {
        public Node<TValue> Left { get; internal set; }

        public Node<TValue> Right { get; internal set; }

        public Node<TValue> Parent { get; internal set; }

        public TValue Value { get; internal set; }

        internal Node(TValue value)
        {
            Value = value;
        }

        internal Node(Node<TValue> parent, Node<TValue> left, Node<TValue> right, TValue value)
        {
            Parent = parent;
            Left = left;
            Right = right;
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
