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
using System.Globalization;
using System.Net;
using System.Text;

namespace BlockEditGen.ViewModels
{
	public class MacViewModel : StringBasedViewModelBase
	{
		public MacViewModel()
			: this(
				new Value(Value.AccessEnum.ReadWrite, "0x00", null, "Test description of MAC", "Test Name", "6", null, Value.TypeEnum.Mac, null),
				new CachedRegisterBlock<byte>(new RamRegisterBlock<byte>(4096))
			)
		{
		}

		public MacViewModel(Value value, ICachedRegisterBlock block)
			: base(value, block)
		{
			if (value.Type != Value.TypeEnum.Mac)
				throw new ArgumentException($"The value provided ({value.Name}) is not a MAC type.");

			if (!string.IsNullOrEmpty(value.Subtype))
				throw new ArgumentException($"The value provided ({value.Name}) is a {value.Type}, but the subtype contained a value ({value.Subtype}). The subtype should not be provided or be empty.");

			if (value.Length.TotalBits != 48)
				throw new ArgumentException($"The value provided ({value.Name}) is specified as a MAC, but the length of the value isn't 48 bits.");
		}

		protected override string GetString()
		{
			var buf = new byte[LengthInBytes];
			_block.ReadSection(_value.Address, _value.Length, buf);
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < buf.Length; i++)
			{
				sb.Append(buf[i].ToString("X2").ToUpper());
				if(i != buf.Length - 1)
					sb.Append(":");
			}
			return sb.ToString();
		}

		protected override bool TrySetString(string value)
		{
			if (value == null) return false;
			value = value.Replace(":", string.Empty); // Remove colons.
			if (value.Length != 12) return false;

			var buf = new byte[LengthInBytes];
			for(int i = 0; i < LengthInBytes; i++)
			{
				if (!byte.TryParse(value.Substring(i*2, 2), NumberStyles.HexNumber, CultureInfo.CurrentCulture, out byte byteVal))
					return false;
				buf[i] = byteVal;
			}
			_block.WriteSection(_value.Address, _value.Length, buf);
			return true;
		}
	}
}
