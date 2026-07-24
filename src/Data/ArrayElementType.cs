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
namespace BlockEditGen.Data
{
	/// <summary>
	///   Unsigned integer element types supported by the <c>array</c> value type.
	/// </summary>
	public enum ArrayElementType
	{
		Byte = 1,
		UShort = 2,
		UInt = 4,
		ULong = 8,
	}

	/// <summary>
	///   How the character column alongside the hex editor interprets a row's bytes.
	/// </summary>
	public enum HexTextColumn
	{
		/// <summary>No character column is shown.</summary>
		None,

		/// <summary>One character per byte, limited to the printable ASCII range.</summary>
		Ascii,

		/// <summary>
		///   One character per four byte element, read little endian and treated as a UTF-32 code point. Note that this
		///   is not the same as the <c>Unicode</c> string subtype, which is UTF-16.
		/// </summary>
		Utf32,
	}

	public static class ArrayElementTypeExtensions
	{
		/// <summary>
		///   Gets the character column that suits an element of this size. A byte maps onto a single ASCII character and
		///   a 32-bit element covers the whole Unicode code point range, so those are the two that get a column.
		/// </summary>
		public static HexTextColumn TextColumn(this ArrayElementType elementType)
		{
			return elementType switch
			{
				ArrayElementType.Byte => HexTextColumn.Ascii,
				ArrayElementType.UInt => HexTextColumn.Utf32,
				_ => HexTextColumn.None,
			};
		}

		public static bool TryParse(string subtype, out ArrayElementType elementType)
		{
			elementType = default;
			if (string.IsNullOrEmpty(subtype))
				return false;

			switch (subtype.ToLowerInvariant())
			{
				case "byte":
					elementType = ArrayElementType.Byte;
					return true;
				case "ushort":
					elementType = ArrayElementType.UShort;
					return true;
				case "uint":
					elementType = ArrayElementType.UInt;
					return true;
				case "ulong":
					elementType = ArrayElementType.ULong;
					return true;
				default:
					return false;
			}
		}

		public static int SizeInBytes(this ArrayElementType elementType)
		{
			return (int)elementType;
		}

		public static string ToSubtypeString(this ArrayElementType elementType)
		{
			return elementType switch
			{
				ArrayElementType.Byte => "byte",
				ArrayElementType.UShort => "ushort",
				ArrayElementType.UInt => "uint",
				ArrayElementType.ULong => "ulong",
				_ => throw new ArgumentOutOfRangeException(nameof(elementType)),
			};
		}
	}
}
