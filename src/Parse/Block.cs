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
	///   Partial <see cref="Block"/> class to include custom code.
	/// </summary>
	public partial class Block
	{
		#region Properties

		/// <summary>
		///   Lookup table for referencing child <see cref="Enum"/> objects based on their ID.
		/// </summary>
		public Dictionary<string, Enum> EnumLookup { get; private set; } = new Dictionary<string, Enum>();

		/// <summary>
		///   Lookup table for referencing child <see cref="Conv"/> objects based on their ID.
		/// </summary>
		public Dictionary<string, Conv> ConvLookup { get; private set; } = new Dictionary<string, Conv>();

		/// <summary>
		///   Gets the accessiibility of the block in <see cref="AccessType"/> object.
		/// </summary>
		public AccessType Accessibility
		{
			get
			{
				if (!Access.HasValue)
					return AccessType.Read | AccessType.Write;

				if (Access.Value == AccessEnum.Read)
					return AccessType.Read;
				if (Access.Value == AccessEnum.Write)
					return AccessType.Write;
				return AccessType.Write | AccessType.Read;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///   Initializes the custom code of the block and checks it for additional errors. This must be called prior to using the block.
		/// </summary>
		/// <exception cref="InvalidOperationException">Unable to initialize the block or a child element.</exception>
		public void Initialize()
		{
			if(!Addressable.HasValue)
			{
				Addressable = 1;
			}
			else
			{
				if (Addressable.Value != 1 && Addressable.Value != 2 && Addressable.Value != 4 && Addressable.Value != 8)
					throw new InvalidOperationException($"The addressable value provided ({Addressable.Value}) is not 1, 2, 4, or 8.");
			}

			// Build enum lookup table.
			foreach (var en in ChildEnums)
			{
				if (EnumLookup.ContainsKey(en.Id))
					throw new InvalidOperationException($"The enum ID {en.Id} is being used in one or more enum. IDs must be unique across all enum entries in the block.");
				EnumLookup.Add(en.Id, en);

				en.Initialize();
			}

			// Build conv lookup table.
			foreach(var conv in ChildConvs)
			{
				if(ConvLookup.ContainsKey(conv.Id))
					throw new InvalidOperationException($"The conv ID {conv.Id} is being used in one or more conversion elements. IDs must be unique across all conv entries in the block.");
				ConvLookup.Add(conv.Id, conv);
			}

			// Initialize the registers.
			foreach (var val in ChildValues)
				val.Initialize(this);
			foreach (var group in ChildGroups)
				group.Initialize(this);

			// Check for value overlap and find max.
			ByteBitValue maxEndValue = new ByteBitValue(0, 0);
			for(int i = 0; i < ChildValues.Length; i++)
			{
				var val =  ChildValues[i];
				var end = val.Address + val.Length;
				for(int j = 0; j < ChildValues.Length; j++)
				{
					if(i != j)
					{
						if (ChildValues[j].Address >= val.Address && ChildValues[j].Address < end)
							throw new InvalidOperationException($"A value ({ChildValues[i].Name}) overlaps another value ({ChildValues[j].Name}). The mapped regions must be unique in the block.");
					}
				}

				if(end > maxEndValue)
					maxEndValue = end;
			}

			if (maxEndValue >= SizeInBytes)
				throw new InvalidOperationException($"The end address ({maxEndValue.ToString()}) in one of the values is larger than the size of the block ({SizeInBytes}).");

			if (Addressable != 1 && Addressable != 2 && Addressable != 4 && Addressable != 8)
				throw new InvalidOperationException($"The Addressable word size is {Addressable}, but only 1, 2, 4, and 8 are allowed.");
		}

		#endregion
	}
}
