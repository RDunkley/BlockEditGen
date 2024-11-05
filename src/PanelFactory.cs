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
using Avalonia.Media;
using BlockEditGen.Controls;
using BlockEditGen.Interfaces;
using BlockEditGen.Parse;
using BlockEditGen.ViewModels;

namespace BlockEditGen
{
	public static class PanelFactory
	{
		public static void PopulatePanel(ICachedRegisterBlock block, string xmlPath, Panel panel)
		{
			DataViewModelBase.FontFamily = new FontFamily("Arial");

			var pBlock = new Block(xmlPath);
			pBlock.Initialize();

			panel.Children.Clear();
			var sv = new ScrollViewer
			{
				HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Visible,
			};

			var stackPanel = new StackPanel
			{
				HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
				Orientation = Avalonia.Layout.Orientation.Vertical,
				Margin = new Avalonia.Thickness(20, 20, 20, 20),
			};
			sv.Content = stackPanel;
			panel.Children.Add(sv);

			string maxName = string.Empty;
			foreach(var value in pBlock.ChildValues)
			{
				if (value.Name.Length > maxName.Length)
					maxName = value.Name;
			}
			var typeFace = new Typeface(DataViewModelBase.FontFamily);
			var nameFormattedText = new FormattedText(maxName, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, DataViewModelBase.FontSize, Brushes.Black);

			foreach (var value in pBlock.ChildValues)
			{
				int sizeInBytes = value.Length.Bytes;
				if (value.Length.Bits > 0)
					sizeInBytes += 1;

				UserControl newControl;
				switch(value.Type)
				{
					case Value.TypeEnum.Bool:
						newControl = new BoolUserControl
						{
							DataContext = new BoolViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.Double:
						newControl = new StringBasedUserControl
						{
							DataContext = new DoubleViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.Enum:
						newControl = new EnumUserControl
						{
							DataContext = new EnumViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.Float:
						newControl = new StringBasedUserControl
						{
							DataContext = new FloatViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.Int8:
						newControl = new StringBasedUserControl
						{
							DataContext = new Int8ViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.Int16:
						newControl = new StringBasedUserControl
						{
							DataContext = new Int16ViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.Int32:
						newControl = new StringBasedUserControl
						{
							DataContext = new Int32ViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.Int64:
						newControl = new StringBasedUserControl
						{
							DataContext = new Int64ViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.Ip:
						newControl = new StringBasedUserControl
						{
							DataContext = new IpViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.Mac:
						newControl = new StringBasedUserControl
						{
							DataContext = new MacViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.String:
						newControl = new StringBasedUserControl
						{
							DataContext = new StringViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.Uint8:
						newControl = new StringBasedUserControl
						{
							DataContext = new UInt8ViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.Uint16:
						newControl = new StringBasedUserControl
						{
							DataContext = new UInt16ViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.Uint32:
						newControl = new StringBasedUserControl
						{
							DataContext = new UInt32ViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					case Value.TypeEnum.Uint64:
						newControl = new StringBasedUserControl
						{
							DataContext = new UInt64ViewModel(value, block),
							NameWidth = nameFormattedText.WidthIncludingTrailingWhitespace,
						}; break;
					default:
						newControl = null; break;
				}

				if (newControl != null)
				{
					newControl.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
					newControl.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
					stackPanel.Children.Add(newControl);
				}
			}

			//panel.InvalidateMeasure();
		}
	}
}
