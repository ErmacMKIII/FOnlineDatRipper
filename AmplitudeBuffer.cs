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
    /// Defines the <see cref="AmplitudeBuffer" />.
    /// </summary>
    internal static class AmplitudeBuffer
    {
        /// <summary>
        /// Defines the amplitureBuffer.
        /// </summary>
        private static readonly short[] amplitureBuffer = new short[0x10000];

        /// <summary>
        /// Gets the AmplitureBuffer.
        /// </summary>
        public static short[] AmplitureBuffer => amplitureBuffer;

        /// <summary>
        /// Get The Amplitude Middle.
        /// </summary>
        /// <param name="index">The index<see cref="int"/>.</param>
        /// <returns>The <see cref="short"/>.</returns>
        public static short Middle(int index)
        {
            int amplIndex = index + 0x8000;
            return amplitureBuffer[amplIndex];
        }

        /// <summary>
        /// Set The Amplitude Middle.
        /// </summary>
        /// <param name="index">The index<see cref="int"/>.</param>
        /// <param name="val">The val<see cref="short"/>.</param>
        public static void Middle(int index, short val)
        {
            int amplIndex = index + 0x8000;
            amplitureBuffer[amplIndex] = val;
        }
    }
}
