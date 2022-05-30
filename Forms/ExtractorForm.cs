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
using System.Diagnostics;
using System.Windows.Forms;

namespace FOnlineDatRipper
{
    internal class ExtractorForm : Form
    {
        /// <summary>
        /// Defines the backgroundWorker.
        /// </summary>
        private readonly BackgroundWorker extractor = new BackgroundWorker();


        private readonly List<Dat> datList;

        private readonly Label lblChoose = new Label();        

        private readonly ComboBox cmbBox = new ComboBox();

        private readonly Button btnExtract = new Button();

        private readonly Label lblFileProcessing = new Label();

        private readonly ProgressBar progBar = new ProgressBar();

        private readonly Stopwatch stopwatch = new Stopwatch();

        private double seconds = 0.0;

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
            this.Width = 400;
            this.Height = 300;

            this.Text = "Dat File(s) Extractor";
            this.Icon = Resources.app;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoSize = true;

            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;

            this.lblChoose.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblChoose.Text = "Choose Archive: ";

            this.lblChoose.Dock = DockStyle.Bottom;
            this.Controls.Add(lblChoose);

            // add each dat archive to the combo box
            foreach (Dat dat in datList)
            {
                cmbBox.Items.Add(dat.Tag);
            }
            this.cmbBox.Dock = DockStyle.Bottom;
            this.Controls.Add(cmbBox);


            // button Extract
            this.btnExtract.Dock = DockStyle.Bottom;
            this.btnExtract.Text = "EXTRACT...";
            this.btnExtract.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExtract.Image = Resources.extract_all_icon;
            this.btnExtract.Click += BtnExtract_Click;
            this.Controls.Add(btnExtract);

            // file proc label
            this.lblFileProcessing.Text = string.Empty;
            this.Controls.Add(lblFileProcessing);

            // progress bar
            this.progBar.Dock = DockStyle.Bottom;
            this.Controls.Add(progBar);

            // Extractor
            extractor.DoWork += Extractor_DoWork;
            extractor.RunWorkerCompleted += Extractor_RunWorkerCompleted;
        }

        private void BtnExtract_Click(object sender, EventArgs e)
        {
            var currDatFile = this.datList.Find(m => m.GetTag().Equals(cmbBox.SelectedItem));
            using (FolderBrowserDialog openDirDialog = new FolderBrowserDialog())
            {
                if (openDirDialog.ShowDialog() == DialogResult.OK)
                {
                    var outDir = openDirDialog.SelectedPath;
                    var kvp = new KeyValuePair<Dat, string>(currDatFile, outDir);
                    extractor.RunWorkerAsync(kvp);
                }
            }
        }

        /// <summary>
        /// The Extractor_DoWork.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DoWorkEventArgs"/>.</param>
        private void Extractor_DoWork(object sender, DoWorkEventArgs e)
        {
            // fetch key-value pair argument
            KeyValuePair<Dat, string> kvp = (KeyValuePair<Dat, string>)e.Argument;
            // extract only archives                        
            DatDoExtractAll(kvp.Key, kvp.Value);
        }

        /// <summary>
        /// The MiniExtractor_RunWorkerCompleted.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RunWorkerCompletedEventArgs"/>.</param>
        private void Extractor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show($"Dat File(s) sucessfully extracted in {seconds} seconds!", "Extracting File(s)", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.progBar.Value = 0;
        }

        /// <summary>
        /// Extract all from the dat.
        /// Called from Background worker.
        /// </summary>
        /// <param name="dat">The dat<see cref="Dat"/>.</param>
        private void DatDoExtractAll(Dat dat, string outDir)
        {
            // sub to the event(s)
            dat.OnProgressUpdate += Dat_OnProgressUpdate;

            // 2nd event (recently added)
            dat.OnFileNameProcessing += Dat_OnFileNameProcessing;

            // start measuring the time
            stopwatch.Start();

            // call dat to extract all
            dat.ExtractAll(outDir);

            // stop measuring the time
            stopwatch.Stop();

            // set displayed elapsed time (in the message), measured in seconds
            seconds = stopwatch.ElapsedMilliseconds / 1000.0;

            // reset for another read or any op
            stopwatch.Reset();
        }

        private void Dat_OnFileNameProcessing(string fileName)
        {
            // its in another thread so invoke back to UI thread
            base.Invoke((Action)delegate
            {
                // fonline file index
                this.lblFileProcessing.Text = fileName;
            });
        }

        private void Dat_OnProgressUpdate(double progress)
        {            
            // its in another thread so invoke back to UI thread
            base.Invoke((Action)delegate
            {
                // fonline file index
                this.progBar.Value = (int)Math.Round(progress);
            });
        }
    }
}
