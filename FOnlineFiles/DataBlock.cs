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
    /// <summary>
    /// Defines the <see cref="DataBlock" />.
    /// </summary>
    internal class DataBlock
    {
        /// <summary>
        /// Defines the filename.
        /// </summary>
        private readonly string filename;

        /// <summary>
        /// Defines the compressed.
        /// </summary>
        private readonly bool compressed;

        /// <summary>
        /// Defines the realSize.
        /// </summary>
        private readonly uint realSize;

        /// <summary>
        /// Defines the packedSize.
        /// </summary>
        private readonly uint packedSize;

        /// <summary>
        /// Defines the offset.
        /// </summary>
        private readonly uint offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBlock"/> class.
        /// </summary>
        /// <param name="filename"> filename.</param>
        /// <param name="compressed">compressed flag.</param>
        /// <param name="realSize">size if not cmopressed.</param>
        /// <param name="packedSize">size if compressed.</param>
        /// <param name="offset">offset of the file in the buffer.</param>
        public DataBlock(string filename, bool compressed, uint realSize, uint packedSize, uint offset)
        {
            this.filename = filename;
            this.compressed = compressed;
            this.realSize = realSize;
            this.packedSize = packedSize;
            this.offset = offset;
        }

        /// <summary>
        /// Gets the Filename.
        /// </summary>
        public string Filename => filename;

        /// <summary>
        /// Gets a value indicating whether Compressed.
        /// </summary>
        public bool Compressed => compressed;

        /// <summary>
        /// Gets the RealSize.
        /// </summary>
        public uint RealSize => realSize;

        /// <summary>
        /// Gets the PackedSize.
        /// </summary>
        public uint PackedSize => packedSize;

        /// <summary>
        /// Gets the Offset.
        /// </summary>
        public uint Offset => offset;
    }
}
