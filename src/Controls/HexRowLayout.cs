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
using Avalonia.Controls;
using BlockEditGen.ViewModels;

namespace BlockEditGen.Controls
{
	/// <summary>
	///   Maps between caret positions in a row's hex text and the nibbles they sit on.
	/// </summary>
	/// <remarks>
	///   The nibbles of an element are written together with no separator and a single space follows each element, so
	///   the caret stride depends on the element size. A byte array reads "1C 23 2A", while a ushort array covering the
	///   same memory reads "1C23 2A31".
	/// </remarks>
	internal readonly struct HexRowLayout
	{
		private readonly int _byteCount;
		private readonly int _bytesPerElement;

		public HexRowLayout(int byteCount, int bytesPerElement)
		{
			_byteCount = byteCount;
			_bytesPerElement = Math.Max(1, bytesPerElement);
		}

		public HexRowLayout(HexEditorRowViewModel row)
			: this(row.Bytes.Count, row.ElementSizeInBytes)
		{
		}

		/// <summary>
		///   Characters an element occupies, counting the space that separates it from the next one.
		/// </summary>
		private int ElementStride => (_bytesPerElement * 2) + 1;

		/// <summary>
		///   Caret position of the last nibble on the row, which is the furthest right the caret may sit.
		/// </summary>
		private int LastCaret => _byteCount == 0 ? 0 : GetCaretForByteNibble(_byteCount - 1, highNibble: false);

		public int GetCaretForByteNibble(int byteIndexInRow, bool highNibble)
		{
			int element = byteIndexInRow / _bytesPerElement;
			int byteInElement = byteIndexInRow % _bytesPerElement;
			return (element * ElementStride) + (byteInElement * 2) + (highNibble ? 0 : 1);
		}

		public bool TryMapCaretToNibble(int caretIndex, out int byteIndexInRow, out bool highNibble)
		{
			byteIndexInRow = -1;
			highNibble = false;

			if (caretIndex < 0)
				return false;

			int offsetInElement = caretIndex % ElementStride;
			if (offsetInElement == _bytesPerElement * 2)
				return false; // Sitting on the separator between two elements.

			int index = ((caretIndex / ElementStride) * _bytesPerElement) + (offsetInElement / 2);
			if (index >= _byteCount)
				return false;

			byteIndexInRow = index;
			highNibble = offsetInElement % 2 == 0;
			return true;
		}

		public int GetNextCaretAfterNibble(int byteIndexInRow, bool highNibble)
		{
			if (highNibble)
				return GetCaretForByteNibble(byteIndexInRow, highNibble: false);

			if (byteIndexInRow + 1 < _byteCount)
				return GetCaretForByteNibble(byteIndexInRow + 1, highNibble: true);

			return GetCaretForByteNibble(byteIndexInRow, highNibble: false) + 1;
		}

		public int GetNextCaret(int caretIndex)
		{
			if (TryMapCaretToNibble(caretIndex, out int byteIndexInRow, out bool highNibble))
				return GetNextCaretAfterNibble(byteIndexInRow, highNibble);

			return SnapCaretToNibble(caretIndex + 1);
		}

		public int GetPreviousCaret(int caretIndex)
		{
			if (caretIndex <= 0)
				return 0;

			if (TryMapCaretToNibble(caretIndex, out int byteIndexInRow, out bool highNibble))
			{
				if (!highNibble)
					return GetCaretForByteNibble(byteIndexInRow, highNibble: true);

				if (byteIndexInRow > 0)
					return GetCaretForByteNibble(byteIndexInRow - 1, highNibble: false);
			}

			return Math.Max(0, caretIndex - 1);
		}

		public int SnapCaretToNibble(int caretIndex)
		{
			if (_byteCount == 0)
				return 0;

			if (TryMapCaretToNibble(caretIndex, out _, out _))
				return caretIndex;

			if (caretIndex <= 0)
				return 0;

			int lastCaret = LastCaret;
			if (caretIndex >= lastCaret)
				return lastCaret;

			// The only remaining miss inside the row is a separator, so step onto the next element.
			return Math.Min(caretIndex + 1, lastCaret);
		}

		public void SelectBytePair(TextBox textBox, int byteIndexInRow, bool highNibble)
		{
			int start = GetCaretForByteNibble(byteIndexInRow, highNibble: true);
			int end = start + 2;
			textBox.SelectionStart = start;
			textBox.SelectionEnd = end;
			textBox.CaretIndex = highNibble ? start : Math.Min(start + 1, end);
		}
	}
}
