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
    using NAudio.Wave;
    using System.IO;

    /// <summary>
    /// Defines the <see cref="ACM" />.
    /// </summary>
    internal class ACM : FOnlineFile
    {
        /// <summary>
        /// The ProgressUpdate.
        /// </summary>
        /// <param name="progress">The value<see cref="double"/>.</param>
        public delegate void ProgressUpdate(double progress);

        /// <summary>
        /// Defines the OnProgressUpdate.
        /// </summary>
        public event ProgressUpdate OnProgressUpdate;

        /// <summary>
        /// Represents decoded data (64 MB buffer)........
        /// </summary>
        private readonly byte[] content = new byte[0x4000000];

        /// <summary>
        /// Read only tag as a display name (on the tab for example).........
        /// </summary>
        private readonly string tag;

        /// <summary>
        /// Gets the Tag
        /// Tag for this acm (for display)........
        /// </summary>
        public string Tag => tag;

        /// <summary>
        /// Gets the Content
        /// Decoded bytes........
        /// </summary>
        public byte[] Content => content;

        /// <summary>
        /// Content as Wave Stream........
        /// </summary>
        private RawSourceWaveStream waveStream;

        /// <summary>
        /// Gets the WaveStream.
        /// </summary>
        public RawSourceWaveStream WaveStream { get => waveStream; }

        /// <summary>
        /// Length of Content Buffer........
        /// </summary>
        private int length = 0;

        /// <summary>
        /// Gets the Length
        /// Length of Content Buffer........
        /// </summary>
        public int Length { get => length; }

        /// <summary>
        /// Gets a value indicating whether Error.
        /// </summary>
        public bool Error { get => error; }

        /// <summary>
        /// Gets the ErrorMessage.
        /// </summary>
        public string ErrorMessage { get => errorMessage; }

        /// <summary>
        /// Did error occurred?..
        /// </summary>
        private bool error = false;

        /// <summary>
        /// Error message for display..
        /// </summary>
        private string errorMessage = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="ACM"/> class.
        /// </summary>
        /// <param name="acmFile">.</param>
        public ACM(string acmFile)
        {
            int lastIndex = acmFile.LastIndexOf('\\');
            this.tag = acmFile.Substring(lastIndex + 1);
        }

        /// <summary>
        /// Decode packed bytes.
        /// </summary>
        /// <param name="acmFile">The acmFile<see cref="string"/>.</param>
        public override void ReadFile(string acmFile)
        {
            bool ok = false;
            byte[] acmBytes = File.ReadAllBytes(acmFile);

            ACMDecoder acmDecoder = new ACMDecoder(acmBytes);
            if (acmDecoder.Info.Id != 0x32897)
            {
                errorMessage = "Error - ACM file does not have valid Id!";
            }
            else if (acmDecoder.Info.Version != 0x01)
            {
                errorMessage = "Error - ACM file is not of correct version!";
            }
            else
            {
                length = acmDecoder.Decode(this.content);
                if (length == acmDecoder.Info.Samples * 2)
                {
                    this.waveStream = new RawSourceWaveStream(
                        new MemoryStream(this.content, 0, this.length),
                        new WaveFormat((int)(acmDecoder.Info.Bitrate), 16, 2)
                    );
                    ok = true;
                }
                else
                {
                    errorMessage = "Error - ACM file is not valid (Decoding error)!";
                }
            }

            this.error = !ok;
        }

        /// <summary>
        /// Decode packed bytes.
        /// </summary>
        /// <param name="acmBytes">.</param>
        public void ReadBytes(byte[] acmBytes)
        {
            bool ok = false;

            ACMDecoder acmDecoder = new ACMDecoder(acmBytes);
            if (acmDecoder.Info.Id != 0x32897)
            {
                errorMessage = "Error - ACM file does not have valid Id!";
            }
            else if (acmDecoder.Info.Version != 0x01)
            {
                errorMessage = "Error - ACM file is not of correct version!";
            }
            else
            {
                length = acmDecoder.Decode(this.content);
                if (length == acmDecoder.Info.Samples * 2)
                {
                    this.waveStream = new RawSourceWaveStream(
                        new MemoryStream(this.content, 0, this.length),
                        new WaveFormat((int)(acmDecoder.Info.Bitrate), 16, 2)
                    );
                    ok = true;
                }
                else
                {
                    errorMessage = "Error - ACM file is not valid (Decoding error)!";
                }
            }

            this.error = !ok;
        }

        /// <summary>
        /// The GetTag.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string GetTag()
        {
            return Tag;
        }

        /// <summary>
        /// The IsError.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool IsError()
        {
            return Error;
        }

        /// <summary>
        /// The GetErrorMessage.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public override string GetErrorMessage()
        {
            return errorMessage;
        }

        /// <summary>
        /// The GetProgress.
        /// </summary>
        /// <returns>The <see cref="double"/>.</returns>
        public override double GetProgress()
        {
            throw new System.NotImplementedException();
        }
    }
}
