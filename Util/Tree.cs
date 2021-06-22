// Copyright (C) 2021 Alexander Stojanovich
//
// This file is part of FOnlineDatRipper.
//
// FOnlineDatRipper is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
//
// FOnlineDatRipper is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with FOnlineDatRipper. If not, see http://www.gnu.org/licenses/.

namespace FOnlineDatRipper
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// N-Tree which contains Nodes of specified data type.
    /// </summary>
    /// <typeparam name="T">data type of the tree .</typeparam>
    internal class Tree<T>
    {
        /// <summary>
        /// Defines the root.
        /// </summary>
        private readonly Node<T> root;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tree{T}"/> class.
        /// </summary>
        /// <param name="root">The root<see cref="Node{T}"/>.</param>
        public Tree(Node<T> root)
        {
            this.root = root;
        }

        /// <summary>
        /// Gets the Root.
        /// </summary>
        internal Node<T> Root => root;

        /// <summary>
        /// Add the given node to the tree as a child node.
        /// </summary>
        /// <param name="node"> given node.</param>
        public void AddChild(Node<T> node)
        {
            node.Parent = Root;
            Root.Children.Add(node);
        }

        /// <summary>
        /// Does tree contain child with the given data.
        /// </summary>
        /// <param name="t">given data.</param>
        /// <returns>true if tree has that node as a child, otherwise false.</returns>
        public bool ContainsChild(T t)
        {
            Predicate<Node<T>> predicate = delegate (Node<T> node)
            {
                return node.Data.Equals(t);
            };

            return Root.Children.Exists(predicate);
        }

        /// <summary>
        /// Clears tree of children nodes.
        /// </summary>
        public void Clear()
        {
            root.Children.Clear();
        }

        /// <summary>
        /// Pre-order traversal of this tree.
        /// </summary>
        /// <returns>list containing nodes.</returns>
        public List<Node<T>> Preorder()
        {
            List<Node<T>> datalist = new List<Node<T>>();
            Stack<Node<T>> stack = new Stack<Node<T>>();
            stack.Push(root);

            while (stack.Count > 0)
            {
                Node<T> node = stack.Pop();
                datalist.Add(node);

                // push them in reverse order 
                // notice this is imporant since natural order is gonna be preserved
                for (int i = node.Children.Count - 1; i >= 0; i--)
                {
                    stack.Push(node.Children[i]);
                }
            }

            return datalist;
        }

        /// <summary>
        /// Get node with parsed data.
        /// </summary>
        /// <param name="t">The t<see cref="T"/>.</param>
        /// <returns>The <see cref="Node{T}"/>.</returns>
        public Node<T> getNode(T t)
        {
            Predicate<Node<T>> predicate = delegate (Node<T> node)
            {
                return node.Data.Equals(t);
            };

            return Preorder().Find(predicate);
        }
    }
}
