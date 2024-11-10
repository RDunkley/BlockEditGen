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
	public class DoubleViewModel : FloatingViewModelBase<double>
	{
		private int _size;

		public DoubleViewModel(Value value, ICachedRegisterBlock block)
			: base(value, block)
		{
			_size = sizeof(double);
			if(value.Type != Value.TypeEnum.Double)
				throw new ArgumentException($"The value provided ({value.Name}) has an invalid type ({value.Type}) for this object.");
			if (value.Length.TotalBits != _size * 8)
				throw new ArgumentException($"The value provided ({value.Name}) has a bit width ({value.Length.TotalBits}) that does not equal a {value.Type} ({_size * 8}).");
		}

		protected override string GetString()
		{
			var ret = new byte[_size];
			_block.ReadSection(_value.Address, _value.Length, ret);
			var val = BitConverter.ToDouble(ret);

			if(_value.Conversion == null)
				return val.ToString($"F{_numPrecision}");

			// Value has a conversion so convert it.
			return ConvertRegToValue(_value.Conversion, val);
		}

		protected override void SetValue(double value)
		{
			_block.WriteSection(_value.Address, _value.Length, BitConverter.GetBytes(value));
		}
	}
}
