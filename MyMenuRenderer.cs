﻿// Copyright (C) 2021 Alexander Stojanovich
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
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="MyMenuRenderer" />.
    /// </summary>
    internal class MyMenuRenderer : ToolStripRenderer
    {
        /// <summary>
        /// The OnRenderItemText.
        /// </summary>
        /// <param name="e">The e<see cref="ToolStripItemTextRenderEventArgs"/>.</param>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.ToolStrip.BackColor = MainForm.DarkBackground;
            e.TextColor = MainForm.DarkForeground;
            base.OnRenderItemText(e);
        }
    }
}
