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
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="FRMForm" />.
    /// </summary>
    internal class FRMForm : Form
    {
        /// <summary>
        /// frame array which is equal to the number of tabs........
        /// </summary>
        private readonly List<FRM> frms;

        /// <summary>
        /// index of frame array to display frame........
        /// </summary>
        private int frmDisplayIndex = 0;

        /// <summary>
        /// tab control with selected tab........
        /// </summary>
        private readonly TabControl tabControl = new TabControl();

        /// <summary>
        /// Buttons for choosing the next frame........
        /// </summary>
        private readonly Button btnNextFrame = new Button();

        /// <summary>
        /// Buttons for choosing the previous frame........
        /// </summary>
        private readonly Button btnPrevFrame = new Button();

        /// <summary>
        /// Resets display index to 0........
        /// </summary>
        private readonly Button btnReset = new Button();

        /// <summary>
        /// Initializes a new instance of the <see cref="FRMForm"/> class.
        /// </summary>
        /// <param name="fRMs">The fRMs<see cref="FRM[]"/>.</param>
        public FRMForm(List<FRM> fRMs)
        {
            frms = fRMs;
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
            frmDisplayIndex = 0;
            this.Text = "FRM File(s) Preview";
            this.Icon = Resources.app;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoSize = true;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;

            tabControl.Dock = DockStyle.Fill;
            // for parsed FRMs make tab for each
            foreach (FRM frm in frms)
            {
                TabPage tabPage = new TabPage(frm.Tag);
                Panel pnlImg = new Panel();
                pnlImg.Dock = DockStyle.Fill;
                pnlImg.AutoScroll = true;

                PictureBox picBox = new PictureBox();
                picBox.SizeMode = PictureBoxSizeMode.AutoSize;
                // initialize display image
                if (frm.Frames.Count > 0)
                {
                    picBox.Image = frm.Frames[frmDisplayIndex].ToBitmap();
                }

                pnlImg.Controls.Add(picBox);
                tabPage.Controls.Add(pnlImg);

                // display properties of this frame
                ListBox listBox = new ListBox();
                listBox.Dock = DockStyle.Bottom;
                listBox.SelectionMode = SelectionMode.None;
                listBox.Items.Add("Version: " + frm.Version);
                listBox.Items.Add("FPS: " + frm.Fps);
                listBox.Items.Add("Action frame :" + frm.ActionFrame);
                listBox.Items.Add("Frames per direction: " + frm.FramesPerDirection);
                listBox.Items.Add("Total frames: " + frm.Frames.Count);
                listBox.Items.Add("Index:" + frmDisplayIndex);
                listBox.Items.Add("Direction: " + frm.GetDirection(frm.Frames[frmDisplayIndex]));
                tabPage.Controls.Add(listBox);

                tabControl.TabPages.Add(tabPage);
            }
            // select the first tab
            tabControl.SelectedIndex = 0;
            this.Controls.Add(tabControl);

            // next frame button
            btnNextFrame.Dock = DockStyle.Bottom;
            btnNextFrame.Text = "Next Frame";
            btnNextFrame.Click += BtnNextFrame_Click;
            this.Controls.Add(btnNextFrame);

            // prev frame button
            btnPrevFrame.Dock = DockStyle.Bottom;
            btnPrevFrame.Text = "Previous Frame";
            btnPrevFrame.Click += BtnPrevFrame_Click;
            this.Controls.Add(btnPrevFrame);

            // reset index button
            btnReset.Dock = DockStyle.Bottom;
            btnReset.Text = "Reset";
            btnReset.Click += BtnReset_Click;
            this.Controls.Add(btnReset);
        }

        /// <summary>
        /// The BtnReset_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BtnReset_Click(object sender, EventArgs e)
        {
            int index = tabControl.SelectedIndex;
            frmDisplayIndex = 0;
            TabPage tabPage = tabControl.TabPages[index];
            PictureBox picBox = (PictureBox)tabPage.Controls[0].Controls[0];

            FRM frm = frms[index];
            // display the next frame
            picBox.Image = frm.Frames[frmDisplayIndex].ToBitmap();

            // refresh the properties of the frame
            ListBox listBox = (ListBox)tabPage.Controls[1];
            listBox.Items.Clear();
            listBox.Items.Add("Version: " + frm.Version);
            listBox.Items.Add("FPS: " + frm.Fps);
            listBox.Items.Add("Action frame :" + frm.ActionFrame);
            listBox.Items.Add("Frames per direction: " + frm.FramesPerDirection);
            listBox.Items.Add("Total frames: " + frm.Frames.Count);
            listBox.Items.Add("Index:" + frmDisplayIndex);
            listBox.Items.Add("Direction: " + frm.GetDirection(frm.Frames[frmDisplayIndex]));
        }

        /// <summary>
        /// The BtnNextFrame_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BtnNextFrame_Click(object sender, EventArgs e)
        {
            int index = tabControl.SelectedIndex;

            if (frmDisplayIndex < frms[index].Frames.Count - 1)
            {
                frmDisplayIndex++;
                TabPage tabPage = tabControl.TabPages[index];
                PictureBox picBox = (PictureBox)tabPage.Controls[0].Controls[0];

                FRM frm = frms[index];
                // display the next frame
                picBox.Image = frm.Frames[frmDisplayIndex].ToBitmap();

                // refresh the properties of the frame
                ListBox listBox = (ListBox)tabPage.Controls[1];
                listBox.Items.Clear();
                listBox.Items.Add("Version: " + frm.Version);
                listBox.Items.Add("FPS: " + frm.Fps);
                listBox.Items.Add("Action frame :" + frm.ActionFrame);
                listBox.Items.Add("Frames per direction: " + frm.FramesPerDirection);
                listBox.Items.Add("Total frames: " + frm.Frames.Count);
                listBox.Items.Add("Index:" + frmDisplayIndex);
                listBox.Items.Add("Direction: " + frm.GetDirection(frm.Frames[frmDisplayIndex]));
            }
        }

        /// <summary>
        /// The BtnPrevFrame_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="EventArgs"/>.</param>
        private void BtnPrevFrame_Click(object sender, EventArgs e)
        {
            int index = tabControl.SelectedIndex;

            if (frmDisplayIndex > 0)
            {
                frmDisplayIndex--;
                TabPage tabPage = tabControl.TabPages[index];
                PictureBox picBox = (PictureBox)tabPage.Controls[0].Controls[0];

                FRM frm = frms[index];
                // display the previous frame
                picBox.Image = frm.Frames[frmDisplayIndex].ToBitmap();

                // refresh the properties of the frame
                ListBox listBox = (ListBox)tabPage.Controls[1];
                listBox.Items.Clear();
                listBox.Items.Add("Version: " + frm.Version);
                listBox.Items.Add("FPS: " + frm.Fps);
                listBox.Items.Add("Action frame :" + frm.ActionFrame);
                listBox.Items.Add("Frames per direction: " + frm.FramesPerDirection);
                listBox.Items.Add("Total frames: " + frm.Frames.Count);
                listBox.Items.Add("Index:" + frmDisplayIndex);
                listBox.Items.Add("Direction: " + frm.GetDirection(frm.Frames[frmDisplayIndex]));
            }
        }
    }
}
