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
using System;
using System.Collections.Generic;

namespace BlockEditGen.Parse
{
	public partial class Enum
	{
		public Dictionary<ulong, Item> ItemLookupByValue { get; private set; } = new Dictionary<ulong, Item>();
		public Dictionary<string, Item> ItemLookupByName { get; private set; } = new Dictionary<string, Item>();

		public Item this[ulong index] { get => ItemLookupByValue[index]; }

		public Item this[string index] { get => ItemLookupByName[index]; }

		public ByteBitValue Length { get; private set; }

		public ulong BitMask { get; private set; }

		public void Initialize()
		{
			Length = new ByteBitValue(Width);
			if (Length.TotalBits > 64)
				throw new InvalidOperationException($"The width of the enumeration ({Length.ToString()}) is larger than 64-bits. Enumerations are limited to 64 bits or less.");
			BitMask = GetBitMask(Length);

			// Build item lookup table.
			foreach (var item in ChildItems)
			{
				if (ItemLookupByName.ContainsKey(item.Name))
					throw new InvalidOperationException($"The item name {item.Name} in enum {Id} is being used in one or more items. Names must be unique across all items in an enum.");
				ItemLookupByName.Add(item.Name, item);

				if(ItemLookupByValue.ContainsKey(item.Value))
					throw new Exception($"The item value in {item.Name} is being used in one or more items in enum {Id}. Values must be unique across all items in an enum.");
				ItemLookupByValue.Add(item.Value, item);

				// Check if item is larger than the available bit width.
				if(item.Value > BitMask)
					throw new Exception($"The item value ({item.Value}) with name {item.Name} in enum {Id} is greater than the available bits in the enum ({Width}).");
			}
		}

		private ulong GetBitMask(ByteBitValue size)
		{
			ulong mask = 0;
			for(int i = 0; i < size.TotalBits; i++)
			{
				mask <<= 1;
				mask |= 1;
			}
			return mask;
		}
	}
}
