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
using BlockEditGen.Interfaces;
using BlockEditGen.Parse;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BlockEditGen.ViewModels
{
	public partial class ArrayViewModel : DataViewModelBase
	{
		/// <summary>
		///   Number of bytes shown on a row before the control has been measured. Replaced by a width driven value once
		///   the control reports its size.
		/// </summary>
		private const int DefaultBytesPerRow = 16;

		/// <summary>
		///   Rows always hold a whole number of these so an element never straddles two rows. Byte arrays still step in
		///   groups of four to keep the columns readable.
		/// </summary>
		private const int MinimumRowGranularity = 4;

		public FontFamily HexFontFamily => PanelFactory.HexFontFamily;

		private readonly ArrayElementType _elementType;
		private readonly int _elementSizeInBytes;
		private bool _isRefreshing;
		private int _editSuspendCount;

		public bool IsEditing => _editSuspendCount > 0;

		public ObservableCollection<HexEditorRowViewModel> Rows { get; } = new();

		public int ElementSizeInBytes => _elementSizeInBytes;

		public HexTextColumn TextColumn => _elementType.TextColumn();

		public bool ShowTextColumn => TextColumn != HexTextColumn.None;

		public string TextColumnHeader => TextColumn switch
		{
			HexTextColumn.Ascii => "ASCII",
			HexTextColumn.Utf32 => "UTF-32",
			_ => string.Empty,
		};

		/// <summary>
		///   Character cells the hex column needs for each byte on a row. Two nibbles per byte, plus the share of the
		///   single space that separates one element from the next.
		/// </summary>
		public double HexColumnWidthPerByte => 2.0 + (1.0 / _elementSizeInBytes);

		/// <summary>
		///   Character cells the text column needs for each byte on a row. ASCII shows one character per byte, while
		///   UTF-32 shows one character per four bytes but is given two cells, as many code points render double width.
		/// </summary>
		public double TextColumnWidthPerByte => TextColumn switch
		{
			HexTextColumn.Ascii => 1.0,
			HexTextColumn.Utf32 => 0.5,
			_ => 0.0,
		};

		public int BytesPerRow { get; private set; } = DefaultBytesPerRow;

		public double OffsetColumnWidth { get; private set; } = double.NaN;

		public double TextColumnWidth { get; private set; } = double.NaN;

		public ArrayViewModel()
			: this(
				new Value(Value.AccessEnum.ReadWrite, "0x50", null, "Test array description", "Test Array", "16", "byte", Value.TypeEnum.Array, null),
				new CachedRegisterBlock<byte>(new RamRegisterBlock<byte>(4096))
			)
		{
		}

		public ArrayViewModel(Value value, ICachedRegisterBlock block)
			: base(value, block)
		{
			if (value.Type != Value.TypeEnum.Array)
				throw new ArgumentException($"The value provided ({value.Name}) is not an array type.");

			if (!ArrayElementTypeExtensions.TryParse(value.Subtype, out _elementType))
				throw new ArgumentException($"The value provided ({value.Name}) has an unsupported array subtype ({value.Subtype}).");

			_elementSizeInBytes = _elementType.SizeInBytes();

			if (value.Length.Bits != 0)
				throw new ArgumentException($"The value provided ({value.Name}) must be byte-aligned.");

			if (LengthInBytes % _elementSizeInBytes != 0)
				throw new ArgumentException($"The value provided ({value.Name}) size is not a multiple of the element size.");

			BuildRows();
			block.CacheChanged += Block_CacheChanged;
		}

		/// <summary>
		///   Rounds <paramref name="bytesPerRow"/> down so a row holds whole elements, and keeps it within the bounds of
		///   the array.
		/// </summary>
		internal int ClampBytesPerRow(int bytesPerRow)
		{
			int granularity = Math.Max(_elementSizeInBytes, MinimumRowGranularity);
			if (granularity % _elementSizeInBytes != 0)
				granularity = _elementSizeInBytes;

			int clamped = bytesPerRow - (bytesPerRow % granularity);
			if (clamped < granularity)
				clamped = granularity;
			if (clamped > LengthInBytes)
				clamped = LengthInBytes;
			return clamped;
		}

		/// <summary>
		///   Applies a width driven layout, rebuilding the rows when the number of bytes on a row changes.
		/// </summary>
		internal void UpdateRowLayout(int bytesPerRow, double offsetColumnWidth, double textColumnWidth)
		{
			if (bytesPerRow < 1)
				return;

			bool sameLayout = bytesPerRow == BytesPerRow
				&& offsetColumnWidth.Equals(OffsetColumnWidth)
				&& textColumnWidth.Equals(TextColumnWidth);
			if (sameLayout)
				return;

			BytesPerRow = bytesPerRow;
			OffsetColumnWidth = offsetColumnWidth;
			TextColumnWidth = textColumnWidth;

			OnPropertyChanged(nameof(BytesPerRow));
			OnPropertyChanged(nameof(OffsetColumnWidth));
			OnPropertyChanged(nameof(TextColumnWidth));

			BuildRows();
		}

		internal void BeginEdit()
		{
			_editSuspendCount++;
		}

		internal void EndEdit()
		{
			if (_editSuspendCount > 0)
				_editSuspendCount--;
		}

		internal void WriteByte(int byteIndex, byte value)
		{
			if (IsReadOnly || byteIndex < 0 || byteIndex >= LengthInBytes)
				return;

			var address = new ByteBitValue(_value.Address.Bytes + byteIndex, 0);
			var length = new ByteBitValue(8);
			var buf = new byte[] { value };
			_block.WriteSection(address, length, buf);

			var row = Rows[byteIndex / BytesPerRow];
			row.Bytes[byteIndex % BytesPerRow].SetHexFromByte(value);
			row.UpdateHexDisplayText();
			row.UpdateTextColumn();
			OnPropertyChanged(nameof(CurrentState));
		}

		private void BuildRows()
		{
			Rows.Clear();
			var data = new byte[LengthInBytes];
			_block.ReadSection(_value.Address, _value.Length, data);

			int rowCount = (LengthInBytes + BytesPerRow - 1) / BytesPerRow;
			for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
			{
				int rowOffset = rowIndex * BytesPerRow;
				var row = new HexEditorRowViewModel(rowOffset.ToString("X8"), rowOffset, FontSize, _elementSizeInBytes,
					TextColumn, IsReadOnly, OffsetColumnWidth, TextColumnWidth);
				int bytesInRow = Math.Min(BytesPerRow, LengthInBytes - rowOffset);

				for (int i = 0; i < bytesInRow; i++)
				{
					int byteIndex = rowOffset + i;
					row.Bytes.Add(new HexEditorByteViewModel(this, byteIndex, data[byteIndex]));
				}

				row.UpdateTextColumn();
				row.UpdateHexDisplayText();
				Rows.Add(row);
			}
		}

		private void RefreshFromCache()
		{
			_isRefreshing = true;
			try
			{
				var data = new byte[LengthInBytes];
				_block.ReadSection(_value.Address, _value.Length, data);

				foreach (var row in Rows)
				{
					foreach (var byteVm in row.Bytes)
						byteVm.Refresh(data[byteVm.ByteIndex]);
					row.UpdateHexDisplayText();
					row.UpdateTextColumn();
				}
			}
			finally
			{
				_isRefreshing = false;
			}

			OnPropertyChanged(nameof(CurrentState));
		}

		private void Block_CacheChanged(object sender, EventArgs e)
		{
			if (_isRefreshing || IsEditing)
				return;
			RefreshFromCache();
		}
	}
}
