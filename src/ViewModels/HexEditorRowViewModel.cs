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
using Avalonia.Media;
using BlockEditGen.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace BlockEditGen.ViewModels
{
	public partial class HexEditorRowViewModel : ObservableObject
	{
		public string OffsetText { get; }

		public int FontSize { get; }

		/// <summary>
		///   Number of bytes making up one element. The nibbles of an element are shown as a group.
		/// </summary>
		public int ElementSizeInBytes { get; }

		public HexTextColumn TextColumn { get; }

		public bool ShowTextColumn => TextColumn != HexTextColumn.None;

		public bool IsReadOnly { get; }

		public int RowByteOffset { get; }

		public double OffsetColumnWidth { get; }

		public double TextColumnWidth { get; }

		public FontFamily HexFontFamily => PanelFactory.HexFontFamily;

		public int RowHeight => DataViewModelBase.GetRowHeight(FontSize);

		public ObservableCollection<HexEditorByteViewModel> Bytes { get; } = new();

		public string HexDisplayText
		{
			get => _hexDisplayText;
			private set => SetProperty(ref _hexDisplayText, value);
		}

		private string _hexDisplayText;

		public string TextColumnText
		{
			get => _textColumnText;
			private set => SetProperty(ref _textColumnText, value);
		}

		private string _textColumnText;

		public HexEditorRowViewModel(string offsetText, int rowByteOffset, int fontSize, int elementSizeInBytes,
			HexTextColumn textColumn, bool isReadOnly, double offsetColumnWidth, double textColumnWidth)
		{
			OffsetText = offsetText;
			RowByteOffset = rowByteOffset;
			FontSize = fontSize;
			ElementSizeInBytes = elementSizeInBytes;
			TextColumn = textColumn;
			IsReadOnly = isReadOnly;
			OffsetColumnWidth = offsetColumnWidth;
			TextColumnWidth = textColumnWidth;
		}

		internal void UpdateHexDisplayText()
		{
			if (Bytes.Count == 0)
			{
				HexDisplayText = string.Empty;
				return;
			}

			var sb = new StringBuilder((Bytes.Count * 2) + (Bytes.Count / ElementSizeInBytes));
			for (int i = 0; i < Bytes.Count; i++)
			{
				if (i > 0 && i % ElementSizeInBytes == 0)
					sb.Append(' ');
				sb.Append(Bytes[i].HexText);
			}

			HexDisplayText = sb.ToString();
		}

		internal void UpdateTextColumn()
		{
			switch (TextColumn)
			{
				case HexTextColumn.Ascii:
					TextColumnText = BuildAsciiText();
					break;
				case HexTextColumn.Utf32:
					TextColumnText = BuildUtf32Text();
					break;
				default:
					TextColumnText = string.Empty;
					break;
			}
		}

		private string BuildAsciiText()
		{
			var sb = new StringBuilder(Bytes.Count);
			foreach (var byteVm in Bytes)
			{
				byte b = byteVm.GetByteValue();
				sb.Append(b >= 32 && b <= 126 ? (char)b : '.');
			}
			return sb.ToString();
		}

		private string BuildUtf32Text()
		{
			const int bytesPerCodePoint = 4;

			var sb = new StringBuilder(Bytes.Count / bytesPerCodePoint);
			for (int i = 0; i + bytesPerCodePoint <= Bytes.Count; i += bytesPerCodePoint)
			{
				uint codePoint = 0;
				for (int b = 0; b < bytesPerCodePoint; b++)
					codePoint |= (uint)Bytes[i + b].GetByteValue() << (b * 8);
				sb.Append(GetPrintableCodePoint(codePoint));
			}
			return sb.ToString();
		}

		/// <summary>
		///   Renders a UTF-32 code point, falling back to a period for anything that has no sensible glyph. Surrogates
		///   and values past the end of the code space are not valid on their own, so those are filtered out too.
		/// </summary>
		private static string GetPrintableCodePoint(uint codePoint)
		{
			if (codePoint > int.MaxValue || !Rune.IsValid((int)codePoint))
				return ".";

			var rune = new Rune((int)codePoint);
			if (Rune.IsControl(rune) || Rune.GetUnicodeCategory(rune) == UnicodeCategory.OtherNotAssigned)
				return ".";

			return rune.ToString();
		}
	}
}
