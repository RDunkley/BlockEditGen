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
using System.Numerics;

namespace BlockEditGen.ViewModels
{
	public abstract class UnsignedViewModelBase<T> : StringBasedViewModelBase where T : struct, INumber<T>, IUnsignedNumber<T>
	{
		public Endian ValueEndian { get; private set; } = Endian.Little;

		public T BitMask { get; private set; }

		public IntegerViewType ViewType { get; private set; }
		public int NumberOfBits { get { return (int)_value.Length.TotalBits; } }

		public UnsignedViewModelBase(Value value, ICachedRegisterBlock block)
			: base(value, block)
		{
			if (value.Type != Value.TypeEnum.Uint8 && value.Type != Value.TypeEnum.Uint16 && value.Type != Value.TypeEnum.Uint32 && value.Type != Value.TypeEnum.Uint64)
				throw new ArgumentException($"The value provided ({value.Name}) is not an unsigned type.");

			if(value.Subtype.Contains(','))
			{
				var splits = value.Subtype.Split(',', StringSplitOptions.RemoveEmptyEntries);
				if (splits.Length == 1)
				{
					ViewType = GetViewType(splits[0]);
				}
				else if (splits.Length == 2)
				{
					ViewType = GetViewType(splits[0]);
					ValueEndian = GetEndian(splits[1]);
				}
				else
				{
					throw new ArgumentException($"The value provided ({_value.Name}) is an unsigned type, but does not have a valid subtype ({_value.Subtype}). The format of the subtype must be <display type>,<endian> with <display type> as 'hex', 'bin', or 'num' and <endian> as 'be' or 'le'. <endian> is optional (when not included omit comma).");
				}
			}
			else
			{
				ViewType = GetViewType(value.Subtype);
			}

			BitMask = GetBitMask((int)value.Length.TotalBits);
		}

		private IntegerViewType GetViewType(string subTypeOption)
		{
			switch (subTypeOption)
			{
				case "hex":
					return IntegerViewType.Hexadecimal;
				case "bin":
					return IntegerViewType.Binary;
				case "num":
					return IntegerViewType.Number;
				default:
					throw new ArgumentException($"The value provided ({_value.Name}) is an unsigned type, but does not have a valid subtype ({_value.Subtype}). The first part of the subtype for unsigned types must be 'hex', 'bin', or 'num'.");
			}
		}

		private Endian GetEndian(string subTypeOption)
		{
			if (subTypeOption.ToLower() == "be")
				return Endian.Big;
			if (subTypeOption.ToLower() == "le")
				return Endian.Little;
			throw new ArgumentException($"The value provided ({_value.Name}) is an unsigned type, but does not have a valid subtype ({_value.Subtype}). The second part of the subtype for unsigned types must be 'be' or 'le' for big endian or little endian respectively.");
		}

		protected bool TryParse(string value, out T result)
		{
			value = value.Replace("_", string.Empty).ToLower();
			if (value.Length > 2 && value[0] == '0' && value[1] == 'x')
			{
				// Number is specified as a hexadecimal number (0xFF).
				return T.TryParse(value.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out result);
			}

			if (value.Length > 1 && char.ToLower(value[value.Length - 1]) == 'b')
			{
				// Number is a binary number.
				return TryParseBinary(value.Substring(0, value.Length - 1), out result);
			}

			// Attempt to parse the number as just an integer.
			return T.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out result);
		}

		#region Derived Classes

		protected abstract T GetBitMask(int numBits);

		protected abstract byte[] ValueToBytes(T value);

		protected abstract string ValueToString(T value);

		protected abstract T BytesToValue(byte[] buf);

		protected abstract bool TryParseBinary(string value, out T result);

		#endregion

		#region StringBasedViewModelBase

		protected override string GetString()
		{
			var buf = new byte[LengthInBytes];
			_block.ReadSection(_value.Address, _value.Length, buf);
			T val = BytesToValue(buf);
			return ValueToString(val);
		}

		protected override bool TrySetString(string value)
		{
			if (!TryParse(value, out T result)) return false;
			if (result > BitMask) return false;

			var buf = ValueToBytes(result);
			_block.WriteSection(_value.Address, _value.Length, buf);
			return true;
		}

		#endregion
	}
}
