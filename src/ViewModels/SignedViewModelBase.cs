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
	public abstract class SignedViewModelBase<T> : StringBasedViewModelBase where T : struct, INumber<T>, ISignedNumber<T>
	{
		public Endian ValueEndian { get; private set; } = Endian.Little;

		public T BitMask { get; private set; }

		public T MaxValue { get; private set; }

		public T MinValue { get; private set; }

		public int NumberOfBits { get { return (int)_value.Length.TotalBits; } }

		public SignedViewModelBase(Value value, ICachedRegisterBlock block)
			: base(value, block)
		{
			if (value.Type != Value.TypeEnum.Int8 && value.Type != Value.TypeEnum.Int16 && value.Type != Value.TypeEnum.Int32 && value.Type != Value.TypeEnum.Int64)
				throw new ArgumentException($"The value provided ({value.Name}) is not a signed type.");

			if(!string.IsNullOrEmpty(value.Subtype))
			{
				if (value.Subtype.ToLower() == "be")
					ValueEndian = Endian.Big;
				else if (value.Subtype.ToLower() == "le")
					ValueEndian = Endian.Little;
				else
					throw new ArgumentException($"The signed value ({value.Name}) subtype ({value.Subtype}) is not valid. It should be 'be' or 'le' for big endian or little endian respectively.");
			}

			BitMask = GetBitMask(value.Length.TotalBits);
			MaxValue = GetMaxValue(value.Length.TotalBits);
			MinValue = GetMinValue(value.Length.TotalBits);
		}

		#region Derived Classes

		protected abstract T GetBitMask(int numBits);

		protected abstract T GetMaxValue(int numBits);

		protected abstract T GetMinValue(int numBits);

		protected abstract byte[] ValueToBytes(T value);
		protected abstract T BytesToValue(byte[] buf);

		#endregion

		#region StringBasedViewModelBase

		protected override string GetString()
		{
			var buf = new byte[LengthInBytes];
			_block.ReadSection(_value.Address, _value.Length, buf);
			T val = BytesToValue(buf);
			return $"{val:N0}";
		}

		protected override bool TrySetString(string value)
		{
			value = value.Replace("_", string.Empty).ToLower();
			if (!T.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out T result)) return false;
			if (result > MaxValue) return false;
			if (result < MinValue) return false;

			var buf = ValueToBytes(result);
			_block.WriteSection(_value.Address, _value.Length, buf);
			return true;
		}

		#endregion
	}
}
