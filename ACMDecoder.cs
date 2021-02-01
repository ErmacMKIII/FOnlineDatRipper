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
    using System;
    using System.IO;

    /// <summary>
    /// Defines the <see cref="ACMInfo" />.
    /// </summary>
    struct ACMInfo
    {
        /// <summary>
        /// Defines the samples.
        /// </summary>
        internal uint samples;

        /// <summary>
        /// Defines the channels.
        /// </summary>
        internal uint channels;

        /// <summary>
        /// Defines the bitrate.
        /// </summary>
        internal uint bitrate;

        /// <summary>
        /// Defines the id.
        /// </summary>
        internal uint id;

        /// <summary>
        /// Defines the version.
        /// </summary>
        internal uint version;

        /// <summary>
        /// Gets or sets the Samples.
        /// </summary>
        public uint Samples { get => samples; set => samples = value; }

        /// <summary>
        /// Gets or sets the Channels.
        /// </summary>
        public uint Channels { get => channels; set => channels = value; }

        /// <summary>
        /// Gets or sets the Bitrate.
        /// </summary>
        public uint Bitrate { get => bitrate; set => bitrate = value; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public uint Id { get => id; set => id = value; }

        /// <summary>
        /// Gets or sets the Version.
        /// </summary>
        public uint Version { get => version; set => version = value; }
    }

    /// <summary>
    /// Defines the <see cref="ACMDecoder" />.
    /// </summary>
    internal class ACMDecoder
    {
        /// <summary>
        /// Defines the Filler.
        /// </summary>
        public enum Filler
        {
            /// <summary>
            /// Defines the ZeroFill.
            /// </summary>
            ZeroFill,
            /// <summary>
            /// Defines the Return0.
            /// </summary>
            Return0,
            /// <summary>
            /// Defines the LinearFill.
            /// </summary>
            LinearFill,
            /// <summary>
            /// Defines the k1_3bits.
            /// </summary>
            k1_3bits,
            /// <summary>
            /// Defines the k1_2bits.
            /// </summary>
            k1_2bits,
            /// <summary>
            /// Defines the t1_5bits.
            /// </summary>
            t1_5bits,
            /// <summary>
            /// Defines the k2_4bits.
            /// </summary>
            k2_4bits,
            /// <summary>
            /// Defines the k2_3bits.
            /// </summary>
            k2_3bits,
            /// <summary>
            /// Defines the t2_7bits.
            /// </summary>
            t2_7bits,
            /// <summary>
            /// Defines the k3_5bits.
            /// </summary>
            k3_5bits,
            /// <summary>
            /// Defines the k3_4bits.
            /// </summary>
            k3_4bits,
            /// <summary>
            /// Defines the k4_5bits.
            /// </summary>
            k4_5bits,
            /// <summary>
            /// Defines the k4_4bits.
            /// </summary>
            k4_4bits,
            /// <summary>
            /// Defines the t3_7bits.
            /// </summary>
            t3_7bits
        }

        /// <summary>
        /// Defines the fillers.
        /// </summary>
        private readonly Filler[] fillers =
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

        /// <summary>
        /// Defines the info.
        /// </summary>
        private ACMInfo info = new ACMInfo();

        /// <summary>
        /// Defines the srcBuff.
        /// </summary>
        private readonly byte[] srcBuff;

        /// <summary>
        /// Defines the srcBuffPos.
        /// </summary>
        private int srcBuffPos = 0;

        /// <summary>
        /// Defines the packAttrs, someSize, packAttrs2, someSize2...
        /// </summary>
        private int packAttrs, someSize, packAttrs2, someSize2;

        /// <summary>
        /// Defines the MidBuffSize.
        /// </summary>
        private const int MidBuffSize = 0x800;

        /// <summary>
        /// Defines the midBuff.
        /// </summary>
        private readonly byte[] midBuff = new byte[MidBuffSize];

        /// <summary>
        /// Defines the mPtr.
        /// </summary>
        private int mPtr = 0;

        /// <summary>
        /// Defines the decBuff.
        /// </summary>
        private int[] decBuff;

        /// <summary>
        /// Defines the someBuff.
        /// </summary>
        private int[] someBuff;

        /// <summary>
        /// Defines the blocks, totBlSize...
        /// </summary>
        private int blocks, totBlSize;

        /// <summary>
        /// Defines the valsToGo.
        /// </summary>
        private int valsToGo;// samples left to decompress

        /// <summary>
        /// Defines the valCnt.
        /// </summary>
        private int valCnt;// count of decompressed samples

        /// <summary>
        /// Defines the values.
        /// </summary>
        internal int[] values;

        /// <summary>
        /// Defines the vPtr.
        /// </summary>
        internal int vPtr = 0;

        /// <summary>
        /// Defines the availBytes.
        /// </summary>
        internal int availBytes = 0;

        /// <summary>
        /// Defines the nextBits.
        /// </summary>
        internal int nextBits;// new bits

        /// <summary>
        /// Defines the availBits.
        /// </summary>
        internal int availBits;// count of new bits

        /// <summary>
        /// Gets or sets the Info.
        /// </summary>
        internal ACMInfo Info { get => info; set => info = value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ACMDecoder"/> class.
        /// </summary>
        /// <param name="acmData">The rawData<see cref="byte[]"/>.</param>
        public ACMDecoder(byte[] acmData)
        {
            this.srcBuff = acmData;
            Init();
        }

        /// <summary>
        /// The Init.
        /// </summary>
        private void Init()
        {
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

            packAttrs = GetBits(4) & 0xF; // known as acm_level
            packAttrs2 = GetBits(12) & 0xFFF; // known as rows

            someSize = 1 << packAttrs; // known as columns
            someSize2 = someSize * packAttrs2;

            int decBuf_size = 0;
            if (packAttrs != 0)
            {
                decBuf_size = 2 * someSize - 2;
            }

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

        /// <summary>
        /// The UnpackValues.
        /// </summary>
        private void UnpackValues()
        {
            if (packAttrs == 0)
            {
                return;
            }

            int[] xBuff = someBuff;
            int xPtr = 0;

            int counter = packAttrs2;

            while (counter > 0)
            {
                int[] yBuff = decBuff;
                int yPtr = 0;

                int loc_blocks = blocks;
                int loc_someSize = someSize / 2;

                if (loc_blocks > counter)
                {
                    loc_blocks = counter;
                }

                loc_blocks *= 2;
                Juggle(Convert(yBuff), yPtr, xBuff, xPtr, loc_someSize, loc_blocks);
                yPtr += loc_someSize;

                for (int i = 0; i < loc_blocks; i++)
                {
                    xBuff[i * loc_someSize]++;
                }

                loc_someSize /= 2;
                loc_blocks *= 2;

                while (loc_someSize != 0)
                {
                    Juggle(SimplePass(yBuff), yPtr, xBuff, xPtr, loc_someSize, loc_blocks);
                    yPtr += loc_someSize * 2;

                    loc_someSize /= 2;
                    loc_blocks *= 2;
                }

                counter -= blocks;
                xPtr += totBlSize;
            }
        }

        /// <summary>
        /// The ReadNextPortion.
        /// </summary>
        /// <returns>The <see cref="byte"/>.</returns>
        private byte ReadNextPortion()
        {
            availBytes = Math.Min(MidBuffSize, srcBuff.Length - srcBuffPos);

            if (availBytes > 0)
            {
                mPtr = 0;
                Array.Copy(srcBuff, srcBuffPos, midBuff, 0, availBytes);
                srcBuffPos += availBytes;
            }

            return midBuff[mPtr++];
        }

        /// <summary>
        /// The PrepareBits.
        /// </summary>
        /// <param name="bits">The bits<see cref="int"/>.</param>
        private void PrepareBits(int bits)
        {
            while (bits > availBits)
            {
                byte oneByte;
                availBytes--;
                if (availBytes > 0)
                {
                    oneByte = midBuff[mPtr++];
                }
                else
                {
                    oneByte = ReadNextPortion();
                }
                nextBits |= oneByte << availBits;
                availBits += 8;
            }
        }

        /// <summary>
        /// The GetBits.
        /// </summary>
        /// <param name="bits">The bits<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        private int GetBits(int bits)
        {
            PrepareBits(bits);
            int res = nextBits & ((1 << bits) - 1);
            availBits -= bits;
            nextBits >>= bits;
            return res;
        }

        /// <summary>
        /// The CreateAmplitudeDictionary.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool CreateAmplitudeDictionary()
        {
            int pwr = GetBits(4) & 0xF;
            int val = GetBits(16) & 0xFFFF;
            int count = 1 << pwr;
            int v = 0;

            int i;
            for (i = 0; i < count; i++)
            {
                AmplitudeBuffer.Middle((short)i, (short)v);
                v += val;
            }

            v = -val;
            for (i = 0; i < count; i++)
            {
                AmplitudeBuffer.Middle((short)(-i - 1), (short)v);
                v -= val;
            }

            // FillTables(). We have aleady done it, see definitions of Tables

            for (int pass = 0; pass < someSize; pass++)
            {
                int ind = GetBits(5) & 0x1F;
                int res = 0;
                Filler filler = fillers[ind];

                switch (filler)
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

        /// <summary>
        /// The MakeNewValues.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool MakeNewValues()
        {
            if (!CreateAmplitudeDictionary())
            {
                return false;
            }
            UnpackValues();

            valCnt = Math.Min(valsToGo, someSize2);
            Array.Copy(someBuff, 0, values, vPtr, valCnt);
            vPtr += valCnt;

            valsToGo -= valCnt;

            return true;
        }

        /// <summary>
        /// Decode given ACM by reading into buffer (output).
        /// </summary>
        /// <param name="buffer"> acm buffer .</param>
        /// <returns>.</returns>
        public int Decode(byte[] buffer)
        {
            // while it's not fully read
            while (valsToGo != 0)
            {
                MakeNewValues();
                // Console.WriteLine("ValsToGo = " + valsToGo);
            }

            // copy decoded values into the buffer
            int bPtr = 0;

            foreach (int value in values)
            {
                int x = value >> packAttrs;
                buffer[bPtr] = (byte)(x & 0xFF);
                buffer[bPtr + 1] = (byte)((x >> 8) & 0xFF);

                bPtr += 2;
            }

            // return buffer length
            Console.WriteLine("Read Bytes = " + bPtr);

            File.WriteAllBytes("tst.dat", buffer);
            return bPtr;
        }

        /// <summary>
        /// Converts integer aray to short array, twice the size of original.
        /// </summary>
        /// <param name="sourceArray">original integer array.</param>
        /// <returns>destination short array.</returns>
        private static short[] Convert(int[] sourceArray)
        {
            short[] result = new short[sourceArray.Length * 2];
            int resPtr = 0;
            foreach (int src in sourceArray)
            {
                byte[] bytes = BitConverter.GetBytes(src);
                result[resPtr] = BitConverter.ToInt16(bytes, 0);
                result[resPtr + 1] = BitConverter.ToInt16(bytes, 2);
                resPtr += 2;
            }

            return result;
        }

        /// <summary>
        /// Make destination array as simple pass of source array.
        /// </summary>
        /// <param name="sourceArray">original integer array.</param>
        /// <returns>destination short array.</returns>
        private static short[] SimplePass(int[] sourceArray)
        {
            short[] result = new short[sourceArray.Length];
            int resPtr = 0;
            foreach (int src in sourceArray)
            {
                result[resPtr++] = (short)src;
            }

            return result;
        }

        /// <summary>
        /// Marko's method of block decomposition; 
        /// Source: Libacm github by Marko Kreen.
        /// </summary>
        /// <param name="decBuff">unpacking buffer.</param>
        /// <param name="dPtr">.</param>
        /// <param name="someBuff">buffer with decompressed samples.</param>
        /// <param name="sPtr">index of some buffer.</param>
        /// <param name="someSize">number of iterations.</param>
        /// <param name="blocks">number of blocks.</param>
        /// <returns>The <see cref="short[]"/>.</returns>
        private static short[] Juggle(short[] decBuff, int dPtr, int[] someBuff, int sPtr, int someSize, int blocks)
        {
            int i, j;
            int[] x;
            int xPtr;
            int row0, row1, row2, row3;
            for (i = 0; i < someSize; i++)
            {
                x = someBuff;
                xPtr = sPtr;
                row0 = decBuff[dPtr];
                row1 = decBuff[dPtr + 1];
                for (j = 0; j < blocks / 2; j++)
                {
                    row2 = x[xPtr]; x[xPtr] = row1 * 2 + (row0 + row2); xPtr += someSize;
                    row3 = x[xPtr]; x[xPtr] = row2 * 2 - (row1 + row3); xPtr += someSize;
                    row0 = row2; row1 = row3;
                }

                decBuff[dPtr] = (short)row0;
                decBuff[dPtr + 1] = (short)row1;

                dPtr += 2;
                sPtr++;
            }

            return decBuff;
        }

        /// <summary>
        /// The Return0.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        private int Return0(int pass, int ind)
        {
            return 0;
        }

        /// <summary>
        /// The ZeroFill.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
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

        /// <summary>
        /// The LinearFill.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        private int LinearFill(int pass, int ind)
        {
            int mask = (1 << ind) - 1;
            int middle = 1 << (ind - 1);

            for (int i = 0; i < packAttrs2; i++)
            {
                int b = GetBits(ind) & mask;
                int index = b - middle;
                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle((short)index);
            }

            return 1;
        }

        /// <summary>
        /// The K1_3bits.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int K1_3bits(int pass, int ind)
        {
            //Eng: column with number pass is filled with zeros, and also +/-1, zeros are repeated frequently
            //Rus: c������ pass �������� ������, � ����� +/- 1, �� ���� ����� ���� ������
            // efficiency (bits per value): 3-p0-2.5*p00, p00 - cnt of paired zeros, p0 - cnt of single zeros.
            //Eng: it makes sense to use, when the freqnecy of paired zeros (p00) is greater than 2/3
            //Rus: ����� ����� ������������, ����� ����������� ������ ����� (p00) ������ 2/3
            for (int i = 0; i < packAttrs2; i++)
            {
                PrepareBits(3);
                if ((nextBits & 1) == 0)
                {
                    availBits--;
                    nextBits >>= 1;
                    someBuff[i * someSize + pass] = 0;
                    if ((++i) == packAttrs2)
                    {
                        break;
                    }
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

        /// <summary>
        /// The K1_2bits.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int K1_2bits(int pass, int ind)
        {
            //Eng: column is filled with zero and +/-1
            //Rus: c������ pass �������� ������, � ����� +/- 1
            // efficiency: 2-P0. P0 - cnt of any zero (P0 = p0 + p00)
            //Eng: use it when P0 > 1/3
            //Rus: ����� ����� ������������, ����� ����������� ���� ������ 1/3
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

        /// <summary>
        /// The T1_5bits.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int T1_5bits(int pass, int ind)
        {
            //Eng: all the -1, 0, +1 triplets
            //Rus: ��� ���������� ����� -1, 0, +1.
            // efficiency: always 5/3 bits per value
            // use it when P0 <= 1/3
            for (int i = 0; i < packAttrs2; i++)
            {
                int bits = GetBits(5) & Tables.Table1Mask;
                bits = Tables.Table1[bits];

                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle((short)(-1 + (bits & 3)));
                if ((++i) == packAttrs2)
                {
                    break;
                }
                bits >>= 2;
                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle((short)(-1 + (bits & 3)));
                if ((++i) == packAttrs2)
                {
                    break;
                }
                bits >>= 2;
                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle((short)(-1 + bits));
            }
            return 1;
        }

        /// <summary>
        /// The K2_4bits.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int K2_4bits(int pass, int ind)
        {
            // -2, -1, 0, 1, 2, and repeating zeros
            // efficiency: 4-2*p0-3.5*p00, p00 - cnt of paired zeros, p0 - cnt of single zeros.
            //Eng: makes sense to use when p00>2/3
            //Rus: ����� ����� ������������, ����� ����������� ������ ����� (p00) ������ 2/3
            for (int i = 0; i < packAttrs2; i++)
            {
                PrepareBits(4);
                if ((nextBits & 1) == 0)
                {
                    availBits--;
                    nextBits >>= 1;
                    someBuff[i * someSize + pass] = 0;
                    if ((++i) == packAttrs2)
                    {
                        break;
                    }
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
                    someBuff[i * someSize + pass] = (nextBits & 8) != 0 ? (((nextBits & 4) != 0) ? AmplitudeBuffer.Middle(2) : AmplitudeBuffer.Middle(1))
                        : (((nextBits & 4) != 0) ? AmplitudeBuffer.Middle(-1) : AmplitudeBuffer.Middle(-2));
                    availBits -= 4;
                    nextBits >>= 4;
                }
            }
            return 1;
        }

        /// <summary>
        /// The K2_3bits.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int K2_3bits(int pass, int ind)
        {
            // -2, -1, 0, 1, 2
            // efficiency: 3-2*P0, P0 - cnt of any zero (P0 = p0 + p00)
            //Eng: use when P0>1/3
            //Rus: ����� ����� ������������, ����� ����������� ���� ������ 1/3
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
                    someBuff[i * someSize + pass] = ((nextBits & 4) != 0) ?
                        (((nextBits & 2) != 0) ? AmplitudeBuffer.Middle(2) : AmplitudeBuffer.Middle(1))
                        : (((nextBits & 2) != 0) ? AmplitudeBuffer.Middle(-1) : AmplitudeBuffer.Middle(-2));
                    availBits -= 3;
                    nextBits >>= 3;
                }
            }
            return 1;
        }

        /// <summary>
        /// The T2_7bits.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int T2_7bits(int pass, int ind)
        {
            //Eng: all the +/-2, +/-1, 0  triplets
            // efficiency: always 7/3 bits per value
            //Rus: ��� ���������� ����� -2, -1, 0, +1, 2.
            // �������������: 7/3 ���� �� �������� - ������
            // use it when p0 <= 1/3
            for (int i = 0; i < packAttrs2; i++)
            {
                int bits = GetBits(7) & Tables.Table2Mask;
                short val = Tables.Table2[bits];

                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle((short)(-2 + (val & 7)));
                if ((++i) == packAttrs2)
                {
                    break;
                }
                val >>= 3;
                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle((short)(-2 + (val & 7)));
                if ((++i) == packAttrs2)
                {
                    break;
                }
                val >>= 3;
                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle((short)(-2 + val));
            }
            return 1;
        }

        /// <summary>
        /// The K3_5bits.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int K3_5bits(int pass, int ind)
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
                    someBuff[i * someSize + pass] = 0;
                    if ((++i) == packAttrs2)
                    {
                        break;
                    }
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
                    int val = (nextBits & 0x18) >> 3;
                    nextBits >>= 5;
                    if (val >= 2)
                    {
                        val += 3;
                    }
                    someBuff[i * someSize + pass] = AmplitudeBuffer.Middle((short)(-3 + val));
                }
            }
            return 1;
        }

        /// <summary>
        /// The K3_4bits.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int K3_4bits(int pass, int ind)
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
                    int val = (nextBits & 0xC) >> 2;
                    availBits -= 4;
                    nextBits >>= 4;
                    if (val >= 2)
                    {
                        val += 3;
                    }
                    someBuff[i * someSize + pass] = AmplitudeBuffer.Middle((short)(-3 + val));
                }
            }
            return 1;
        }

        /// <summary>
        /// The K4_5bits.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int K4_5bits(int pass, int ind)
        {
            // fills with values: +/-4, +/-3, +/-2, +/-1, 0, and double zeros
            // efficiency: 5-3*p0-4.5*p00, p00 - cnt of paired zeros, p0 - cnt of single zeros.
            //Eng: makes sense to use when p00>2/3
            //Rus: ����� ����� ������������, ����� ����������� ������ ����� (p00) ������ 2/3
            for (int i = 0; i < packAttrs2; i++)
            {
                PrepareBits(5);
                if ((nextBits & 1) == 0)
                {
                    availBits--;
                    nextBits >>= 1;
                    someBuff[i * someSize + pass] = 0;
                    if ((++i) == packAttrs2)
                    {
                        break;
                    }
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
                    int val = (nextBits & 0x1C) >> 2;
                    if (val >= 4)
                    {
                        val++;
                    }
                    someBuff[i * someSize + pass] = AmplitudeBuffer.Middle((short)(-4 + val));
                    availBits -= 5;
                    nextBits >>= 5;
                }
            }
            return 1;
        }

        /// <summary>
        /// The K4_4bits.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int K4_4bits(int pass, int ind)
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
                    int val = (nextBits & 0xE) >> 1;
                    availBits -= 4;
                    nextBits >>= 4;
                    if (val >= 4)
                    {
                        val++;
                    }
                    someBuff[i * someSize + pass] = AmplitudeBuffer.Middle((short)(-4 + val));
                }
            }
            return 1;
        }

        /// <summary>
        /// The T3_7bits.
        /// </summary>
        /// <param name="pass">The pass<see cref="int"/>.</param>
        /// <param name="ind">The ind<see cref="int"/>.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public int T3_7bits(int pass, int ind)
        {
            //Eng: all the pairs of values from -5 to +5
            // efficiency: 7/2 bits per value
            //Rus: ��� ���������� ��� �� -5 �� +5
            // �������������: 7/2 ���� �� �������� - ������
            for (int i = 0; i < packAttrs2; i++)
            {
                int bits = GetBits(7) & Tables.Table3Mask;
                byte val = Tables.Table3[bits];

                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle((short)(-5 + (val & 0xF)));
                if ((++i) == packAttrs2)
                {
                    break;
                }
                val >>= 4;
                someBuff[i * someSize + pass] = AmplitudeBuffer.Middle((short)(-5 + val));
            }
            return 1;
        }
    }
}
