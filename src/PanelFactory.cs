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
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using BlockEditGen.Controls;
using BlockEditGen.Interfaces;
using BlockEditGen.Parse;
using BlockEditGen.ViewModels;
using System.Xml.Linq;

namespace BlockEditGen
{
	public static class PanelFactory
	{
		public static int FontSize { get; set; } = 22;

		public static FontFamily FontFamily { get; set; } = new FontFamily("Arial");

		public static void PopulatePanel(ICachedRegisterBlock block, string xmlPath, Panel panel)
		{
			var pBlock = new Block(xmlPath);
			pBlock.Initialize();

			PopulatePanel(block, pBlock, panel);
		}

		public static void PopulatePanel(ICachedRegisterBlock block, Block pBlock, Panel panel)
		{
			panel.Children.Clear();
			var sv = new ScrollViewer
			{
				HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
			};

			var stackPanel = new StackPanel
			{
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Orientation = Orientation.Vertical,
				Margin = new Thickness(20, 20, 20, 20),
			};
			sv.Content = stackPanel;
			panel.Children.Add(sv);

			var maxNameWidth = GetMaxPixelWidthOfNames(pBlock.ChildValues);

			int valueIndex = 0;
			int groupIndex = 0;
			while(valueIndex < pBlock.ChildValues.Length || groupIndex < pBlock.ChildGroups.Length)
			{
				if(groupIndex < pBlock.ChildGroups.Length && (valueIndex == pBlock.ChildValues.Length || pBlock.ChildGroups[groupIndex].Ordinal < pBlock.ChildValues[valueIndex].Ordinal))
				{
					var group = pBlock.ChildGroups[groupIndex++];

					// Place a group with all it's values.
					var groupBox = new HeaderedContentControl
					{
						Header = group.Name,
						//Background = new SolidColorBrush(Colors.Black),
						Padding = new Thickness(10),
						Margin = new Thickness(10),
						BorderThickness = new Thickness(2),
						Content = new Border
						{
							BorderBrush = new SolidColorBrush(Colors.Black),
							BorderThickness = new Thickness(1),
							Padding = new Thickness(1),
							Child = new StackPanel
							{
								HorizontalAlignment = HorizontalAlignment.Stretch,
								VerticalAlignment = VerticalAlignment.Stretch,
								Orientation = Orientation.Vertical,
								Margin = new Thickness(5, 5, 5, 5),
							}
						}
					};
					stackPanel.Children.Add(groupBox);

					var groupStackPanel = (groupBox.Content as Border).Child as StackPanel;
					var maxGroupNameWidth = GetMaxPixelWidthOfNames(group.ChildValues);
					foreach (var value in group.ChildValues)
						AddValueControl(groupStackPanel, maxGroupNameWidth, value, block);
				}
				else
				{
					var value = pBlock.ChildValues[valueIndex++];

					// Place the value.
					AddValueControl(stackPanel, maxNameWidth, value, block);
				}
			}

			//panel.InvalidateMeasure();
		}

		private static double GetMaxPixelWidthOfNames(Value[] values)
		{
			var typeFace = new Typeface(FontFamily);
			double maxSize = 0.0;
			foreach (var value in values)
			{
				var nameFormattedText = new FormattedText($"{value.Name}:", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeFace, FontSize, Brushes.Black);
				if(nameFormattedText.WidthIncludingTrailingWhitespace > maxSize)
					maxSize = nameFormattedText.WidthIncludingTrailingWhitespace;
			}
			return maxSize;
		}

		private static void AddValueControl(StackPanel panel, double nameWidth, Value value, ICachedRegisterBlock block)
		{
			UserControl newControl;
			switch (value.Type)
			{
				case Value.TypeEnum.Bool:
					newControl = new BoolUserControl
					{
						DataContext = new BoolViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.Double:
					newControl = new StringBasedUserControl
					{
						DataContext = new DoubleViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.Enum:
					newControl = new EnumUserControl
					{
						DataContext = new EnumViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.Float:
					newControl = new StringBasedUserControl
					{
						DataContext = new FloatViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.Int8:
					newControl = new StringBasedUserControl
					{
						DataContext = new Int8ViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.Int16:
					newControl = new StringBasedUserControl
					{
						DataContext = new Int16ViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.Int32:
					newControl = new StringBasedUserControl
					{
						DataContext = new Int32ViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.Int64:
					newControl = new StringBasedUserControl
					{
						DataContext = new Int64ViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.Ip:
					newControl = new StringBasedUserControl
					{
						DataContext = new IpViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.Mac:
					newControl = new StringBasedUserControl
					{
						DataContext = new MacViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.String:
					newControl = new StringBasedUserControl
					{
						DataContext = new StringViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.Uint8:
					newControl = new StringBasedUserControl
					{
						DataContext = new UInt8ViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.Uint16:
					newControl = new StringBasedUserControl
					{
						DataContext = new UInt16ViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.Uint32:
					newControl = new StringBasedUserControl
					{
						DataContext = new UInt32ViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				case Value.TypeEnum.Uint64:
					newControl = new StringBasedUserControl
					{
						DataContext = new UInt64ViewModel(value, block),
						NameWidth = nameWidth,
					}; break;
				default:
					newControl = null; break;
			}

			if (newControl != null)
			{
				newControl.HorizontalAlignment = HorizontalAlignment.Stretch;
				newControl.VerticalAlignment = VerticalAlignment.Stretch;
				panel.Children.Add(newControl);
			}
		}
	}
}
