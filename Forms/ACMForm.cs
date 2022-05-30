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
    using NAudio.Gui;
    using NAudio.Wave;
    using NAudio.WaveFormRenderer;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="ACMForm" />.
    /// </summary>
    internal class ACMForm : Form
    {
        /// <summary>
        /// acm sound files array which is equal to the number of tabs.............
        /// </summary>
        private readonly List<ACM> acms;

        /// <summary>
        /// button for playing current track or music file............
        /// </summary>
        private readonly Button btnPlay = new Button();

        /// <summary>
        /// button for pausing current track or music file............
        /// </summary>
        private readonly Button btnPause = new Button();

        /// <summary>
        /// button to reset current track or music file............
        /// </summary>
        private readonly Button btnStop = new Button();

        /// <summary>
        /// list box contains tracks............
        /// </summary>
        private readonly ListBox listBox = new ListBox();

        /// <summary>
        /// Wave sound player............
        /// </summary>
        private readonly WaveOutEvent wo = new WaveOutEvent();

        /// <summary>
        /// Visualizer
        /// </summary>
        private readonly WaveFormRenderer waveFormRenderer = new WaveFormRenderer();
         
        /// <summary>
        /// Picture Box containing the waveform
        /// </summary>
        private readonly PictureBox picBox = new PictureBox();        

        /// <summary>
        /// Initializes a new instance of the <see cref="ACMForm"/> class.
        /// </summary>
        /// <param name="acms">The acms<see cref="List{ACM}"/>.</param>
        public ACMForm(List<ACM> acms)
        {
            this.acms = acms;
            Init();
            MainForm.InitDarkTheme(this);
        }       

        /// <summary>
        /// Defines initalization of this subform.
        /// </summary>
        private void Init()
        {
            this.Width = 400;
            this.Height = 300;

            this.Text = "ACM File(s) Preview";
            this.Icon = Resources.app;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoSize = true;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;

            listBox.Dock = DockStyle.Top;
            this.listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;

            // add each ACM to the list box
            foreach (ACM acm in acms)
            {
                listBox.Items.Add(acm.Tag);
            }
            this.Controls.Add(listBox);

            // add pic box to display waveform
            this.picBox.Dock = DockStyle.Fill;
            this.picBox.Paint += PicBox_Paint;
            this.Controls.Add(picBox);

            // next frame button
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.Dock = DockStyle.Bottom;
            btnPlay.Text = "Play";
            btnPlay.Click += BtnPlay_Click;
            this.Controls.Add(btnPlay);

            // prev frame button
            btnPause.FlatStyle = FlatStyle.Flat;
            btnPause.Dock = DockStyle.Bottom;
            btnPause.Text = "Pause";
            btnPause.Click += BtnPause_Click;
            this.Controls.Add(btnPause);

            // reset index button
            btnStop.FlatStyle = FlatStyle.Flat;
            btnStop.Dock = DockStyle.Bottom;
            btnStop.Text = "Stop";
            btnStop.Click += BtnStop_Click;
            this.Controls.Add(btnStop);
        }

        private void PicBox_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.Transparent);

            var settings = new SoundCloudBlockWaveFormSettings(Color.Orange, Color.DarkOrange, Color.Magenta, Color.DarkMagenta);
            
            settings.BackgroundColor = MainForm.DefaultBackColor;
            settings.Width = picBox.Width >> 1;
            settings.TopHeight = picBox.Height >> 2;
            settings.BottomHeight = picBox.Height >> 2;

            // render the wave form the field image
            int index = listBox.SelectedIndex;
            if (index != -1)
            {
                ACM acm = acms[index];
                var waveFormImg = waveFormRenderer.Render(
                    acm.WaveStream, settings
                );
                
                g.DrawImage(waveFormImg, Rectangle.FromLTRB(0, picBox.Height / 2, picBox.Width, picBox.Height));
            }                       
        }

        /// <summary>
        /// The ListBox_SelectedIndexChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = listBox.SelectedIndex;
            if (index != -1)
            {
                if (wo.PlaybackState != PlaybackState.Stopped)
                {
                    wo.Stop();
                }
                ACM acm = acms[index];
                if (acm.WaveStream != null && wo.PlaybackState == PlaybackState.Stopped)
                {
                    // invalidate so the sound wave form is being displayed
                    picBox.Invalidate();

                    // initialize the stream for playing
                    acm.WaveStream.Position = 0;
                    wo.Init(acm.WaveStream); 
                }
            } 
        }

        /// <summary>
        /// The BtnStop_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BtnStop_Click(object sender, EventArgs e)
        {
            if (wo.PlaybackState != PlaybackState.Stopped)
            {
                wo.Stop();
            }
        }

        /// <summary>
        /// The BtnPause_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BtnPause_Click(object sender, EventArgs e)
        {
            if (wo.PlaybackState != PlaybackState.Paused)
            {
                wo.Pause();
            }
        }

        /// <summary>
        /// The BtnPlay_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BtnPlay_Click(object sender, EventArgs e)
        {
            if (wo.PlaybackState != PlaybackState.Paused)
            {
                wo.Stop();
            }

            int index = listBox.SelectedIndex;
            if (index != -1)
            {
                ACM acm = acms[index];                
                if (acm.WaveStream != null
                    && acm.WaveStream.Position == acm.WaveStream.Length)
                {
                    acm.WaveStream.Position = 0;
                }

                if (wo.PlaybackState != PlaybackState.Playing)
                {                                       
                    wo.Play();
                }
            }
        }       

        /// <summary>
        /// The Dispose.
        /// </summary>
        /// <param name="disposing">The disposing<see cref="bool"/>.</param>
        protected override void Dispose(bool disposing)
        {
            wo.Dispose();
        }
    }
}
