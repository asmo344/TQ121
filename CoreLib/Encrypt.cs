using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
	public class Encrypt
	{
		private uint[] sboxEven = new uint[16];
        private uint[] sboxOdd = new uint[16];
        private uint[] sboxHigh = new uint[4];
        private uint[] invsEven = new uint[16];
        private uint[] invsOdd = new uint[16];
        private uint[] invsHigh = new uint[4];
        UInt16 startCode;
        UInt16 offset;
        public uint pixelLen;

		public void genSbox()
		{
			Random crandom = new Random();
			sboxEven[0] = (uint)(crandom.NextDouble() * 15.999);
			bool difFlag = true;
			for (int i = 1; i < 16; i++)
			{
				difFlag = true;
				while (difFlag)
				{
					uint tmp = (uint)(crandom.NextDouble() * 15.999);
					difFlag = false;
					for (int j = 0; j < i; j++)
					{
						if (tmp == sboxEven[j])
						{
							difFlag = true;
						}
					}

					if (difFlag == false)
					{
						sboxEven[i] = tmp;
					}
				}
			}

			for (int i = 0; i < 16; i++)
			{
				invsEven[sboxEven[i]] = (uint)i;
			}

			sboxOdd[0] = (uint)(crandom.NextDouble() * 15.999);
			difFlag = true;
			for (int i = 1; i < 16; i++)
			{
				difFlag = true;
				while (difFlag)
				{
					uint tmp = (uint)(crandom.NextDouble() * 15.999);
					difFlag = false;
					for (int j = 0; j < i; j++)
					{
						if (tmp == sboxOdd[j])
						{
							difFlag = true;
						}
					}

					if (difFlag == false)
					{
						sboxOdd[i] = tmp;
					}
				}
			}

			for (int i = 0; i < 16; i++)
			{
				invsOdd[sboxOdd[i]] = (uint)i;
			}

			sboxHigh[0] = (uint)(crandom.NextDouble() * 3.999);
			for (int i = 1; i < 4; i++)
			{
				difFlag = true;
				while (difFlag)
				{
					uint tmp = (uint)(crandom.NextDouble() * 3.999);
					difFlag = false;
					for (int j = 0; j < i; j++)
					{
						if (tmp == sboxHigh[j])
						{
							difFlag = true;
						}
					}

					if (difFlag == false)
					{
						sboxHigh[i] = tmp;
					}
				}
			}

			for (int i = 0; i < 4; i++)
			{
				invsHigh[sboxHigh[i]] = (uint)i;
			}

            startCode = (UInt16)(crandom.NextDouble() * 15.999);
            offset = (UInt16)(crandom.NextDouble() * 15.999);
        }

        private uint exchangeHL(uint input)
		{
			uint output = 0;
			for (int i = 0; i < 10; i++)
			{
				uint inBit = input % 2;
				input /= 2;
				output = output + inBit;
				output *= 2;
			}

			output /= 2;

			return output;
		}

        private uint funcInvs(uint input)
		{
			uint inOdd = 0;
			uint inEven = 0;
			for (int i = 0; i < 4; i++)
			{
				inOdd = inOdd + (input % 2);
				input /= 2;

				inEven = inEven + (input % 2);
				input /= 2;

				if (i < 3)
				{
					inOdd *= 2;
					inEven *= 2;
				}
			}

			uint outOdd = invsOdd[inOdd];
			uint outEven = invsEven[inEven];

			uint output = 0;
			for (int i = 0; i < 4; i++)
			{
				output = output + (outEven % 2);
				outEven /= 2;
				output *= 2;

				output = output + (outOdd % 2);
				outOdd /= 2;
				if (i < 3)
				{
					output *= 2;
				}
			}

			if (pixelLen == 10)
			{
				uint outHigh = invsHigh[input];
				output = output + outHigh * 256;
			}

			return output;

		}

        private uint funcSbox(uint input)
		{
			uint output = 0;
			uint inOdd = 0;
			uint inEven = 0;

			for (int i = 0; i < 4; i++)
			{
				inOdd = inOdd + (input % 2);
				input /= 2;

				inEven = inEven + (input % 2);
				input /= 2;

				if (i < 3)
				{
					inOdd *= 2;
					inEven *= 2;
				}

			}

			uint outOdd = sboxOdd[inOdd];
			uint outEven = sboxEven[inEven];

			for (int i = 0; i < 4; i++)
			{
				output = output + (outEven % 2);
				outEven = outEven / 2;
				output *= 2;

				output = output + (outOdd % 2);
				outOdd /= 2;
				if (i < 3)
				{
					output *= 2;
				}

				if (i == 4)
				{
					output = output * 2;
					output /= 2;
					outEven = 0;
					outOdd = 0;
					
				}
			}

			uint outHigh = sboxHigh[input];
			output = output + outHigh * 256;

			return output;
		}

		public uint[] encodeStream(uint[] plain, uint startCode, uint offset)
		{
			uint[] cipher = new uint[plain.Length];
			cipher[0] = funcSbox((((exchangeHL(plain[0]) ^ startCode) + offset) % 1024));

			for (int i = 1; i < cipher.Length; i++)
			{
				cipher[i] = funcSbox((((exchangeHL(plain[i]) ^ cipher[i - 1]) + offset) % 1024));
			}

			if (pixelLen == 8)
			{
				for (int i = 0; i < cipher.Length; i++)
				{
					cipher[i] = cipher[i] % 256;
				}
			}

			return cipher;
		}

		public int[] decodeStream(int[] cipher, uint startCode, uint offset)
		{
			int[] plain = new int[cipher.Length];
			plain[0] = (int)exchangeHL(((funcInvs((uint)cipher[0]) - offset) % 1024) ^ startCode);
			for (int i = 1; i < plain.Length; i++)
			{
				plain[i] = (int)exchangeHL(((funcInvs((uint)cipher[i]) - offset) % 1024) ^ (uint)cipher[i - 1]);
			}

			if (pixelLen == 8)
			{
				for (int i = 0; i < plain.Length; i++)
				{
					plain[i] = (plain[i] / 4);
				}
			}

			return plain;
		}

        public byte[] SboxEven
        {
            get {
                byte[] temp = new byte[sboxEven.Length];
                for (var idx = 0; idx < temp.Length; idx++)
                {
                    temp[idx] = (byte)(sboxEven[idx] & 0xF);
                }
                return temp;
            }
        }

        public byte[] SboxOdd
        {
            get
            {
                byte[] temp = new byte[sboxOdd.Length];
                for (var idx = 0; idx < temp.Length; idx++)
                {
                    temp[idx] = (byte)(sboxOdd[idx] & 0xF);
                }
                return temp;
            }
        }

        public byte[] SboxHigh
        {
            get
            {
                byte[] temp = new byte[sboxHigh.Length];
                for (var idx = 0; idx < temp.Length; idx++)
                {
                    temp[idx] = (byte)(sboxHigh[idx] & 0x3);
                }
                return temp;
            }
        }

        public UInt16 StartCode
        { get { return startCode; } }

        public UInt16 Offset
        { get { return offset; } }
    }
}
