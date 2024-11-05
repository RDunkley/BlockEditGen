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
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace BlockEditGen.Converters
{
	public class BooleanToStringConverter : IMultiValueConverter
	{
		public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Count != 3)
				throw new ArgumentException("Expected three bindings: two strings and one boolean.");
			if (values[0] is not string || values[1] is not string || values[2] is not bool)
				return string.Empty;

			string trueString = values[0] as string;
			string falseString = values[1] as string;
			bool condition = (bool)values[2];

			return condition ? trueString : falseString;
		}
	}
}
