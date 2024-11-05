// ******************************************************************************************************************************
// Copyright © Richard Dunkley 2024
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ******************************************************************************************************************************
using BlockEditGen.Data;
using BlockEditGen.Interfaces;
using BlockEditGen.Parse;
using System;

namespace BlockEditGen.ViewModels
{
	public class UInt16ViewModel : UnsignedViewModelBase<ushort>
	{
		public UInt16ViewModel(Value value, ICachedRegisterBlock block)
			: base(value, block)
		{
			var size = sizeof(ushort);
			if (value.Type != Value.TypeEnum.Uint16)
				throw new ArgumentException($"The value provided ({value.Name}) has an invalid type ({value.Type}) for this object.");
			if (value.Length.TotalBits > size * 8)
				throw new ArgumentException($"The value provided ({value.Name}) has a bit width ({value.Length.TotalBits}) longer than a {value.Type} ({size * 8}).");
		}

		#region UnsignedViewModelBase

		protected override ushort GetBitMask(int numBits)
		{
			return (ushort)GenerateBitMask(numBits);
		}

		protected override byte[] ValueToBytes(ushort value)
		{
			var buf = new byte[LengthInBytes];
			if (ValueEndian == Endian.Little)
			{
				for (int i = 0; i < buf.Length; i++)
				{
					buf[i] = (byte)value;
					value >>= 8;
				}
			}
			else
			{
				for (int i = buf.Length - 1; i >= 0; i--)
				{
					buf[i] = (byte)value;
					value >>= 8;
				}
			}
			return buf;
		}

		protected override string ValueToString(ushort value)
		{
			if (ViewType == IntegerViewType.Binary)
				return $"{Convert.ToString(value, 2).PadLeft(NumberOfBits, '0')}b";
			if (ViewType == IntegerViewType.Hexadecimal)
				return $"0x{value.ToString($"X{(NumberOfBits + 3) / 4}")}";
			return $"{value:N0}";
		}

		protected override ushort BytesToValue(byte[] buf)
		{
			ushort value = 0;
			if (ValueEndian == Endian.Little)
			{
				for (int i = 0; i < buf.Length; i++)
					value |= (ushort)(buf[i] << (i * 8));
			}
			else
			{
				int shift = 0;
				for (int i = buf.Length - 1; i >= 0; i--)
				{
					value |= (ushort)(buf[i] << shift);
					shift += 8;
				}
			}
			return value;
		}

		protected override bool TryParseBinary(string value, out ushort result)
		{
			try
			{
				result = Convert.ToUInt16(value, 2);
				return true;
			}
			catch (Exception e) when (e is FormatException || e is OverflowException)
			{
				result = 0;
				return false;
			}
		}

		#endregion
	}
}
