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
    /// <summary>
    /// Defines the <see cref="FOnlineFile" />.
    /// </summary>
    internal abstract class FOnlineFile
    {
        public enum FOType
        {
            ACM, FRM, DAT
        }

        /// <summary>
        /// Get type. It's one of the following {ACM, FRM, DAT}.
        /// </summary>
        /// <returns></returns>
        public abstract FOType GetFOFileType();

        /// <summary>
        /// Read file {ACM, FRM or FALLOUT2 DAT}.
        /// </summary>
        /// <param name="file">.</param>
        public abstract void ReadFile(string file);

        /// <summary>
        /// Gets filename that serves as a tag.
        /// </summary>
        /// <returns>Filename Tag.</returns>
        public abstract string GetTag();

        /// <summary>
        /// Tells if error occurred during reading this FOnline File.
        /// </summary>
        /// <returns>Did error occur?.</returns>
        public abstract bool IsError();

        /// <summary>
        /// If an error occurred read the message to display it.
        /// </summary>
        /// <returns>Error message.</returns>
        public abstract string GetErrorMessage();

        /// <summary>
        /// Gets reading Progress.
        /// </summary>
        /// <returns>Progress.</returns>
        public abstract double GetProgress();
    }
}
