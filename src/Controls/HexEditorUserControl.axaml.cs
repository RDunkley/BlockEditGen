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
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using BlockEditGen.ViewModels;
using System.Globalization;

namespace BlockEditGen.Controls;

public partial class HexEditorUserControl : UserControl
{
	/// <summary>
	///   Space placed to the left of the hex and ASCII columns. Must match the margins used in the XAML.
	/// </summary>
	private const double ColumnGap = 16.0;

	/// <summary>
	///   Number of characters in a row's offset label.
	/// </summary>
	private const int OffsetCharacterCount = 8;

	private bool _isUpdatingHexText;

	public double NameWidth
	{
		get => nameText.Width;
		set => nameText.Width = value;
	}

	public HexEditorUserControl()
	{
		InitializeComponent();
	}

	private ArrayViewModel GetArrayViewModel() => DataContext as ArrayViewModel;

	private void HexPanel_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		if (GetArrayViewModel() is not ArrayViewModel arrayVm)
			return;

		// Reflowing would tear down the row that is being typed into, so leave the layout alone until the edit ends.
		if (arrayVm.IsEditing)
			return;

		double available = hexPanel.Bounds.Width;
		if (double.IsNaN(available) || available <= 0)
			return;

		double charWidth = MeasureCharacterWidth(arrayVm.FontSize);
		if (charWidth <= 0)
			return;

		double offsetWidth = charWidth * OffsetCharacterCount;
		double budget = available - offsetWidth - ColumnGap;
		if (arrayVm.ShowTextColumn)
			budget -= ColumnGap;

		// The final element on a row has no trailing space, so one character's worth is added back to the budget.
		double widthPerByte = charWidth * (arrayVm.HexColumnWidthPerByte + arrayVm.TextColumnWidthPerByte);
		int bytesPerRow = arrayVm.ClampBytesPerRow((int)Math.Floor((budget + charWidth) / widthPerByte));

