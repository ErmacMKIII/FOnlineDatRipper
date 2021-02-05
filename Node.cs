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
    using System.Collections.Generic;

    /// <summary>
    /// Tree node with the specified node data type.
    /// </summary>
    /// <typeparam name="T">data type of the tree node .</typeparam>
    internal class Node<T>
    {
        /// <summary>
        /// Defines the data.
        /// </summary>
        private T data;

        /// <summary>
        /// Defines the parent.
        /// </summary>
        private Node<T> parent;

        /// <summary>
        /// Defines the children.
        /// </summary>
        private readonly List<Node<T>> children = new List<Node<T>>();

        /// <summary>
        /// Gets the Children.
        /// </summary>
        public List<Node<T>> Children => children;

        /// <summary>
        /// Gets or sets the Parent.
        /// </summary>
        public Node<T> Parent { get => parent; set => parent = value; }

        /// <summary>
        /// Gets or sets the data of this node.....
        /// </summary>
        public T Data { get => data; set => data = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class.
        /// </summary>
        /// <param name="data">.</param>
        public Node(T data)
        {
            this.data = data;
        }

        /// <summary>
        /// Gets the level of the node, root level is zero.
        /// </summary>
        /// <returns>node level.</returns>
        public int Level()
        {
            int level = 0;
            Node<T> node = this;
            while (node != null && node.Parent != null)
            {
                node = node.Parent;
                level++;
            }

            return level;
        }
    }
}
