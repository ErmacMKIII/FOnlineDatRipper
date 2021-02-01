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
    using NAudio.Wave;
    using System;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// Defines the <see cref="Program" />.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static void Main()
        {
            // Init palette (load 256 colors)
            Palette.Init();

            // Init Marko's Tables!
            Tables.Init();

            byte[] buffer;
            buffer = File.ReadAllBytes("08VATS.acm");

            byte[] Raw = new byte[0x2000000];
            ACMDecoder decoder = new ACMDecoder(buffer);
            int len = decoder.Decode(Raw);

            RawSourceWaveStream rawSourceWave = new RawSourceWaveStream(new MemoryStream(Raw, 0, len), new WaveFormat(22050, 16, 2));
            WaveOutEvent wo = new WaveOutEvent();
            if (wo.PlaybackState != PlaybackState.Playing)
            {
                wo.Init(rawSourceWave);
                wo.Play();
            }
            Thread.Sleep(3600 * 1000);

            // Create and display the form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(true);
            Application.Run(new MainForm());
        }
    }
}
