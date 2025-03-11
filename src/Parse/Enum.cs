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

namespace BlockEditGen.Parse
{
	/// <summary>
	///   Partial <see cref="Enum"/> class to include custom code.
	/// </summary>
	public partial class Enum
	{
		#region Properties

		/// <summary>
		///   Item lookup table which references an item by it's value.
		/// </summary>
		public Dictionary<ulong, Item> ItemLookupByValue { get; private set; } = new Dictionary<ulong, Item>();

		/// <summary>
		///   Item lookup table which references an item by it's name.
		/// </summary>
		public Dictionary<string, Item> ItemLookupByName { get; private set; } = new Dictionary<string, Item>();

		/// <summary>
		///   Indexes into <see cref="ItemLookupByValue"/>.
		/// </summary>
		/// <param name="index">Value of the item to reference.</param>
		/// <returns>Item referenced by the provided value.</returns>
		public Item this[ulong index] { get => ItemLookupByValue[index]; }

		/// <summary>
		///   Indexes into <see cref="ItemLookupByName"/>.
		/// </summary>
		/// <param name="index">Name of the item to reference.</param>
		/// <returns></returns>
		public Item this[string index] { get => ItemLookupByName[index]; }

		/// <summary>
		///   Length of the enumeration's values.
		/// </summary>
		public ByteBitValue Length { get; private set; }

		/// <summary>
		///   Bit mask associated 
		/// </summary>
		public ulong BitMask { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		///   Initializes the custom logic of the class.
		/// </summary>
		/// <exception cref="InvalidOperationException">Unable to initialize the enumeration.</exception>
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
					throw new InvalidOperationException($"The item value in {item.Name} is being used in one or more items in enum {Id}. Values must be unique across all items in an enum.");
				ItemLookupByValue.Add(item.Value, item);

				// Check if item is larger than the available bit width.
				if(item.Value > BitMask)
					throw new InvalidOperationException($"The item value ({item.Value}) with name {item.Name} in enum {Id} is greater than the available bits in the enum ({Width}).");
			}
		}

		/// <summary>
		///   Gets a bitmask based on the <see cref="ByteBitValue"/> size.
		/// </summary>
		/// <param name="size">Size of the bitmask. Must be less than 64 bits.</param>
		/// <returns>Generated bitmask.</returns>
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

		#endregion
	}
}
