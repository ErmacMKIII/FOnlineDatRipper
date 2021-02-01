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
    using NAudio.Wave;
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="ACMForm" />.
    /// </summary>
    internal class ACMForm : Form
    {
        /// <summary>
        /// acm sound files array which is equal to the number of tabs......
        /// </summary>
        private readonly List<ACM> acms;

        /// <summary>
        /// button for playing current track or music file.....
        /// </summary>
        private readonly Button btnPlay = new Button();

        /// <summary>
        /// button for pausing current track or music file.....
        /// </summary>
        private readonly Button btnPause = new Button();

        /// <summary>
        /// button to reset current track or music file.....
        /// </summary>
        private readonly Button btnStop = new Button();

        /// <summary>
        /// list box contains tracks.....
        /// </summary>
        private readonly ListBox listBox = new ListBox();

        /// <summary>
        /// tab control with selected tab......
        /// </summary>
        private readonly TabControl tabControl = new TabControl();

        /// <summary>
        /// Wave sound player.....
        /// </summary>
        private readonly WaveOutEvent wo = new WaveOutEvent();

        /// <summary>
        /// Initializes a new instance of the <see cref="ACMForm"/> class.
        /// </summary>
        /// <param name="acms">The acms<see cref="List{ACM}"/>.</param>
        public ACMForm(List<ACM> acms)
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
        /// Defines initalization of this subform.
        /// </summary>
        private void Init()
        {
            this.Text = "ACM File(s) Preview";
            this.Icon = Resources.app;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoSize = true;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;

            listBox.Dock = DockStyle.Fill;
            // add each ACM to the list box
            foreach (ACM acm in acms)
            {
                listBox.Items.Add(acm.Tag);
            }
            this.Controls.Add(listBox);

            // next frame button
            btnPlay.Dock = DockStyle.Bottom;
            btnPlay.Text = "Play";
            btnPlay.Click += BtnPlay_Click;
            this.Controls.Add(btnPlay);

            // prev frame button
            btnPause.Dock = DockStyle.Bottom;
            btnPause.Text = "Pause";
            btnPause.Click += BtnPause_Click;
            this.Controls.Add(btnPause);

            // reset index button
            btnStop.Dock = DockStyle.Bottom;
            btnStop.Text = "Stop";
            btnStop.Click += BtnStop_Click;
            this.Controls.Add(btnStop);
        }

        /// <summary>
        /// The BtnStop_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BtnStop_Click(object sender, EventArgs e)
        {
            wo.Stop();
        }

        /// <summary>
        /// The BtnPause_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BtnPause_Click(object sender, EventArgs e)
        {
            wo.Pause();
        }

        /// <summary>
        /// The BtnPlay_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BtnPlay_Click(object sender, EventArgs e)
        {
            int index = listBox.SelectedIndex;
            if (index != -1)
            {
                if (wo.PlaybackState != PlaybackState.Playing)
                {
                    wo.Init(acms[index].WaveStream);
                    wo.Play();
                }
            }
        }
    }
}
