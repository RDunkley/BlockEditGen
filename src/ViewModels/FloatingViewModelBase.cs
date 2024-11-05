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
	public abstract class FloatingViewModelBase<T> : StringBasedViewModelBase where T : struct, INumber<T>, IFloatingPoint<T>
	{
		protected readonly int _numPrecision;

		public int NumberOfBits { get { return (int)_value.Length.TotalBits; } }

		public FloatingViewModelBase(Value value, ICachedRegisterBlock block)
			: base(value, block)
		{
			if (value.Type != Value.TypeEnum.Float && value.Type != Value.TypeEnum.Double)
				throw new ArgumentException($"The value provided ({value.Name}) is not a floating type.");
			if (!int.TryParse(value.Subtype, out int _numPrecision))
				throw new ArgumentException($"The value provided ({value.Name}) is a {value.Type}, but the subtype ({value.Subtype}) can't be parsed to an integer to represent the number of precision bits.");
		}

		protected abstract void SetValue(T value);

		protected override bool TrySetString(string value)
		{
			if (!TryParse(value, out T result)) return false;

			SetValue(result);
			return true;
		}

		protected bool TryParse(string value, out T result)
		{
			value = value.Replace("_", string.Empty).ToLower();

			// Attempt to parse the number as just a float.
			return T.TryParse(value, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out result);
		}
	}
}
