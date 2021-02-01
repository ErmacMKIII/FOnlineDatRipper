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
    /// Defines the <see cref="Tables" />.
    /// </summary>
    internal static class Tables
    {
        /// <summary>
        /// Mask for selecting table 1 entries..
        /// </summary>
        public const int Table1Mask = 0x1A;

        /// <summary>
        /// Mask for selecting table 2 entries..
        /// </summary>
        public const int Table2Mask = 0x7C;

        /// <summary>
        /// Mask for selecting table 3 entries..
        /// </summary>
        public const int Table3Mask = 0x78;

        /// <summary>
        /// Defines the table1.
        /// </summary>
        private static readonly byte[] table1 = new byte[27];

        /// <summary>
        /// Defines the table2.
        /// </summary>
        private static readonly short[] table2 = new short[125];

        /// <summary>
        /// Defines the table3.
        /// </summary>
        private static readonly byte[] table3 = new byte[121];

        /// <summary>
        /// Gets the Table1.
        /// </summary>
        public static byte[] Table1 => table1;

        /// <summary>
        /// Gets the Table2.
        /// </summary>
        public static short[] Table2 => table2;

        /// <summary>
        /// Gets the Table3.
        /// </summary>
        public static byte[] Table3 => table3;

        /// <summary>
        /// Initalizes the tables used by ACM Fillers; 
        /// Source: Libacm github by Marko Kreen.
        /// </summary>
        public static void Init()
        {
            int x3, x2, x1;
            for (x3 = 0; x3 < 3; x3++)
                for (x2 = 0; x2 < 3; x2++)
                    for (x1 = 0; x1 < 3; x1++)
                        table1[x1 + x2 * 3 + x3 * 3 * 3] =
                            (byte)(x1 + (x2 << 4) + (x3 << 8));

            for (x3 = 0; x3 < 5; x3++)
                for (x2 = 0; x2 < 5; x2++)
                    for (x1 = 0; x1 < 5; x1++)
                        table2[x1 + x2 * 5 + x3 * 5 * 5] =
                            (short)(x1 + (x2 << 4) + (x3 << 8));

            for (x2 = 0; x2 < 11; x2++)
                for (x1 = 0; x1 < 11; x1++)
                    table3[x1 + x2 * 11] = (byte)(x1 + (x2 << 4));
        }
    }
}
