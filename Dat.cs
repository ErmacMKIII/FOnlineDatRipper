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
    using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="Dat" />.
    /// </summary>
    internal class Dat : FOnlineFile
    {
        /// <summary>
        /// The ProgressUpdate.
        /// </summary>
        /// <param name="progress">The value<see cref="double"/>.</param>
        public delegate void ProgressUpdate(double progress);

        /// <summary>
        /// Defines the OnProgressUpdate.
        /// </summary>
        public event ProgressUpdate OnProgressUpdate;

        /// <summary>
        /// Defines the BufferSize.
        /// </summary>
        public const int BufferSize = 0x10000000;// 256 MB Buffer

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
        /// Defines the scope for nodes..............
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
        private double progress = 0.0;

        /// <summary>
        /// Gets the Progress
        /// Gets or sets the Progress....
        /// </summary>
        public double Progress { get => progress; }

        /// <summary>
        /// Is error occurred?..........
        /// </summary>
        private bool error = false;

        /// <summary>
        /// Error message to display..........
        /// </summary>
        private string errorMessage = "";

        /// <summary>
        /// Gets a value indicating whether Error.
        /// </summary>
        public bool Error { get => error; }

        /// <summary>
        /// Gets the ErrorMessage.
        /// </summary>
        public string ErrorMessage { get => errorMessage; }

        /// <summary>
        /// Tag used to associtate this (e.g. filename)...
        /// </summary>
        private readonly string tag;

        /// <summary>
        /// Gets the Tag
        /// Tag used to associtate this (e.g. filename)...
        /// </summary>
        public string Tag { get => tag; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dat"/> class.
        /// </summary>
        /// <param name="filepath">The filepath<see cref="string"/>.</param>
        public Dat(string filepath)
        {
            int lastIndex = filepath.LastIndexOf('\\');
            this.tag = filepath.Substring(lastIndex + 1);
        }

        /// <summary>
        /// Loads from the file into the memory.
        /// </summary>
        /// <param name="filename"> filename binary.</param>
        private void LoadFromFile(string filename)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(filename)))
            {
                datSize = br.Read(this.buffer, 0, BufferSize);
                br.Close();
            }
        }

        /// <summary>
        /// Reads the binary data from the input file, store into the memory and creates data blocks.
        /// </summary>
        /// <param name="filepath">The filepath<see cref="string"/>.</param>
        public override void ReadFile(string filepath)
        {
            progress = 0.0;

            this.tree.Root.Data = tag;

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
                error = true;
                errorMessage = "Error - input filepath doesn't exist!";
                progress = 100.0;
                return;
            }

            // reading from the buffer (stored in the memory) ...
            using (BinaryReader br = new BinaryReader(new MemoryStream(buffer, 0, datSize)))
            {
                br.BaseStream.Position = datSize - 4;
                // reading real dat size to check if it's ok
                uint realDatSize = br.ReadUInt32();

                // checking if size is ok
                if (realDatSize != datSize)
                {
                    error = true;
                    errorMessage = "Error - real and dat differ in size!";
                    progress = 100.0;
                    return;
                }

                // reading tree size
                br.BaseStream.Position = datSize - 8;
                uint filesTreeSize = br.ReadUInt32();

                // reading file count
                br.BaseStream.Position = datSize - filesTreeSize - 8;
                uint fileCount = br.ReadUInt32();

                progress += 5.0;
                if (OnProgressUpdate != null)
                {
                    OnProgressUpdate(progress);
                }

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
                    if (OnProgressUpdate != null)
                    {
                        OnProgressUpdate(progress);
                    }
                }

                if (br.BaseStream.Position > datSize)
                {
                    error = true;
                    errorMessage = "Error - reading position going over the max dat archive size!";
                }

                // closing the stream (although unecessary)
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
                // data array (uncompressed)
                byte[] data = new byte[dataBlock.RealSize];

                // involve ICSharpCode.SharpZipLib for extracting (RECOMMENDED)
                using (InflaterInputStream decompressStream = new InflaterInputStream(new MemoryStream(buffer, (int)dataBlock.Offset, (int)dataBlock.PackedSize)))
                {
                    decompressStream.Read(data, 0, (int)dataBlock.RealSize);
                }

                return data;
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
        /// Extract all the files to the output directory.
        /// </summary>
        /// <param name="outdir">Output directory.</param>
        public void ExtractAll(string outdir)
        {
            progress = 0.0;
            foreach (DataBlock dataBlock in dataBlocks)
            {
                Extract(outdir, dataBlock);
                progress += 100.0 / (double)dataBlocks.Count;
                if (OnProgressUpdate != null)
                {
                    OnProgressUpdate(progress);
                }
            }
            progress = 100.0;
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
                progress += 100.0 / (double)dataBlocks.Count;
                if (OnProgressUpdate != null)
                {
                    OnProgressUpdate(progress);
                }
            }

            progress = 100.0;
        }

        /// <summary>
        /// Gets data block from string building filename of dat node.
        /// </summary>
        /// <param name="datNode">.</param>
        /// <returns>.</returns>
        public DataBlock GetDataBlock(Node<string> datNode)
        {
            string fileNameKey = GetAlternativeFileName(datNode);
            Predicate<DataBlock> predicate = new Predicate<DataBlock>(db => { return db.Filename.Equals(fileNameKey); });
            return dataBlocks.Find(predicate);
        }

        /// <summary>
        /// Gets full filename from the dat archive file.
        /// </summary>
        /// <param name="datNode">.</param>
        /// <returns>full filename.</returns>
        public static string GetFileName(Node<string> datNode)
        {
            // create path list in reverse order
            Node<string> inode = datNode;
            List<string> pathList = new List<string>();
            while (inode != null)
            {
                pathList.Add(inode.Data);
                inode = inode.Parent;
            }
            pathList.Reverse();

            // build string from the list -> txtBoxPathInfo
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (string part in pathList)
            {
                sb.Append(part);
                if (index != pathList.Count - 1)
                {
                    sb.Append('/');
                }
                index++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets alternative filename from the dat archive file for data block search.
        /// </summary>
        /// <param name="datNode">.</param>
        /// <returns>exact datblock filename.</returns>
        public string GetAlternativeFileName(Node<string> datNode)
        {
            // create path list in reverse order
            Node<string> inode = datNode;
            List<string> pathList = new List<string>();
            while (inode != null && inode != Tree.Root)
            {
                pathList.Add(inode.Data);
                inode = inode.Parent;
            }
            pathList.Reverse();

            // build string from the list -> txtBoxPathInfo
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (string part in pathList)
            {
                sb.Append(part);
                if (index != pathList.Count - 1)
                {
                    sb.Append('\\');
                }
                index++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the tag (filename).
        /// </summary>
        /// <returns>.</returns>
        public override string GetTag()
        {
            return Tag;
        }

        /// <summary>
        /// Tells if error ocurred.
        /// </summary>
        /// <returns>.</returns>
        public override bool IsError()
        {
            return error;
        }

        /// <summary>
        /// Shows the error message.
        /// </summary>
        /// <returns>.</returns>
        public override string GetErrorMessage()
        {
            return errorMessage;
        }

        /// <summary>
        /// The GetProgress.
        /// </summary>
        /// <returns>The <see cref="double"/>.</returns>
        public override double GetProgress()
        {
            return progress;
        }
    }
}
