#region copyright
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
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOnlineDatRipper
{
    static class Tables
    {
        private static readonly int[] table1 = new int[27];
        private static readonly int[] table2 = new int[125];
        private static readonly int[] table3 = new int[121];

        public static int[] Table1 => table1;

        public static int[] Table2 => table2;

        public static int[] Table3 => table3;

		/// <summary>
		/// Initalizes the table; Source: Libacm github by Marko Kreen		
		/// </summary>
		public static void Init() 
		{		
			int x3, x2, x1;
			for (x3 = 0; x3 < 3; x3++)
				for (x2 = 0; x2 < 3; x2++)
					for (x1 = 0; x1 < 3; x1++)
						table1[x1 + x2 * 3 + x3 * 3 * 3] =
							x1 + (x2 << 4) + (x3 << 8);
			for (x3 = 0; x3 < 5; x3++)
				for (x2 = 0; x2 < 5; x2++)
					for (x1 = 0; x1 < 5; x1++)
						table2[x1 + x2 * 5 + x3 * 5 * 5] =
							x1 + (x2 << 4) + (x3 << 8);
			for (x2 = 0; x2 < 11; x2++)
				for (x1 = 0; x1 < 11; x1++)
					table3[x1 + x2 * 11] = x1 + (x2 << 4);
		}
	}
}
