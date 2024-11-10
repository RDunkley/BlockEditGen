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
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using BlockEditGen.Data;
using BlockEditGen.Interfaces;
using BlockEditGen.Parse;
using System.Runtime.CompilerServices;

namespace BlockEditGen.ViewModels
{
	public abstract class DataViewModelBase : ObservableObject
	{
		protected ICachedRegisterBlock _block;
		protected Value _value;

		#region Read-Only Properties

		public int LengthInBytes { get; protected set; }

		public static int FontSize { get; set; } = 16;

		public static FontFamily FontFamily { get; set; } = FontFamily.Default;

		public static Thickness Margin { get; set; } = new Thickness(5, 5, 5, 5);

		public string Name { get { return _value.Name; } }
		public string Description { get { return _value.Tooltip; } }
		public bool IsReadOnly { get { return _value.Accessibility == AccessType.Read; } }
		public bool HasError { get; protected set; }

		public DataControlState CurrentState
		{
			get
			{
				if (HasError)
					return DataControlState.Error;
				return GetControlState();
			}
		}

		#endregion

		#region Methods

		public DataViewModelBase(Value value, ICachedRegisterBlock block)
		{
			_block = block;
			_value = value;
			LengthInBytes = (value.Length.TotalBits + 7) / 8;
		}

		private DataControlState GetControlState()
		{
			return _block.GetSectionState(_value.Address, _value.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong GenerateBitMask(int numBits)
		{
			ulong mask = 0;
			for (int i = 0; i < numBits; i++)
				mask |= GenerateSingleBitMask(i);
			return mask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong GenerateSingleBitMask(int index)
		{
			return (ulong)1 << index;
		}

		#endregion
	}
}
