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
using System;

namespace BlockEditGen.ViewModels
{
	public partial class BoolViewModel : DataViewModelBase
	{
		#region Observable Fields

		public bool IsChecked
		{
			get
			{
				var val = new byte[1];
				_block.ReadSection(_value.Address, _value.Length, val);
				return val[0] == 1;
			}
			set
			{
				byte val = value ? (byte)0x1 : (byte)0x0;
				_block.WriteSection(_value.Address, _value.Length, new byte[] { val });
				OnPropertyChanged(nameof(IsChecked));
				OnPropertyChanged(nameof(CurrentState));
			}
		}

		#endregion

		#region Read-Only Properties

		public string HighText { get; private set; }

		public string LowText { get; private set; }

		#endregion

		public BoolViewModel()
			: this(
				new Value(Value.AccessEnum.ReadWrite, "0x07.5", null, "bool description", "Bool Name", "0.1", "Off,On", Value.TypeEnum.Bool, null),
				new CachedRegisterBlock<byte>(new RamRegisterBlock<byte>(4096))
				)
		{
		}

		public BoolViewModel(Value value, ICachedRegisterBlock block)
			: base(value, block)
		{
			if(value.Type != Value.TypeEnum.Bool)
				throw new ArgumentException($"The value provided ({value.Name}) is not a boolean type.");
			if (value.Length.TotalBits != 1)
				throw new ArgumentException($"The value provided ({value.Name}) is not a single bit in length.");

			if (!value.Subtype.Contains(','))
				throw new ArgumentException($"The value provided ({value.Name}) has an invalid subtype ({value.Subtype}) for a boolean type (must be <low bit text>,<high bit text>).");
			var splits = value.Subtype.Split(',', StringSplitOptions.RemoveEmptyEntries);
			if(splits.Length != 2)
				throw new ArgumentException($"The value provided ({value.Name}) has an invalid subtype ({value.Subtype}) for a boolean type (must be <low bit text>,<high bit text>).");

			// For the designer.
			LowText = splits[0];
			HighText = splits[1];

			block.CacheChanged += Block_CacheChanged;
		}

		private void Block_CacheChanged(object sender, EventArgs e)
		{
			OnPropertyChanged(nameof(IsChecked));
			OnPropertyChanged(nameof(CurrentState));
		}
	}
}
