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
using BlockEditGen.Data;
using BlockEditGen.Interfaces;
using BlockEditGen.Parse;
using System;
using System.Collections.Generic;

namespace BlockEditGen.ViewModels
{
	public class EnumViewModel : DataViewModelBase
	{
		private int _sizeInBytes;
		private Item[] _items;
		private Dictionary<ulong, int> _itemLookupByValue = new Dictionary<ulong, int>();

		public string[] Items { get; private set; }

		public int SelectedIndex
		{
			get
			{
				var buf = new byte[_sizeInBytes];
				_block.ReadSection(_value.Address, _value.Length, buf);
				ulong val = ConvertBufToVal(buf);
				if (!_itemLookupByValue.ContainsKey(val))
				{
					HasError = true;
					OnPropertyChanged(nameof(CurrentState));
					return -1;
				}
				HasError = false;
				return _itemLookupByValue[val];
			}
			set
			{
				var buf = new byte[_sizeInBytes];
				ConvertValToBuf(_items[value].Value, buf);
				_block.WriteSection(_value.Address, _value.Length, buf);
				OnPropertyChanged(nameof(SelectedIndex));
				OnPropertyChanged(nameof(CurrentState));
			}
		}

		public EnumViewModel()
			: this(
				new Block(Block.AccessEnum.ReadWrite, 1, "block description", "Block ID", "Block Name", 4096, new Version(1,0), null,
					[new("enum_id", "0.3", [new("Item1", "Item 1 Description", 0)])], null,
					[new Value(Value.AccessEnum.ReadWrite, null, null, "ValueName", "0.3", "enum_id", "Enum Tooltip", Value.TypeEnum.Enum, null)]).ChildValues[0],
				new CachedRegisterBlock<byte>(new RamRegisterBlock<byte>(4096))
			)
		{
		}

		public EnumViewModel(Value value, ICachedRegisterBlock block)
			: base(value, block)
		{
			if(value.Type != Value.TypeEnum.Enum)
				throw new ArgumentException($"The value provided ({value.Name}) is not an enum type.");

			_sizeInBytes = value.Length.Bytes;
			if (value.Length.Bits > 0)
				_sizeInBytes += 1;

			_items = value.ParentBlock.EnumLookup[value.Subtype].ChildItems;
			Items = new string[_items.Length];
			for (int i = 0; i < Items.Length; i++)
			{
				Items[i] = _items[i].Name;
				_itemLookupByValue.Add(_items[i].Value, i);
			}

			block.CacheChanged += Block_CacheChanged;
		}

		private void Block_CacheChanged(object sender, EventArgs e)
		{
			OnPropertyChanged(nameof(SelectedIndex));
			OnPropertyChanged(nameof(CurrentState));
		}

		private ulong ConvertBufToVal(byte[] buf)
		{
			ulong val = buf[0];
			for(int i = 1; i < buf.Length; i++)
				val |= (ulong)buf[i] << (i * 8);
			return val;
		}

		private void ConvertValToBuf(ulong val, byte[] buf)
		{
			for (int i = 0; i < buf.Length; i++)
				buf[i] = (byte)(val >> (i * 8));
		}
	}
}
