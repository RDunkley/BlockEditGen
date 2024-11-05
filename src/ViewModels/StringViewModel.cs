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
using System.Text;

namespace BlockEditGen.ViewModels
{
	public class StringViewModel : StringBasedViewModelBase
	{
		public int MaxNumCharacters { get; private set; }

		public Encoding Encoding { get; private set; }

		public StringViewModel()
			: this(
				new Value(Value.AccessEnum.ReadWrite, "0x00", null, "Test description of string", "Test Name", "6", "UTF8", Value.TypeEnum.String, null),
				new CachedRegisterBlock<byte>(new RamRegisterBlock<byte>(4096))
			)
		{
		}

		public StringViewModel(Value value, ICachedRegisterBlock block)
			: base(value, block)
		{
			if (value.Type != Value.TypeEnum.String)
				throw new ArgumentException($"The value provided ({value.Name}) is not a string type.");
			if (value.Length.Bits != 0)
				throw new ArgumentException($"The value provided ({value.Name}) is a string, but does not end on a byte boundary.");

			switch(value.Subtype.ToLower())
			{
				case "ascii":
					Encoding = Encoding.ASCII;
					MaxNumCharacters = value.Length.Bytes;
					break;
				case "utf8":
					Encoding = Encoding.UTF8;
					MaxNumCharacters = value.Length.Bytes;
					break;
				case "unicode":
					Encoding = Encoding.Unicode;
					MaxNumCharacters = value.Length.Bytes / 2;
					break;
				default:
					throw new ArgumentException($"The value provided ({value.Name}) is a string, but the subtype ({value.Subtype}) does not reflect the encoding (must be 'ASCII', 'UTF8', or 'Unicode').");
			}

			if (Encoding == Encoding.Unicode && (value.Length.Bytes < 2 || value.Length.Bytes % 2 != 0))
				throw new ArgumentException($"The value provided ({value.Name}) is a string, but the encoding is 'unicode' and length is not a multiple of 2.");
		}

		protected override string GetString()
		{
			var buf = new byte[_value.Length.Bytes];
			_block.ReadSection(_value.Address, _value.Length, buf);
			var fullString = Encoding.GetString(buf);

			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < fullString.Length; i++)
			{
				if (fullString[i] == '\0')
					break;
				sb.Append(fullString[i]);
			}
			return sb.ToString();
		}

		protected override bool TrySetString(string value)
		{
			if (value == null) return false;
			if (value.Length > MaxNumCharacters) return false;

			var buf = new byte[_value.Length.Bytes];
			Encoding.GetBytes(value, 0, value.Length, buf, 0);
			_block.WriteSection(_value.Address, _value.Length, buf);
			return true;
		}

		protected char ConvertCharacter(byte[] buf, ref int offset)
		{
			int inc = 1;
			if (Encoding == Encoding.Unicode)
				inc = 2;
			var chars = Encoding.GetChars(buf, offset, inc);
			offset += inc;
			return chars[0];
		}
	}
}