		double textWidth = arrayVm.ShowTextColumn
			? charWidth * arrayVm.TextColumnWidthPerByte * bytesPerRow
			: double.NaN;
		arrayVm.UpdateRowLayout(bytesPerRow, offsetWidth, textWidth);
	}

	private static double MeasureCharacterWidth(int fontSize)
	{
		var typeface = new Typeface(PanelFactory.HexFontFamily);
		var sample = new string('0', OffsetCharacterCount);
		var formatted = new FormattedText(sample, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, fontSize, Brushes.Black);
		return formatted.WidthIncludingTrailingWhitespace / OffsetCharacterCount;
	}

	private void HexRow_GotFocus(object sender, GotFocusEventArgs e)
	{
		if (sender is not TextBox textBox || textBox.DataContext is not HexEditorRowViewModel row)
			return;

		GetArrayViewModel()?.BeginEdit();

		var layout = new HexRowLayout(row);
		int caret = layout.SnapCaretToNibble(textBox.CaretIndex);
		if (!layout.TryMapCaretToNibble(caret, out int byteIndexInRow, out bool highNibble))
		{
			byteIndexInRow = 0;
			highNibble = true;
		}

		SetHexRowText(textBox, row.HexDisplayText);
		layout.SelectBytePair(textBox, byteIndexInRow, highNibble);
	}

	private void HexRow_LostFocus(object sender, RoutedEventArgs e)
	{
		if (sender is not TextBox textBox || textBox.DataContext is not HexEditorRowViewModel row)
			return;

		SetHexRowText(textBox, row.HexDisplayText);
		GetArrayViewModel()?.EndEdit();

		// Any resize that arrived while the row was being edited was ignored, so re-check the layout now.
		HexPanel_SizeChanged(hexPanel, null);
	}

	private void HexRow_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (_isUpdatingHexText)
			return;

		if (sender is not TextBox textBox || textBox.DataContext is not HexEditorRowViewModel row)
			return;

		var layout = new HexRowLayout(row);
		int caret = layout.SnapCaretToNibble(textBox.CaretIndex);
		if (!layout.TryMapCaretToNibble(caret, out int byteIndexInRow, out bool highNibble))
		{
			byteIndexInRow = 0;
			highNibble = true;
		}

		SetHexRowText(textBox, row.HexDisplayText);
		layout.SelectBytePair(textBox, byteIndexInRow, highNibble);
	}

	private void HexRow_PointerPressed(object sender, PointerPressedEventArgs e)
	{
		if (sender is not TextBox textBox || textBox.DataContext is not HexEditorRowViewModel row)
			return;

		textBox.Focus();

		var layout = new HexRowLayout(row);
		int caret = layout.SnapCaretToNibble(GetCaretIndexFromPointer(textBox, layout, e));
		if (!layout.TryMapCaretToNibble(caret, out int byteIndexInRow, out bool highNibble))
		{
			byteIndexInRow = 0;
			highNibble = true;
		}

		layout.SelectBytePair(textBox, byteIndexInRow, highNibble);
		e.Handled = true;
	}

	private void HexRow_TextInput(object sender, TextInputEventArgs e)
	{
		if (sender is not TextBox textBox || textBox.DataContext is not HexEditorRowViewModel row)
			return;

		if (string.IsNullOrEmpty(e.Text) || e.Text.Length != 1 || !Uri.IsHexDigit(e.Text[0]))
		{
			e.Handled = true;
			return;
		}

		e.Handled = true;
		ApplyHexDigit(textBox, row, char.ToUpperInvariant(e.Text[0]));
	}

	private void HexRow_KeyDown(object sender, KeyEventArgs e)
	{
		if (GetArrayViewModel() is not { IsReadOnly: false } arrayVm)
			return;

		if (sender is not TextBox textBox || textBox.DataContext is not HexEditorRowViewModel row)
			return;

		if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
		{
			if (e.Key == Key.C || e.Key == Key.A || e.Key == Key.Insert)
				return;
		}

		if (TryGetHexDigitFromKey(e.Key, out char digit))
		{
			e.Handled = true;
			ApplyHexDigit(textBox, row, digit);
			return;
		}

		var layout = new HexRowLayout(row);

		if (e.Key == Key.Back)
		{
			e.Handled = true;
			int previousCaret = layout.GetPreviousCaret(textBox.CaretIndex);

			if (layout.TryMapCaretToNibble(previousCaret, out int byteIndexInRow, out bool highNibble))
			{
				var byteVm = row.Bytes[byteIndexInRow];
				byte cleared = highNibble ? (byte)(byteVm.GetByteValue() & 0x0F) : (byte)(byteVm.GetByteValue() & 0xF0);
				byteVm.SetHexFromByte(cleared);
				arrayVm.WriteByte(byteVm.ByteIndex, cleared);
				row.UpdateHexDisplayText();
				SetHexRowText(textBox, row.HexDisplayText);
				layout.SelectBytePair(textBox, byteIndexInRow, highNibble);
			}
			return;
		}

		if (e.Key == Key.Left || e.Key == Key.Right)
		{
			int caret = textBox.CaretIndex;
			int newCaret = e.Key == Key.Left ? layout.GetPreviousCaret(caret) : layout.GetNextCaret(caret);

			if (layout.TryMapCaretToNibble(newCaret, out int byteIndexInRow, out bool highNibble))
			{
				layout.SelectBytePair(textBox, byteIndexInRow, highNibble);
				e.Handled = true;
			}
			return;
		}

		if (e.Key == Key.Tab)
		{
			e.Handled = true;
			FocusAdjacentRow(row, forward: !e.KeyModifiers.HasFlag(KeyModifiers.Shift));
			return;
		}

		if (e.Key == Key.Delete || e.Key == Key.Insert || e.Key == Key.Space)
		{
			e.Handled = true;
			return;
		}

		if (IsBlockedTextKey(e.Key))
			e.Handled = true;
	}

	private void ApplyHexDigit(TextBox textBox, HexEditorRowViewModel row, char digit)
	{
		if (GetArrayViewModel() is not { IsReadOnly: false } arrayVm)
			return;

		var layout = new HexRowLayout(row);
		if (!layout.TryMapCaretToNibble(textBox.CaretIndex, out int byteIndexInRow, out bool highNibble))
		{
			byteIndexInRow = 0;
			highNibble = true;
		}

		var byteVm = row.Bytes[byteIndexInRow];
		if (!byteVm.TryApplyNibble(digit, highNibble, out byte newValue))
			return;

		arrayVm.WriteByte(byteVm.ByteIndex, newValue);
		row.UpdateHexDisplayText();

		int nextCaret = layout.GetNextCaretAfterNibble(byteIndexInRow, highNibble);
		SetHexRowText(textBox, row.HexDisplayText);

		if (layout.TryMapCaretToNibble(nextCaret, out int nextByteIndex, out bool nextHighNibble))
			layout.SelectBytePair(textBox, nextByteIndex, nextHighNibble);
		else
			textBox.CaretIndex = nextCaret;
	}

	private void SetHexRowText(TextBox textBox, string text)
	{
		_isUpdatingHexText = true;
		try
		{
			textBox.Text = text;
		}
		finally
		{
			_isUpdatingHexText = false;
		}
	}

	private static bool TryGetHexDigitFromKey(Key key, out char digit)
	{
		digit = default;
		if (key >= Key.D0 && key <= Key.D9)
		{
			digit = (char)('0' + (key - Key.D0));
			return true;
		}

		if (key >= Key.NumPad0 && key <= Key.NumPad9)
		{
			digit = (char)('0' + (key - Key.NumPad0));
			return true;
		}

		if (key >= Key.A && key <= Key.F)
		{
			digit = (char)('A' + (key - Key.A));
			return true;
		}

		return false;
	}

	private static bool IsBlockedTextKey(Key key)
	{
		return (key >= Key.A && key <= Key.Z) ||
			(key >= Key.D0 && key <= Key.D9) ||
			(key >= Key.NumPad0 && key <= Key.NumPad9) ||
			key == Key.OemComma || key == Key.OemPeriod;
	}

	private void FocusAdjacentRow(HexEditorRowViewModel currentRow, bool forward)
	{
		if (GetArrayViewModel() is not ArrayViewModel arrayVm)
			return;

		int currentRowIndex = arrayVm.Rows.IndexOf(currentRow);
		if (currentRowIndex < 0)
			return;

		int nextRowIndex = forward ? currentRowIndex + 1 : currentRowIndex - 1;
		if (nextRowIndex < 0 || nextRowIndex >= arrayVm.Rows.Count)
			return;

		var nextRow = arrayVm.Rows[nextRowIndex];
		var nextTextBox = FindHexRowTextBox(nextRow);
		if (nextTextBox == null)
			return;

		nextTextBox.Focus();
		int byteIndex = forward ? 0 : nextRow.Bytes.Count - 1;
		new HexRowLayout(nextRow).SelectBytePair(nextTextBox, byteIndex, highNibble: forward);
	}

	private static int GetCaretIndexFromPointer(TextBox textBox, HexRowLayout layout, PointerPressedEventArgs e)
	{
		var position = e.GetPosition(textBox);
		if (string.IsNullOrEmpty(textBox.Text))
			return 0;

		var typeface = new Typeface(textBox.FontFamily ?? FontFamily.Default);
		double x = position.X;
		int bestCaret = 0;
		double bestDistance = double.MaxValue;

		for (int caret = 0; caret <= textBox.Text.Length; caret++)
		{
			string prefix = textBox.Text.Substring(0, Math.Min(caret, textBox.Text.Length));
			var formatted = new FormattedText(prefix, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, textBox.FontSize, Brushes.Black);
			double distance = Math.Abs(formatted.WidthIncludingTrailingWhitespace - x);
			if (distance < bestDistance)
			{
				bestDistance = distance;
				bestCaret = caret;
			}
		}

		return layout.SnapCaretToNibble(bestCaret);
	}

	private TextBox FindHexRowTextBox(HexEditorRowViewModel rowVm)
	{
		return FindHexRowTextBoxRecursive(hexPanel, rowVm);
	}

	private static TextBox FindHexRowTextBoxRecursive(Control parent, HexEditorRowViewModel rowVm)
	{
		if (parent is TextBox textBox && textBox.DataContext == rowVm && textBox.Classes.Contains("hexRow"))
			return textBox;

		if (parent is Panel panel)
		{
			foreach (var child in panel.Children)
			{
				if (child is Control control)
				{
					var found = FindHexRowTextBoxRecursive(control, rowVm);
					if (found != null)
						return found;
				}
			}
		}
		else if (parent is ContentControl contentControl && contentControl.Content is Control content)
		{
			return FindHexRowTextBoxRecursive(content, rowVm);
		}
		else if (parent is ItemsControl itemsControl)
		{
			foreach (var item in itemsControl.GetRealizedContainers())
			{
				if (item is Control itemControl)
				{
					var found = FindHexRowTextBoxRecursive(itemControl, rowVm);
					if (found != null)
						return found;
				}
			}
		}

		return null;
	}
}
