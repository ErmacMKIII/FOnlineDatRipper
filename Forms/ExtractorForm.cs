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

using FOnlineDatRipper.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace FOnlineDatRipper
{
    internal class ExtractorForm : Form
    {
        /// <summary>
        /// Defines the progress.
        /// </summary>
        private double progress = 0.0;

        /// <summary>
        /// Defines the backgroundWorker.
        /// </summary>
        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker();


        private readonly List<Dat> datList;

        private readonly Label lbl = new Label();

        private readonly ComboBox cmbBox = new ComboBox();

        private readonly Button btnExtract = new Button();
        
        private readonly ProgressBar progBar = new ProgressBar();

        public ExtractorForm(List<Dat> datList)
        {
            this.datList = datList;            
            Init();
            MainForm.InitDarkTheme(this);
        }

        /// <summary>
        /// The Init.
        /// </summary>
        private void Init()
        {
            this.Text = "Dat File(s) Extractor";
            this.Icon = Resources.app;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoSize = true;

            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;            

            // label
            this.lbl.Text = "Extract Archive:";
            this.lbl.Dock = DockStyle.Bottom;
            //this.Controls.Add(lbl);

            cmbBox.Dock = DockStyle.Bottom;
            // add each dat archive to the combo box
            foreach (Dat dat in datList)
            {
                cmbBox.Items.Add(dat.Tag);
            }
            this.Controls.Add(cmbBox);            

            // button Extract
            btnExtract.Dock = DockStyle.Bottom;
            btnExtract.Text = "Extract All...";
            btnExtract.Image = Resources.extract_all_icon;
            btnExtract.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnExtract.Click += BtnExtract_Click;
            //this.Controls.Add(btnExtract);

            TableLayoutPanel tblLayPnl = new TableLayoutPanel();
            tblLayPnl.AutoSize = true;
            tblLayPnl.ColumnCount = 2;
            tblLayPnl.RowCount = 2;
            tblLayPnl.Controls.Add(lbl);
            tblLayPnl.Controls.Add(cmbBox);
            tblLayPnl.Controls.Add(btnExtract);
            tblLayPnl.Dock = DockStyle.Fill;
            this.Controls.Add(tblLayPnl);

            // progress bar
            this.progBar.Dock = DockStyle.Bottom;
            this.Controls.Add(progBar);
        }

        private void BtnExtract_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
