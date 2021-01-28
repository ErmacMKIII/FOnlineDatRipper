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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOnlineDatRipper
{
    struct ACMInfo
    {
        uint samples;
        uint channels;        
        uint bitrate;
        uint id;
        uint version;        

        public uint Samples { get => samples; set => samples = value; }
        public uint Channels { get => channels; set => channels = value; }
        public uint Bitrate { get => bitrate; set => bitrate = value; }
        public uint Id { get => id; set => id = value; }
        public uint Version { get => version; set => version = value; }
        
    }
    class ACMDecoder
    {
        public enum Filler
        {
            ZeroFill,
	        Return0,
	        LinearFill,	 
            k1_3bits,
            k1_2bits,
            t1_5bits,
            k2_4bits,
            k2_3bits,
            t2_7bits,
            k3_5bits,
            k3_4bits,
            k4_5bits,
            k4_4bits,
            t3_7bits
        }

        private Filler[] fillers =
        {
            Filler.ZeroFill,
            Filler.Return0,
            Filler.Return0,
            Filler.LinearFill,
            Filler.LinearFill,
            Filler.LinearFill,
            Filler.LinearFill,
            Filler.LinearFill,
            Filler.LinearFill,
            Filler.LinearFill,
            Filler.LinearFill,
            Filler.LinearFill,
            Filler.LinearFill,
            Filler.LinearFill,
            Filler.LinearFill,
            Filler.LinearFill,
            Filler.LinearFill,
            Filler.k1_3bits,
            Filler.k1_2bits,
            Filler.t1_5bits,
            Filler.k2_4bits,
            Filler.k2_3bits,
            Filler.t2_7bits,
            Filler.k3_5bits,
            Filler.k3_4bits,
            Filler.Return0,
            Filler.k4_5bits,
            Filler.k4_4bits,
            Filler.Return0,
            Filler.t3_7bits,
            Filler.Return0,
            Filler.Return0
        };

        // some relevant information
        private ACMInfo info = new ACMInfo();
        // source buffer (parsed from constructor and is file buffer)
        private readonly byte[] srcBuff;
        // source buffer position
        private int srcBuffPos = 0;

        // mid buffer (always 0x200 in size)
        private readonly byte[] midBuff = new byte[0x200];
        private int mPtr = 0;

        // Parameters of ACM stream
        private int packAttrs, someSize, packAttrs2, someSize2;

        //// destination buffer (contains decoded data)
        //private byte[] dstBuffer;
        //// destination buffer position
        //private int dstBuffPos = 0;

        // helpers
        private int bufferSize; // size of file buffer
        private int availBytes; // new (not yet processed) unsigned chars in file buffer
        private int[] decBuff, someBuff; // decBuff is decoded output and some buff is some buff

        private int dPtr = 0, sPtr = 0; // indexes of prior arrays

        private int blocks, totBlSize;
        private int valsToGo; // samples left to decompress        
        private int valCnt; // count of decompressed samples
        int[] values; // pointer to decompressed samples
        int vPtr = 0;

        // bits
        uint nextBits; // new bits
        int availBits; // count of new bits        

        internal ACMInfo Info { get => info; set => info = value; }

        public ACMDecoder(byte[] rawData)
        {
            this.srcBuff = rawData;
            Init();
        }

        private void Init()
        {
            bufferSize = 0x200;
            availBytes = 0;
            nextBits = 0;
            availBits = 0;

            info.Id = (uint)(GetBits(24) & 0xFFFFFF);
            Console.WriteLine("Id = " + info.Id);
            info.Version = ((uint)(GetBits(8) & 0xFF));
            Console.WriteLine("Ver = " + info.Version);

            valsToGo = (GetBits(16) & 0xFFFF);
            valsToGo |= ((GetBits(16) & 0xFFFF) << 16);

            info.Samples = (uint)valsToGo;
            Console.WriteLine("Samples = " + info.Samples);

            info.Channels = (uint)GetBits(16) & 0xFFFF;
            Console.WriteLine("Channels = " + info.Channels);
            info.Bitrate = (uint)GetBits(16) & 0xFFFF;
            Console.WriteLine("Bitrate = " + info.Bitrate);

            packAttrs = GetBits(4) & 0xF;
            packAttrs2 = GetBits(12) & 0xFFF;

            someSize = 1 << packAttrs;
            someSize2 = someSize * packAttrs2;

            int decBuf_size = 0;
            if (packAttrs != 0)
                decBuf_size = 3 * someSize / 2 - 2;

            this.blocks = 0x800 / someSize - 2;
            if (blocks < 1) blocks = 1;
            this.totBlSize = blocks * someSize;

            if (decBuf_size != 0)
            {
                this.decBuff = new int[decBuf_size];
            }

            this.someBuff = new int[someSize2];

            valCnt = 0;
            values = new int[valsToGo];
        }

        private void UnpackValues()
        {
            if (packAttrs == 0)
            {
                return;
            }

            int counter = packAttrs2;
            int sPtr = this.sPtr;
            int[] someBuff = this.someBuff;

            while (counter > 0)
            {
                int[] decBuff = this.decBuff;
                int dPtr = this.dPtr;

                int loc_blocks = blocks;
                int loc_someSize = someSize / 2;

                if (loc_blocks > counter)
                {
                    loc_blocks = counter;
                }

                loc_blocks *= 2;
                Sub_4d3fcc(decBuff, dPtr, someBuff, sPtr, loc_someSize, loc_blocks);
                dPtr += loc_someSize;

                for (int i = 0; i < loc_blocks; i++)
                {
                    someBuff[i * loc_someSize]++; 
                }

                loc_someSize /= 2;
                loc_blocks *= 2;

                while (loc_someSize != 0)
                {
                    Sub_4d420c(decBuff, dPtr, someBuff, sPtr, loc_someSize, loc_blocks);
                    dPtr += loc_someSize * 2;

                    loc_someSize /= 2;
                    loc_blocks *= 2;
                }

                counter -= blocks;
                sPtr += totBlSize;
            }
        }

        private byte ReadNextPortion()
        {            
            availBytes = Math.Min(srcBuff.Length - srcBuffPos, bufferSize);
            mPtr = 0;
            if (availBytes > 0)
            {                
                Array.Copy(srcBuff, srcBuffPos, midBuff, 0, availBytes);
                srcBuffPos += availBytes;
            }            

            availBytes--;
            return midBuff[mPtr++];
        }

        private void PrepareBits(int bits)
        {
            while (bits > availBits)
            {
                int oneByte;
                availBytes--;
                if (availBytes >= 0)
                {
                    oneByte = midBuff[mPtr++];
                }
                else
                {
                    oneByte = ReadNextPortion();
                }

                nextBits |= ((uint) oneByte << availBits);
                availBits += 8;
            }
        }

        private int GetBits(int bits)
        {
            PrepareBits(bits);
            int res = (int) nextBits;
            availBits -= bits;
            nextBits >>= bits;
            return res;
        }

        private bool CreateAmplitudeDictionary()
        {            
            int pwr = GetBits(4) & 0xF;
            int val = GetBits(16) & 0xFFFF;
            int count = 1 << pwr;
            int v = 0;

            int i;
            for (i = 0; i < count; i++)
            {
                AmplitudeBuffer.Middle(i, v);
                v += val;
            }

            v = -val;
            for (i = 0; i < count; i++)
            {
                AmplitudeBuffer.Middle(-i - 1, v);                
                v -= val;
            }

            // FillTables(). We have aleady done it, see definitions of Tables

            for (int pass = 0; pass < someSize; pass++)
            {
                int ind = GetBits(5) & 0x1F;
                int res = 0;
                switch (fillers[ind])
                {                   
                    case Filler.k1_2bits:
                        res = K1_2bits(pass, ind);
                        break;
                    case Filler.k1_3bits:
                        res = K1_3bits(pass, ind);
                        break;
                    case Filler.k2_3bits:
                        res = K2_3bits(pass, ind);
                        break;
                    case Filler.k2_4bits:
                        res = K2_4bits(pass, ind);
                        break;
                    case Filler.k3_4bits:
                        res = K3_4bits(pass, ind);
                        break;
                    case Filler.k3_5bits:
                        res = K3_5bits(pass, ind);
                        break;
                    case Filler.k4_4bits:
                        res = K4_4bits(pass, ind);
                        break;
                    case Filler.k4_5bits:
                        res = K4_5bits(pass, ind);
                        break;
                    case Filler.LinearFill:
                        res = LinearFill(pass, ind);
                        break;
                    case Filler.Return0:
                        res = Return0(pass, ind);
                        break;                    
                    case Filler.t1_5bits:
                        res = T1_5bits(pass, ind);
                        break;
                    case Filler.t2_7bits:
                        res = T2_7bits(pass, ind);
                        break;
                    case Filler.t3_7bits:
                        res = T3_7bits(pass, ind);
                        break;
                    case Filler.ZeroFill:
                        res = ZeroFill(pass, ind);
                        break;
                }

                if (res == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private bool MakeNewValues()
        {
            if (!CreateAmplitudeDictionary())
            {
                return false;
            }
            UnpackValues();
            
            valCnt = (someSize2 > valsToGo) ? valsToGo : someSize2;
            valsToGo -= valCnt;
            Array.Copy(someBuff, sPtr, values, vPtr, valCnt);
            vPtr += valCnt;

            return true;
        }

        /// <summary>
        /// Decode given ACM by reading into buffer (output)
        /// </summary>
        /// <param name="buffer"> acm buffer </param>
        /// <returns></returns>
        public int Decode(byte[] buffer)
        {            
            // while it's not fully read
            while (valsToGo != 0)
            {
                if (valCnt == 0)
                {
                    MakeNewValues();
                }
                valCnt--;
                Console.WriteLine("ValsToGo = " + valsToGo);
            }

            for (int i = 0; i < values.Length; i++)
            {
                byte val = (byte) (values[i] >> packAttrs);
                buffer[i] = val;
            }

            Console.WriteLine("Read Bytes = " + values.Length);
            return values.Length;
        }

        private void Sub_4d3fcc(int[] decBuff, int dPtr, int[] someBuff, int sPtr, int someSize, int blocks)
        {
            int row_0 = 0, row_1 = 0, row_2 = 0, row_3 = 0, db_0 = 0, db_1 = 0;
            if (blocks == 2)
            {
                for (int i = 0; i < someSize; i++)
                {
                    row_0 = someBuff[sPtr];
                    row_1 = someBuff[sPtr + someSize];
                    someBuff[sPtr] = someBuff[sPtr] + decBuff[dPtr] + 2 * decBuff[dPtr + 1];
                    someBuff[sPtr + someSize] = 2 * row_0 - decBuff[dPtr + 1] - someBuff[sPtr + someSize];
                    decBuff[dPtr] = row_0;
                    decBuff[dPtr + 1] = row_1;

                    dPtr += 2;
                    sPtr++;
                }
            }
            else if (blocks == 4)
            {
                for (int i = 0; i < someSize; i++)
                {
                    row_0 = someBuff[sPtr];
                    row_1 = someBuff[sPtr + someSize];
                    row_2 = someBuff[sPtr + 2 * someSize];
                    row_3 = someBuff[sPtr + 3 * someSize];

                    someBuff[sPtr] = decBuff[dPtr] + 2 * decBuff[dPtr + 1] + row_0;
                    someBuff[sPtr + someSize] = -decBuff[dPtr + 1] + 2 * row_0 - row_1;
                    someBuff[sPtr + 2 * someSize] = row_0 + 2 * row_1 + row_2;
                    someBuff[sPtr + 3 * someSize] = -row_1 + 2 * row_2 - row_3;

                    decBuff[dPtr] = row_2;
                    decBuff[dPtr+1] = row_3;

                    dPtr += 2;
                    sPtr++;
                }
            }
            else
            {
                for (int i = 0; i < someSize; i++)
                {
                    int[] x = someBuff;
                    int xPtr = sPtr; // some pointer
                    if (((blocks >> 1) & 1) != 0)
                    {
                        row_0 = x[xPtr];
                        row_1 = x[xPtr + someSize];

                        x[xPtr] = decBuff[dPtr] + 2 * decBuff[dPtr + 1] + row_0;
                        x[xPtr + someSize] = -decBuff[dPtr + 1] + 2 * row_0 - row_1;
                        xPtr += 2 * someSize;

                        db_0 = row_0;
                        db_1 = row_1;
                    }
                    else
                    {
                        db_0 = decBuff[dPtr];
                        db_1 = decBuff[dPtr + 1];
                    }

                    for (int j = 0; j < blocks >> 2; j++)
                    {
                        row_0 = x[xPtr]; x[xPtr] = db_0 + 2 * db_1 + row_0; xPtr += someSize;
                        row_1 = x[xPtr]; x[xPtr] = -db_1 + 2 * row_0 - row_1; xPtr += someSize;
                        row_2 = x[xPtr]; x[xPtr] = row_0 + 2 * row_1 + row_2; xPtr += someSize;
                        row_3 = x[xPtr]; x[xPtr] = -row_1 + 2 * row_2 - row_3; xPtr += someSize;

                        db_0 = row_2;
                        db_1 = row_3;
                    }
                    decBuff[dPtr] = row_2;
                    decBuff[dPtr + 1] = row_3;

                    dPtr += 2;
                    sPtr++;
                }
            }
        }

        private void Sub_4d420c(int[] decBuff, int dPtr, int[] someBuff, int sPtr, int someSize, int blocks)
        {
            int row_0 = 0, row_1 = 0, row_2 = 0, row_3 = 0, db_0 = 0, db_1 = 0;
            if (blocks == 4)
            {
                for (int i = 0; i < someSize; i++)
                {
                    row_0 = someBuff[sPtr];
                    row_1 = someBuff[sPtr + someSize];
                    row_2 = someBuff[sPtr + 2 * someSize];
                    row_3 = someBuff[sPtr + 3 * someSize];

                    someBuff[sPtr] = decBuff[sPtr] + 2 * decBuff[sPtr + 1] + row_0;
                    someBuff[sPtr + someSize] = -decBuff[sPtr + 1] + 2 * row_0 - row_1;
                    someBuff[sPtr + 2 * someSize] = row_0 + 2 * row_1 + row_2;
                    someBuff[sPtr + 3 * someSize] = -row_1 + 2 * row_2 - row_3;

                    decBuff[dPtr] = row_2;
                    decBuff[dPtr + 1] = row_3;

                    dPtr += 2;
                    sPtr++;
                }
            }
            else
            {
                for (int i = 0; i < someSize; i++)
                {
                    int[] x = someBuff;
                    int xPtr = sPtr; // some pointer again!                    
                    db_0 = decBuff[dPtr]; db_1 = decBuff[dPtr + 1];
                    for (int j = 0; j < blocks >> 2; j++)
                    {
                        row_0 = x[xPtr]; x[xPtr] = db_0 + 2 * db_1 + row_0; xPtr += someSize;
                        row_1 = x[xPtr]; x[xPtr] = -db_1 + 2 * row_0 - row_1; xPtr += someSize;
                        row_2 = x[xPtr]; x[xPtr] = row_0 + 2 * row_1 + row_2; xPtr += someSize;
                        row_3 = x[xPtr]; x[xPtr] = -row_1 + 2 * row_2 - row_3; xPtr += someSize;

                        db_0 = row_2;
                        db_1 = row_3;
                    }
                    decBuff[dPtr] = row_2;
                    decBuff[dPtr + 1] = row_3;

                    dPtr += 2;
                    sPtr++;
                }
            }
        }


        private int Return0(int pass, int ind)
        {
            return 0;
        }
        private int ZeroFill(int pass, int ind)
        {
            //Eng: used when the whole column #pass is zero-filled            

            //	the meaning is following:

            //	for (int i=0; i<packAttrs2; i++)
            //		someBuff [i, pass] = 0;

            for (int i = 0; i < packAttrs2; i++)
            {
                someBuff[i * someSize + pass] = 0;
            }

            return 1;
        }

        private int LinearFill(int pass, int ind)
        {
            for (int i = 0; i < packAttrs2; i++)
            {
                int val = GetBits(ind);
                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle(val);
            }

            return 1;
        }

        private int K1_3bits(int pass, int ind)
        {
            // Eng: column with number pass is filled with zeros, and also +/-1, zeros are repeated frequently            
            // efficiency (bits per value): 3-p0-2.5*p00, p00 - cnt of paired zeros, p0 - cnt of single zeros.
            // Eng: it makes sense to use, when the freqnecy of paired zeros (p00) is greater than 2/3            
            for (int i = 0; i < packAttrs2; i++)
            {
                PrepareBits(3);
                if ((nextBits & 1) == 0)
                {
                    availBits--;
                    nextBits >>= 1;
                    someBuff[i * someSize + pass] = 0; if ((++i) == packAttrs2) break;
                    someBuff[i * someSize + pass] = 0;
                }
                else if ((nextBits & 2) == 0)
                {
                    availBits -= 2;
                    nextBits >>= 2;
                    someBuff[i * someSize + pass] = 0;
                }
                else
                {
                    someBuff[i * someSize + pass] = ((nextBits & 4) != 0) ? AmplitudeBuffer.Middle(1) : AmplitudeBuffer.Middle(-1);
                    availBits -= 3;
                    nextBits >>= 3;
                }
            }

            return 1;
        }

       private int K1_2bits(int pass, int ind)
        {
            // Eng: column is filled with zero and +/-1
            // efficiency: 2-P0. P0 - cnt of any zero (P0 = p0 + p00)
            // Eng: use it when P0 > 1/3
            for (int i = 0; i < packAttrs2; i++)
            {
                PrepareBits(2);
                if ((nextBits & 1) == 0)
                {
                    availBits--;
                    nextBits >>= 1;
                    someBuff[i * someSize + pass] = 0;
                }
                else
                {
                    someBuff[i * someSize + pass] = ((nextBits & 2) != 0) ? AmplitudeBuffer.Middle(1) : AmplitudeBuffer.Middle(-1);
                    availBits -= 2;
                    nextBits >>= 2;
                }
            }

            return 1;
        }

        private int T1_5bits(int pass, int ind)
        {
            // Eng: all the -1, 0, +1 triplets
            // efficiency: always 5/3 bits per value
            // use it when P0 <= 1/3
            for (int i = 0; i < packAttrs2; i++)
            {
                byte bits = (byte)(GetBits(5) & 0x1F);
                bits = (byte)Tables.Table1[bits];

                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle(-1 + (bits & 3));
                if ((++i) == packAttrs2) break;
                bits >>= 2;
                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle(-1 + (bits & 3));
                if ((++i) == packAttrs2) break;
                bits >>= 2;
                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle(-1 + bits);
            }

            return 1;
        }
        private int K2_4bits(int pass, int ind)
        {
            // -2, -1, 0, 1, 2, and repeating zeros
            // efficiency: 4-2*p0-3.5*p00, p00 - cnt of paired zeros, p0 - cnt of single zeros.
            //Eng: makes sense to use when p00>2/3
            for (int i = 0; i < packAttrs2; i++)
            {
                PrepareBits(4);
                if ((nextBits & 1) == 0)
                {
                    availBits--;
                    nextBits >>= 1;
                    someBuff[i * someSize + pass] = 0; if ((++i) == packAttrs2) break;
                    someBuff[i * someSize + pass] = 0;
                }
                else if ((nextBits & 2) == 0)
                {
                    availBits -= 2;
                    nextBits >>= 2;
                    someBuff[i * someSize + pass] = 0;
                }
                else
                {
                    someBuff[i * someSize + pass] =
                       ((nextBits & 8) != 0) ?
                            (((nextBits & 4) != 0) ? AmplitudeBuffer.Middle(2) : AmplitudeBuffer.Middle(1)) :
                            (((nextBits & 4) != 0) ? AmplitudeBuffer.Middle(-1) : AmplitudeBuffer.Middle(-2));
                    availBits -= 4;
                    nextBits >>= 4;
                }
            }

            return 1;
        }

        private int K2_3bits(int pass, int ind)
        {
            // -2, -1, 0, 1, 2
            // efficiency: 3-2*P0, P0 - cnt of any zero (P0 = p0 + p00)
            // Eng: use when P0>1/3
            for (int i = 0; i < packAttrs2; i++)
            {
                PrepareBits(3);
                if ((nextBits & 1) == 0)
                {
                    availBits--;
                    nextBits >>= 1;
                    someBuff[i * someSize + pass] = 0;
                }
                else
                {
                    someBuff[i * someSize + pass] =
                            ((nextBits & 4) != 0) ?
                            (((nextBits & 2) != 0) ? AmplitudeBuffer.Middle(2) : AmplitudeBuffer.Middle(1)) :
                            (((nextBits & 2) != 0) ? AmplitudeBuffer.Middle(-1) : AmplitudeBuffer.Middle(-2));
                    availBits -= 3;
                    nextBits >>= 3;
                }
            }

            return 1;
        }

        private int T2_7bits(int pass, int ind)
        {
            // Eng: all the +/-2, +/-1, 0  triplets
            // efficiency: always 7/3 bits per value            
            // use it when p0 <= 1/3
            for (int i = 0; i < packAttrs2; i++)
            {
                byte bits = (byte)(GetBits(7) & 0x7F);
                short val = (short)Tables.Table2[bits];

                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle(-2 + (val & 7));
                if ((++i) == packAttrs2) break;
                val >>= 3;
                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle(-2 + (val & 7));
                if ((++i) == packAttrs2) break;
                val >>= 3;
                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle(-2 + val);
            }

            return 1;
        }
        private int K3_5bits(int pass, int ind)
        {
            // fills with values: -3, -2, -1, 0, 1, 2, 3, and double zeros
            // efficiency: 5-3*p0-4.5*p00-p1, p00 - cnt of paired zeros, p0 - cnt of single zeros, p1 - cnt of +/- 1.
            // can be used when frequency of paired zeros (p00) is greater than 2/3
            for (int i = 0; i < packAttrs2; i++)
            {
                PrepareBits(5);
                if ((nextBits & 1) == 0)
                {
                    availBits--;
                    nextBits >>= 1;
                    someBuff[i * someSize + pass] = 0; if ((++i) == packAttrs2) break;
                    someBuff[i * someSize + pass] = 0;
                }
                else if ((nextBits & 2) == 0)
                {
                    availBits -= 2;
                    nextBits >>= 2;
                    someBuff[i * someSize + pass] = 0;
                }
                else if ((nextBits & 4) == 0)
                {
                    someBuff[i * someSize + pass] = ((nextBits & 8) != 0) ? AmplitudeBuffer.Middle(1) : AmplitudeBuffer.Middle(-1);
                    availBits -= 4;
                    nextBits >>= 4;
                }
                else
                {
                    availBits -= 5;
                    int val = (int)((nextBits & 0x18) >> 3);
                    nextBits >>= 5;
                    if (val >= 2) val += 3;
                    someBuff[i * someSize + pass] = AmplitudeBuffer.Middle(-3 + val);
                }
            }

            return 1;
        }
        private int K3_4bits(int pass, int ind)
        {
            // fills with values: -3, -2, -1, 0, 1, 2, 3.
            // efficiency: 4-3*P0-p1, P0 - cnt of all zeros (P0 = p0 + p00), p1 - cnt of +/- 1.
            for (int i = 0; i < packAttrs2; i++)
            {
                PrepareBits(4);
                if ((nextBits & 1) == 0)
                {
                    availBits--;
                    nextBits >>= 1;
                    someBuff[i * someSize + pass] = 0;
                }
                else if ((nextBits & 2) == 0)
                {
                    availBits -= 3;
                    someBuff[i * someSize + pass] = ((nextBits & 4) != 0) ? AmplitudeBuffer.Middle(1) : AmplitudeBuffer.Middle(-1);
                    nextBits >>= 3;
                }
                else
                {
                    int val = (int)((nextBits & 0xC) >> 2);
                    availBits -= 4;
                    nextBits >>= 4;
                    if (val >= 2) val += 3;
                    someBuff[i * someSize + pass] = AmplitudeBuffer.Middle(-3 + val);
                }
            }

            return 1;
        }

        private int K4_5bits(int pass, int ind)
        {
            // fills with values: +/-4, +/-3, +/-2, +/-1, 0, and double zeros
            // efficiency: 5-3*p0-4.5*p00, p00 - cnt of paired zeros, p0 - cnt of single zeros.
            // Eng: makes sense to use when p00>2/3
            for (int i = 0; i < packAttrs2; i++)
            {
                PrepareBits(5);
                if ((nextBits & 1) == 0)
                {
                    availBits--;
                    nextBits >>= 1;
                    someBuff[i * someSize + pass] = 0; if ((++i) == packAttrs2) break;
                    someBuff[i * someSize + pass] = 0;
                }
                else if ((nextBits & 2) == 0)
                {
                    availBits -= 2;
                    nextBits >>= 2;
                    someBuff[i * someSize + pass] = 0;
                }
                else
                {
                    int val = (int)((nextBits & 0x1C) >> 2);
                    if (val >= 4) val++;
                    someBuff[i * someSize + pass] = AmplitudeBuffer.Middle(-4 + val);
                    availBits -= 5;
                    nextBits >>= 5;
                }
            }

            return 1;
        }

        private int K4_4bits(int pass, int ind)
        {
            // fills with values: +/-4, +/-3, +/-2, +/-1, 0, and double zeros
            // efficiency: 4-3*P0, P0 - cnt of all zeros (both single and paired).
            for (int i = 0; i < packAttrs2; i++)
            {
                PrepareBits(4);
                if ((nextBits & 1) == 0)
                {
                    availBits--;
                    nextBits >>= 1;
                    someBuff[i * someSize + pass] = 0;
                }
                else
                {
                    int val = (int)((nextBits & 0xE) >> 1);
                    availBits -= 4;
                    nextBits >>= 4;
                    if (val >= 4) val++;
                    someBuff[i * someSize + pass] = AmplitudeBuffer.Middle(-4 + val);
                }
            }

            return 1;
        }

        private int T3_7bits(int pass, int ind)
        {
            //Eng: all the pairs of values from -5 to +5
            // efficiency: 7/2 bits per value            
            for (int i = 0; i < packAttrs2; i++)
            {
                byte bits = (byte)(GetBits(7) & 0x7F);
                byte val = (byte)Tables.Table3[bits];

                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle(-5 + (val & 0xF));
                if ((++i) == packAttrs2) {
                    break;
                }
                val >>= 4;
                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle(-5 + val);                
            }

            return 1;
        }

    }

}
