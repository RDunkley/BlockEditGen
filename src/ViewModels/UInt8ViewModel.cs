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
	public class UInt8ViewModel : UnsignedViewModelBase<byte>
	{
		public UInt8ViewModel(Value value, ICachedRegisterBlock block)
			: base(value, block)
		{
			var size = sizeof(byte);
			if (value.Type != Value.TypeEnum.Uint8)
				throw new ArgumentException($"The value provided ({value.Name}) has an invalid type ({value.Type}) for this object.");
			if (value.Length.TotalBits > size * 8)
				throw new ArgumentException($"The value provided ({value.Name}) has a bit width ({value.Length.TotalBits}) longer than a {value.Type} ({size * 8}).");
		}

		#region UnsignedViewModelBase

		protected override byte GetBitMask(int numBits)
		{
			return (byte)GenerateBitMask(numBits);
		}

		protected override byte[] ValueToBytes(byte value)
		{
			var buf = new byte[LengthInBytes];
			buf[0] = value;
			return buf;
		}

		protected override string ValueToString(byte value)
		{
			if (ViewType == IntegerViewType.Binary)
				return $"{Convert.ToString(value, 2).PadLeft(NumberOfBits, '0')}b";
			if (ViewType == IntegerViewType.Hexadecimal)
				return $"0x{value.ToString($"X{(NumberOfBits + 3) / 4}")}";
			return $"{value:N0}";
		}

		protected override byte BytesToValue(byte[] buf)
		{
			return buf[0];
		}

		protected override bool TryParseBinary(string value, out byte result)
		{
			try
			{
				result = Convert.ToByte(value, 2);
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
