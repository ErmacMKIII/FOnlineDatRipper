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
    using System.Drawing;
    using System.IO;

    /// <summary>
    /// Defines the <see cref="FRM" />.
    /// </summary>
    internal class FRM
    {
        /// <summary>
        /// Direction for the frames
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// Defines the NORTH_EAST.
            /// </summary>
            NORTH_EAST,
            /// <summary>
            /// Defines the EAST.
            /// </summary>
            EAST,
            /// <summary>
            /// Defines the SOUTH_EAST.
            /// </summary>
            SOUTH_EAST,
            /// <summary>
            /// Defines the SOUTH_WEST.
            /// </summary>
            SOUTH_WEST,
            /// <summary>
            /// Defines the WEST.
            /// </summary>
            WEST,
            /// <summary>
            /// Defines the NORTH_WEST.
            /// </summary>
            NORTH_WEST
        }

        /// <summary>
        /// Defines the version.
        /// </summary>
        private uint version;//  2-byte unsigned (0x0000)

        /// <summary>
        /// Defines the fps.
        /// </summary>
        private uint fps;// 2-byte unsigned (0x0004)

        /// <summary>
        /// Defines the actionFrame.
        /// </summary>
        private uint actionFrame;// 2-byte unsigned (0x0006)

        /// <summary>
        /// Defines the framesPerDirection.
        /// </summary>
        private uint framesPerDirection;// 2-byte unsigned (0x0008)

        /// <summary>
        /// Defines the shiftX.
        /// </summary>
        private int[] shiftX = new int[6];// signed

        /// <summary>
        /// Defines the shiftY.
        /// </summary>
        private int[] shiftY = new int[6];// signed

        /// <summary>
        /// Defines the offset.
        /// </summary>
        private readonly uint[] offset = new uint[6];// unsigned

        // image composed of frames (but frame 0 is primarily used)
        /// <summary>
        /// Defines the frames.
        /// </summary>
        private readonly List<Frame> frames = new List<Frame>();

        /// <summary>
        /// Defines the frameSize.
        /// </summary>
        private uint frameSize = 0;

        // 16 MB Buffer
        /// <summary>
        /// Defines the BufferSize.
        /// </summary>
        internal const int BufferSize = 0x1000000;

        /// <summary>
        /// Defines the buffer.
        /// </summary>
        private readonly byte[] buffer = new byte[BufferSize];

        /// <summary>
        /// Read only tag as a display name (on the tab for example).
        /// </summary>
        private readonly string tag;

        // position in the buffer for reading/writing operation
        // they're mostly big endian operations
        /// <summary>
        /// Defines the pos.
        /// </summary>
        private int pos = 0x0000;

        // maximum positon for read/write operations
        /// <summary>
        /// Defines the pos_max.
        /// </summary>
        private int pos_max = BufferSize - 1;

        /// <summary>
        /// Gets the Frames.
        /// </summary>
        internal List<Frame> Frames => frames;

        /// <summary>
        /// Gets or sets the FramesPerDirection.
        /// </summary>
        public uint FramesPerDirection { get => framesPerDirection; set => framesPerDirection = value; }

        /// <summary>
        /// Gets or sets the ActionFrame.
        /// </summary>
        public uint ActionFrame { get => actionFrame; set => actionFrame = value; }

        /// <summary>
        /// Gets or sets the Fps.
        /// </summary>
        public uint Fps { get => fps; set => fps = value; }

        /// <summary>
        /// Gets or sets the Version.
        /// </summary>
        public uint Version { get => version; set => version = value; }

        /// <summary>
        /// Gets the Tag.
        /// </summary>
        public string Tag => tag;

        /// <summary>
        /// Initializes a new instance of the <see cref="FRM"/> class.
        /// </summary>
        /// <param name="frmFile"> frm binary file.</param>
        public FRM(string frmFile)
        {
            int lastIndex = frmFile.LastIndexOf('\\');
            this.tag = frmFile.Substring(lastIndex + 1);
            ReadFile(frmFile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FRM"/> class.
        /// </summary>
        /// <param name="filename">The filename<see cref="string"/>.</param>
        /// <param name="bytes"> frm as byte array (unpacked) .</param>
        public FRM(string filename, byte[] bytes)
        {
            int lastIndex = filename.LastIndexOf('\\');
            this.tag = filename.Substring(lastIndex + 1);
            pos_max = bytes.Length;
            Array.Copy(bytes, buffer, bytes.Length);
            ReadBuffer();
        }

        /// <summary>
        /// Loads from the file into the memory.
        /// </summary>
        /// <param name="filename">.</param>
        private void LoadFromFile(string filename)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(filename)))
            {
                pos_max = br.Read(buffer, 0, BufferSize);
                br.Close();
            }
        }

        /// <summary>
        /// Reads frm file and create frames.
        /// </summary>
        /// <param name="frmFile">.</param>
        private void ReadFile(string frmFile)
        {
            buffer.Initialize();
            frames.Clear();
            if (File.Exists(frmFile))
            {
                LoadFromFile(frmFile);
            }
            else
            {
                return;
            }
            ReadBuffer();
        }

        /// <summary>
        /// Reads the buffer. Buffer needs to contain loaded bytes.
        /// </summary>
        private void ReadBuffer()
        {
            //----------------------------------------------------------------------
            pos = 0x0000;
            // big endian motorola
            version = (uint)(((buffer[pos] & 0xFF) << 24) | (((buffer[pos + 1] & 0xFF) << 16) | (((buffer[pos + 2]) & 0xFF) | (buffer[pos + 3] & 0xFF))));
            pos += 4;
            fps = (uint)(((buffer[pos] & 0xFF) << 8) | ((buffer[pos + 1] & 0xFF)));
            pos += 2;
            actionFrame = (uint)(((buffer[pos] & 0xFF) << 8) | ((buffer[pos + 1] & 0xFF)));
            pos += 2;
            FramesPerDirection = (uint)(((buffer[pos] & 0xFF) << 8) | ((buffer[pos + 1] & 0xFF)));
            pos += 2;
            //----------------------------------------------------------------------                
            for (uint i = 0; i < 6; i++)
            {
                shiftX[i] = (buffer[pos] << 8) | buffer[pos + 1];
                pos += 2;
            }

            for (uint i = 0; i < 6; i++)
            {
                shiftX[i] = (buffer[pos] << 8) | buffer[pos + 1];
                pos += 2;
            }
            //----------------------------------------------------------------------        
            for (uint i = 0; i < 6; i++)
            {
                offset[i] = (uint)(((buffer[pos] & 0xFF) << 24) | (((buffer[pos + 1] & 0xFF) << 16) | (((buffer[pos + 2]) & 0xFF) | (buffer[pos + 3] & 0xFF))));
                pos += 4;
            }
            //----------------------------------------------------------------------
            frameSize = (uint)(((buffer[pos] & 0xFF) << 24) | ((buffer[pos + 1] & 0xFF) << 16) | ((buffer[pos + 2] & 0xFF) << 8) | (buffer[pos + 3] & 0xFF));
            pos += 4;
            //----------------------------------------------------------------------
            uint total = 0;
            while (total < frameSize)
            {
                for (uint j = 0; j < FramesPerDirection; j++)
                {
                    uint width = (uint)(((buffer[pos] & 0xFF) << 8) | (buffer[pos + 1] & 0xFF));
                    pos += 2;
                    uint height = (uint)(((buffer[pos] & 0xFF) << 8) | (buffer[pos + 1] & 0xFF));
                    pos += 2;
                    //--------------------------------------------------------------
                    pos += 4;
                    //--------------------------------------------------------------
                    int offsetX = (buffer[pos] << 8) | (buffer[pos + 1]);
                    pos += 2;
                    int offsetY = (buffer[pos] << 8) | (buffer[pos + 1]);
                    pos += 2;
                    //--------------------------------------------------------------
                    Frame frame = new Frame(width, height, offsetX, offsetY);
                    for (uint py = 0; py < frame.Height; py++)
                    {
                        for (uint px = 0; px < frame.Width; px++)
                        {
                            byte index = buffer[pos++];
                            frame.SetPixel(px, py, index);
                        }
                    }
                    frames.Add(frame);
                    total += 12 + width * height;
                }
            }
        }

        /// <summary>
        /// Creates Bitmap array from frames.
        /// </summary>
        /// <returns>Bitmap array from frames.</returns>
        public Bitmap[] ToBitmapArray()
        {
            Bitmap[] result = new Bitmap[frames.Count];
            int index = 0;
            foreach (Frame frame in frames)
            {
                result[index++] = frame.ToBitmap();
            }
            return result;
        }

        /// <summary>
        /// Get starting frame for the direction.
        /// </summary>
        /// <param name="direction"> one of the possible six directions .</param>
        /// <returns>starting frame for the direction.</returns>
        public Frame GetFrame(Direction direction)
        {
            // casting enum and frames per Direction 
            int index = (int)direction / (int)framesPerDirection;

            // if such index valid
            if (index > 0 && index < frames.Count)
            {
                // return frame, otherwise null
                return frames[index];
            }
            return null;
        }

        /// <summary>
        /// Returns direction for the frame.
        /// </summary>
        /// <param name="frame">.</param>
        /// <returns>direction for the frame.</returns>
        public Direction GetDirection(Frame frame)
        {
            return (Direction)(frames.IndexOf(frame) / framesPerDirection);
        }
    }
}
