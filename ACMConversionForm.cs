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
    using NAudio.MediaFoundation;
    using NAudio.Wave;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="ACMConversionForm" />.
    /// </summary>
    internal class ACMConversionForm : Form
    {
        /// <summary>
        /// Defines the progress.
        /// </summary>
        private double progress = 0.0;

        /// <summary>
        /// Defines the backgroundWorker.
        /// </summary>
        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker();

        /// <summary>
        /// Defines the AudioFormat.
        /// </summary>
        public enum AudioFormat
        {
            /// <summary>
            /// Defines the AAC.
            /// </summary>
            AAC,
            /// <summary>
            /// Defines the MP3.
            /// </summary>
            MP3,
            /// <summary>
            /// Defines the WAV.
            /// </summary>
            WAV
        }

        /// <summary>
        /// Defines the audioFormat.
        /// </summary>
        private AudioFormat audioFormat = AudioFormat.WAV;

        /// <summary>
        /// Defines the chkListBox.
        /// </summary>
        private readonly CheckedListBox chkListBox = new CheckedListBox();

        /// <summary>
        /// Defines the lblOutputFormat.
        /// </summary>
        private readonly Label lblOutputFormat = new Label();

        /// <summary>
        /// Defines the cmbBoxOutputFormat.
        /// </summary>
        private readonly ComboBox cmbBoxOutputFormat = new ComboBox();

        /// <summary>
        /// Defines the btnGo.
        /// </summary>
        private readonly Button btnGo = new Button();

        /// <summary>
        /// Defines the progBar.
        /// </summary>
        private ProgressBar progBar = new ProgressBar();

        /// <summary>
        /// Defines the outDir.
        /// </summary>
        private string outDir;

        /// <summary>
        /// Defines the acms.
        /// </summary>
        private readonly List<ACM> acms;

        /// <summary>
        /// Initializes a new instance of the <see cref="ACMConversionForm"/> class.
        /// </summary>
        /// <param name="acms">The acms<see cref="List{ACM}"/>.</param>
        public ACMConversionForm(List<ACM> acms)
        {
            this.acms = acms;
            Init();
            InitDarkTheme(this);
        }

        /// <summary>
        /// The InitDarkTheme.
        /// </summary>
        /// <param name="root">The root<see cref="Control"/>.</param>
        private void InitDarkTheme(Control root)
        {
            root.BackColor = MainForm.DarkBackground;
            root.ForeColor = MainForm.DarkForeground;

            foreach (Control ctrl in root.Controls)
            {
                InitDarkTheme(ctrl);
            }
        }

        /// <summary>
        /// The Init.
        /// </summary>
        private void Init()
        {
            this.Text = "ACM File(s) Conversion";
            this.Icon = Resources.app;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoSize = true;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;

            chkListBox.Dock = DockStyle.Fill;
            // add each ACM to the list box
            foreach (ACM acm in acms)
            {
                chkListBox.Items.Add(acm.Tag);
            }
            this.Controls.Add(chkListBox);

            // label output format and combobox for output format
            this.lblOutputFormat.Text = "Output format:";
            this.lblOutputFormat.Dock = DockStyle.Bottom;
            this.Controls.Add(lblOutputFormat);
            this.cmbBoxOutputFormat.Dock = DockStyle.Bottom;
            this.cmbBoxOutputFormat.Items.Add(AudioFormat.AAC.ToString());
            this.cmbBoxOutputFormat.Items.Add(AudioFormat.MP3.ToString());
            this.cmbBoxOutputFormat.Items.Add(AudioFormat.WAV.ToString());
            this.cmbBoxOutputFormat.SelectedIndex = 0;
            this.Controls.Add(cmbBoxOutputFormat);

            // button GO
            btnGo.Dock = DockStyle.Bottom;
            btnGo.Text = "GO";
            btnGo.Click += BtnGo_Click;
            this.Controls.Add(btnGo);

            // progress bar
            this.progBar.Dock = DockStyle.Bottom;
            this.Controls.Add(progBar);
        }

        /// <summary>
        /// The BtnGo_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BtnGo_Click(object sender, EventArgs e)
        {
            // choose output directory
            using (FolderBrowserDialog openDirDialog = new FolderBrowserDialog())
            {
                // if user confirms dialog
                if (openDirDialog.ShowDialog() == DialogResult.OK)
                {
                    // perform encoding
                    this.outDir = openDirDialog.SelectedPath;
                    audioFormat = (AudioFormat)Enum.Parse(typeof(AudioFormat), cmbBoxOutputFormat.SelectedItem.ToString());

                    backgroundWorker.DoWork += BackgroundWorker_DoWork;
                    backgroundWorker.WorkerReportsProgress = true;
                    backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
                    backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

                    backgroundWorker.RunWorkerAsync();
                }

            }
        }

        /// <summary>
        /// The BackgroundWorker_RunWorkerCompleted.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RunWorkerCompletedEventArgs"/>.</param>
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("File conversion completed!", "ACM File(s) Conversion", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.progBar.Value = 0;
        }

        /// <summary>
        /// The BackgroundWorker_ProgressChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="ProgressChangedEventArgs"/>.</param>
        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// The BackgroundWorker_DoWork.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DoWorkEventArgs"/>.</param>
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            progress = 0.0;
            // loop through selected ACM items
            foreach (int index in chkListBox.CheckedIndices)
            {
                if (this.IsDisposed)
                {
                    break;
                }

                ACM acm = acms[index];
                string outFile = outDir + Path.DirectorySeparatorChar + acm.Tag + "." + audioFormat.ToString().ToLower();
                switch (audioFormat)
                {
                    case AudioFormat.AAC:
                        MediaFoundationApi.Startup();
                        MediaFoundationEncoder.EncodeToAac(acm.WaveStream, outFile);
                        MediaFoundationApi.Shutdown();
                        break;
                    case AudioFormat.MP3:
                        MediaFoundationApi.Startup();
                        MediaFoundationEncoder.EncodeToMp3(acm.WaveStream, outFile);
                        MediaFoundationApi.Shutdown();
                        break;
                    case AudioFormat.WAV:
                        WaveFileWriter.CreateWaveFile(outFile, acm.WaveStream);
                        break;
                }
                progress += 100.0 / (double)chkListBox.CheckedItems.Count;
                backgroundWorker.ReportProgress((int)progress);
            }
            progress = 100.0;
        }

        /// <summary>
        /// The Dispose.
        /// </summary>
        /// <param name="disposing">The disposing<see cref="bool"/>.</param>
        protected override void Dispose(bool disposing)
        {
        }
    }
}
