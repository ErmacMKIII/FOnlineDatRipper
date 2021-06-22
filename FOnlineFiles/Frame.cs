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
    using System.Drawing;
    using System.Drawing.Imaging;

    /// <summary>
    /// Defines the <see cref="Frame" />.
    /// </summary>
    internal class Frame
    {
        /// <summary>
        /// Defines the width.
        /// </summary>
        private readonly uint width;

        /// <summary>
        /// Defines the height.
        /// </summary>
        private readonly uint height;

        /// <summary>
        /// Defines the offsetX.
        /// </summary>
        private readonly int offsetX;

        /// <summary>
        /// Defines the offsetY.
        /// </summary>
        private readonly int offsetY;

        /// <summary>
        /// Defines the data.
        /// </summary>
        private readonly byte[] data;

        /// <summary>
        /// Gets the Width.
        /// </summary>
        public uint Width => width;

        /// <summary>
        /// Gets the Height.
        /// </summary>
        public uint Height => height;

        /// <summary>
        /// Gets the OffsetX.
        /// </summary>
        public int OffsetX => offsetX;

        /// <summary>
        /// Gets the OffsetY.
        /// </summary>
        public int OffsetY => offsetY;

        /// <summary>
        /// Gets the Data.
        /// </summary>
        public byte[] Data => data;

        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class.
        /// </summary>
        /// <param name="width">The width<see cref="uint"/>.</param>
        /// <param name="height">The height<see cref="uint"/>.</param>
        /// <param name="offsetX">The offsetX<see cref="int"/>.</param>
        /// <param name="offsetY">The offsetY<see cref="int"/>.</param>
        public Frame(uint width, uint height, int offsetX, int offsetY)
        {
            this.width = width;
            this.height = height;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.data = new byte[width * height];
        }

        /// <summary>
        /// Gets pixel color (palette entry).
        /// </summary>
        /// <param name="px">x coord.</param>
        /// <param name="py">y coord.</param>
        /// <returns>.</returns>
        public byte GetPixel(uint px, uint py)
        {
            uint e = width * py + px;
            return data[e];
        }

        /// <summary>
        /// Sets pixel color to one from the palette (to the entry).
        /// </summary>
        /// <param name="px">x coord.</param>
        /// <param name="py">y coord.</param>
        /// <param name="val">palette index to set.</param>
        public void SetPixel(uint px, uint py, byte val)
        {
            uint e = width * py + px;
            data[e] = val;
        }

        /// <summary>
        /// Converts this frame to bitmap, used later to drawing on controls.
        /// </summary>
        /// <returns>.</returns>
        public Bitmap ToBitmap()
        {
            Bitmap result = new Bitmap((int)width, (int)height, PixelFormat.Format32bppArgb);
            for (uint px = 0; px < width; px++)
            {
                for (uint py = 0; py < height; py++)
                {
                    uint e = width * py + px;
                    result.SetPixel((int)px, (int)py, Palette.Colors[data[e]]);
                }
            }
            // make the blue transparent
            result.MakeTransparent(Palette.Colors[0]);
            return result;
        }
    }
}
