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
using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;

namespace BlockEditGen.ViewModels
{
	public partial class HexEditorByteViewModel : ObservableObject
	{
		private readonly ArrayViewModel _parent;
		private string _hexText;

		public int ByteIndex { get; }

		public int FontSize => _parent.FontSize;

		public int RowHeight => _parent.RowHeight;

		public bool IsReadOnly => _parent.IsReadOnly;

		public string HexText
		{
			get => _hexText;
			set => SetHexText(value, commit: false);
		}

		public HexEditorByteViewModel(ArrayViewModel parent, int byteIndex, byte value)
		{
			_parent = parent;
			ByteIndex = byteIndex;
			_hexText = value.ToString("X2");
		}

		internal void Refresh(byte value)
		{
			_hexText = value.ToString("X2");
			OnPropertyChanged(nameof(HexText));
		}

		internal byte GetByteValue()
		{
			if (byte.TryParse(_hexText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte parsed))
				return parsed;
			return 0;
		}

		internal void SetHexFromByte(byte value)
		{
			_hexText = value.ToString("X2");
			OnPropertyChanged(nameof(HexText));
		}

		internal bool TryApplyNibble(char hexDigit, bool highNibble, out byte newValue)
		{
			newValue = 0;
			if (!Uri.IsHexDigit(hexDigit))
				return false;

			int nibble = Convert.ToInt32(hexDigit.ToString(), 16);
			byte current = GetByteValue();
			newValue = highNibble
				? (byte)((current & 0x0F) | (nibble << 4))
				: (byte)((current & 0xF0) | nibble);
			SetHexFromByte(newValue);
			return true;
		}

		internal void CommitEdit()
		{
			SetHexText(_hexText, commit: true);
		}

		private void SetHexText(string value, bool commit)
		{
			if (IsReadOnly)
				return;

			value = value?.Trim().ToUpperInvariant() ?? string.Empty;
			if (value.Length > 2)
				return;

			foreach (char c in value)
			{
				if (!Uri.IsHexDigit(c))
					return;
			}

			if (_hexText == value && !commit)
				return;

			_hexText = value;
			OnPropertyChanged(nameof(HexText));

			if (value.Length == 0)
			{
				if (commit)
				{
					_parent.WriteByte(ByteIndex, 0);
					_hexText = "00";
					OnPropertyChanged(nameof(HexText));
				}
				return;
			}

			if (value.Length == 1)
			{
				if (commit)
				{
					value = "0" + value;
					_hexText = value;
					OnPropertyChanged(nameof(HexText));
				}
				else
				{
					return;
				}
			}

			if (byte.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte parsed))
				_parent.WriteByte(ByteIndex, parsed);
		}
	}
}
