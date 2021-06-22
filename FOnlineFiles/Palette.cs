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
    using FOnlineDatRipper.Properties;
    using System.Drawing;
    using System.IO;

    /// <summary>
    /// Defines the <see cref="Palette" />.
    /// </summary>
    internal static class Palette
    {
        /// <summary>
        /// Defines the colors.
        /// </summary>
        private static readonly Color[] colors = new Color[256];

        /// <summary>
        /// Gets the Colors.
        /// </summary>
        public static Color[] Colors => colors;

        /// <summary>
        /// Defines the buffer.
        /// </summary>
        private static byte[] buffer = new byte[768];

        /// <summary>
        /// Initializes the palette with Fallout colors from resource file.
        /// </summary>
        public static void Init()
        {
            buffer.Initialize();
            using (BinaryReader br = new BinaryReader(new MemoryStream(Resources.Fallout_Palette)))
            {
                br.Read(buffer, 0, 768);
            }

            for (uint i = 0; i < buffer.Length / 3; i++)
            {
                colors[i] = Color.FromArgb(buffer[i * 3], buffer[i * 3 + 1], buffer[i * 3 + 2]);
            }
        }
    }
}
