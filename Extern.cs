#region copyright
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
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOnlineDatRipper
{
    class Extern : FOnlineFile
    {
        public override string GetErrorMessage()
        {
            throw new NotImplementedException();
        }

        public override FOType GetFOFileType()
        {
            return FOType.EXT;
        }

        public override double GetProgress()
        {
            throw new NotImplementedException();
        }

        public override string GetTag()
        {
            throw new NotImplementedException();
        }

        public override bool IsError()
        {
            throw new NotImplementedException();
        }

        public override void ReadFile(string file)
        {
            throw new NotImplementedException();
        }
    }
}
