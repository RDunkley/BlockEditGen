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
using BlockEditGen.Interfaces;
using BlockEditGen.Parse;
using System.Globalization;

namespace BlockEditGen.ViewModels
{
	/// <summary>
	///   Base class for all UI register or bit representations that are TextBox based.
	/// </summary>
	public abstract class StringBasedViewModelBase : DataViewModelBase
	{
		private string _errorString;

		/// <summary>
		///   Gets or sets the value string. This is what is binded to in the GUI, but should not be used by derived
		///   classes to update the string (call <see cref="UpdateValue"/> instead).
		/// </summary>
		public string ValueString
		{
			get
			{
				if (HasError)
					return _errorString;
				return GetString();
			}
			set
			{
				if (!TrySetString(value))
				{
					HasError = true;
					_errorString = value;
				}
				else
				{
					HasError = false;
				}

				OnPropertyChanged(nameof(ValueString));
				OnPropertyChanged(nameof(CurrentState));
			}
		}

		public string Units { get { return _value.Units; } }

		public StringBasedViewModelBase(Value value, ICachedRegisterBlock block)
			: base(value, block)
		{
			block.CacheChanged += Block_CacheChanged;
		}

		private void Block_CacheChanged(object sender, EventArgs e)
		{
			OnPropertyChanged(nameof(ValueString));
			OnPropertyChanged(nameof(CurrentState));
		}

		protected bool TryConvertValueToReg(Conv conv, string inputVal, out double regVal)
		{
			if (!double.TryParse(inputVal, CultureInfo.CurrentCulture, out regVal))
				return false;

			if (conv.Offset.HasValue)
				regVal -= conv.Offset.Value;
			if (conv.Gain.HasValue)
				regVal /= conv.Gain.Value;
			return true;
		}

		protected string ConvertRegToValue(Conv conv, double regVal)
		{
			if(conv.Gain.HasValue)
				regVal *= conv.Gain.Value;
			if(conv.Offset.HasValue)
				regVal += conv.Offset.Value;
			return regVal.ToString();
		}

		protected abstract string GetString();

		protected abstract bool TrySetString(string value);
	}
}
