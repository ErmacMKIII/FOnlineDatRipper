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
    using System.IO;
    using System.Text;
    using zlib;

    /// <summary>
    /// Defines the <see cref="Dat" />.
    /// </summary>
    internal class Dat
    {
        /// <summary>
        /// Defines the BufferSize.
        /// </summary>
        public const int BufferSize = 256 * 1024 * 1024;

        /// <summary>
        /// Defines the buffer.
        /// </summary>
        private readonly byte[] buffer = new byte[BufferSize];

        /// <summary>
        /// Defines the datSize.
        /// </summary>
        private int datSize = 0;

        /// <summary>
        /// Defines the dataBlocks.
        /// </summary>
        private readonly List<DataBlock> dataBlocks = new List<DataBlock>();

        /// <summary>
        /// Gets the DataBlocks.
        /// </summary>
        internal List<DataBlock> DataBlocks => dataBlocks;

        /// <summary>
        /// Defines the tree.
        /// </summary>
        private readonly Tree<string> tree = new Tree<string>(new Node<string>("root"));

        /// <summary>
        /// Defines the scope for nodes....
        /// </summary>
        private readonly List<List<Node<string>>> scope = new List<List<Node<string>>>();

        /// <summary>
        /// Gets the Tree.
        /// </summary>
        internal Tree<string> Tree => tree;

        /// <summary>
        /// Gets the Scope.
        /// </summary>
        internal List<List<Node<string>>> Scope => scope;

        /// <summary>
        /// Defines the progress.
        /// </summary>
        private double progress = 0.0f;

        /// <summary>
        /// Gets or sets the Progress.
        /// </summary>
        public double Progress { get => progress; set => progress = value; }

        /// <summary>
        /// Loads from the file into the memory.
        /// </summary>
        /// <param name="filename"> filename binary.</param>
        private void LoadFromFile(String filename)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(filename)))
            {
                datSize = br.Read(buffer, 0, BufferSize);
                br.Close();
            }
        }

        /// <summary>
        /// Reads the binary data from the input file, store into the memory and creates data blocks.
        /// </summary>
        /// <param name="filepath">.</param>
        public void Read(string filepath)
        {
            progress = 0.0;

            // inits buffer with zeroes
            buffer.Initialize();
            // clears data blocks
            dataBlocks.Clear();

            // clear the tree (but keep the root)
            tree.Clear();

            // clear the scope
            scope.Clear();

            // if file exits read the bytes (load into the memory)
            if (File.Exists(filepath))
            {
                LoadFromFile(filepath);
            }
            // otherwise quit
            else
            {
                throw new Exception("Error - input filepath doesn't exist!");
            }

            // reading from the buffer (stored in the memory) ...
            using (BinaryReader br = new BinaryReader(new MemoryStream(buffer)))
            {
                br.BaseStream.Position = datSize - 4;
                // reading real dat size to check if it's ok
                uint realDatSize = br.ReadUInt32();

                // checking if size is ok
                if (realDatSize != datSize)
                {
                    throw new Exception("Error - real and dat differ in size!");
                }

                // reading tree size
                br.BaseStream.Position = datSize - 8;
                uint filesTreeSize = br.ReadUInt32();

                // reading file count
                br.BaseStream.Position = datSize - filesTreeSize - 8;
                uint fileCount = br.ReadUInt32();

                progress += 5.0;

                // iterating through the files (they can be compressed or uncompressed)
                for (uint i = 0; i < fileCount; i++)
                {
                    // reading filename
                    uint filenameSize = br.ReadUInt32();
                    byte[] filenameBytes = br.ReadBytes((int)filenameSize);
                    string filename = Encoding.UTF8.GetString(filenameBytes);

                    // reading of compressed
                    bool compressed = (br.ReadByte() == 0x01);

                    // reading real (uncompressed) and compressed size, respectively
                    uint realSize = br.ReadUInt32();
                    uint packedSize = br.ReadUInt32();

                    // reading data offset
                    uint offset = br.ReadUInt32();

                    // make new data block & add it to the data blocklist
                    DataBlock dataBlock = new DataBlock(filename, compressed, realSize, packedSize, offset);
                    dataBlocks.Add(dataBlock);

                    progress += 95.0 / (double)fileCount;
                }

                // closing the stream
                br.Close();
            }

            progress = 100.0;
        }

        /// <summary>
        /// Gets data as byte array of given data block.
        /// </summary>
        /// <param name="dataBlock">.</param>
        /// <returns>.</returns>
        public byte[] Data(DataBlock dataBlock)
        {
            // if data block is compressed
            if (dataBlock.Compressed)
            {
                // src array (compressed), copied from buffer at data block offset
                byte[] src = new byte[dataBlock.PackedSize];
                Array.Copy(buffer, dataBlock.Offset, src, 0, src.Length);

                // dst array (uncompressed)
                byte[] dst = new byte[dataBlock.RealSize];
                // involve 3rd part lib for extracting (speed is preferred)
                using (ZOutputStream zos = new ZOutputStream(new MemoryStream(src), zlibConst.Z_BEST_SPEED))
                {
                    // write to the dst array and finish
                    zos.Write(dst, 0, dst.Length);
                    zos.finish();
                }

                return dst;
            }
            // otherwise if not compressed 
            else
            {
                // just copy from the buffer to the dst
                byte[] data = new byte[dataBlock.RealSize];
                Array.Copy(buffer, dataBlock.Offset, data, 0, data.Length);

                return data;
            }
        }

        /// <summary>
        /// Extracts chosen datablock to the new directory which is outdir.
        /// </summary>
        /// <param name="outdir">output directory path.</param>
        /// <param name="dataBlock">specific data block for extraction .</param>
        public void Extract(string outdir, DataBlock dataBlock)
        {
            using (BinaryWriter bw = new BinaryWriter(new MemoryStream(Data(dataBlock))))
            {
                // last index of backslash separates dir entries from sole filename 
                int lastIndex = dataBlock.Filename.LastIndexOf("\\");
                string dir = dataBlock.Filename.Substring(0, lastIndex + 1);
                string path = outdir + "\\" + dataBlock.Filename;

                // create directories if not exist (optional operation)
                Directory.CreateDirectory(outdir + "\\" + dir);
                if (File.Exists(path))
                {
                    // deleting is not necessary since next statement with writing all bytes overwrites file
                    File.Delete(path);
                }

                // and write all the bytes from the data block (it's extracted if necessary)
                File.WriteAllBytes(path, Data(dataBlock));
            }
        }

        /// <summary>
        /// Build this tree recursively. Parsed index is zero.
        /// </summary>
        /// <param name="pathParts"> path parts.</param>
        private void BuildTree(string[] pathParts)
        {
            int index = 1; // root already exists (with index 0)
            Node<string> currNode = this.tree.Root;
            foreach (string part in pathParts)
            {
                // define predicate
                Predicate<Node<string>> predicate = new Predicate<Node<string>>
                (
                    t =>
                    {
                        return t.Data.Equals(part);
                    }
                );

                // try to find the the node from the same scope (QUICK SOLUTION)                
                Node<string> node = scope[index - 1].Find(predicate);

                // if node doesn't exist
                if (node == null)
                {
                    // create new one
                    node = new Node<string>(part);
                    // position curr node to it's parent
                    while (currNode.Level() > index)
                    {
                        currNode = currNode.Parent;
                    }
                    // add to the parent (currNode)
                    node.Parent = currNode;
                    currNode.Children.Add(node);
                    // if scope doesn't exist create new one
                    if (scope.Count >= index)
                    {
                        scope.Add(new List<Node<string>>());
                    }
                    // add to the scope (for QUICK SEARCH)
                    scope[index - 1].Add(node);
                }

                currNode = node;
                index++;
            }
        }

        /// <summary>
        /// Builds dat tree structure from the data blocks.
        /// If dat has not been read tree won't be built.
        /// </summary>
        public void BuildTreeStruct()
        {
            progress = 0.0;
            // add the root to the zero-level scope
            scope.Add(new List<Node<string>>());
            scope[0].Add(tree.Root);

            foreach (DataBlock dataBlock in dataBlocks)
            {
                BuildTree(dataBlock.Filename.Split('\\'));
                progress += 100.0 / dataBlocks.Count;
            }

            progress = 100.0;
        }
    }
}
