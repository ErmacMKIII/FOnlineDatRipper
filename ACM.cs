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
    using NAudio.Wave;
    using System.IO;

    /// <summary>
    /// Defines the <see cref="ACM" />.
    /// </summary>
    internal class ACM
    {
        /// <summary>
        /// Represents decoded data (1 MB buffer)......
        /// </summary>
        private readonly byte[] content = new byte[0x100000];

        /// <summary>
        /// Read only tag as a display name (on the tab for example).......
        /// </summary>
        private readonly string tag;

        /// <summary>
        /// Gets the Tag
        /// Tag for this acm (for display)......
        /// </summary>
        public string Tag => tag;

        /// <summary>
        /// Gets the Content
        /// Decoded bytes......
        /// </summary>
        public byte[] Content => content;

        /// <summary>
        /// Content as Wave Stream......
        /// </summary>
        private RawSourceWaveStream waveStream;

        /// <summary>
        /// Gets the WaveStream.
        /// </summary>
        public RawSourceWaveStream WaveStream { get => waveStream; }

        /// <summary>
        /// Length of Content Buffer......
        /// </summary>
        private int length = 0;

        /// <summary>
        /// Gets the Length
        /// Length of Content Buffer......
        /// </summary>
        public int Length { get => length; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ACM"/> class.
        /// </summary>
        /// <param name="filename">.</param>
        /// <param name="bytes">.</param>
        public ACM(string filename, byte[] bytes)
        {
            this.tag = filename;
            this.Decode(bytes);
        }

        /// <summary>
        /// Decode packed bytes.
        /// </summary>
        /// <param name="acmBytes">.</param>
        private void Decode(byte[] acmBytes)
        {
            ACMDecoder acmDecoder = new ACMDecoder(acmBytes);
            this.length = acmDecoder.Decode(this.content);

            this.waveStream = new RawSourceWaveStream(
                new MemoryStream(this.content, 0, this.length),
                new WaveFormat((int)(acmDecoder.Info.Bitrate), 16, 2)
            );

            File.WriteAllBytes("thisIS", content);
        }
    }
}
