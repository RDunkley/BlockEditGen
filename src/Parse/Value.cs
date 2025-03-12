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
	///   Partial <see cref="Value"/> class to include custom code.
	/// </summary>
	public partial class Value
	{
		#region Properties

		/// <summary>
		///   Parent block the value is contained in.
		/// </summary>
		public Block ParentBlock { get; private set; }

		/// <summary>
		///   Address of the value in the block.
		/// </summary>
		public ByteBitValue Address { get; private set; }

		/// <summary>
		///   Length of the value.
		/// </summary>
		public ByteBitValue Length { get; private set; }

		/// <summary>
		///   Conversion to use when reading/writing from/to the register.
		/// </summary>
		public Conv Conversion
		{
			get
			{
				if (string.IsNullOrEmpty(Conv))
					return null;
				return ParentBlock.ConvLookup[Conv];
			}
		}

		/// <summary>
		///   Accessibility of the value.
		/// </summary>
		public AccessType Accessibility
		{
			get
			{
				if (!Access.HasValue)
					return ParentBlock.Accessibility;

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
		///   Initializes the value's custom code.
		/// </summary>
		/// <param name="parent">Parent block the value belongs to (even if under a group).</param>
		/// <exception cref="ArgumentNullException"><paramref name="parent"/> is null.</exception>
		/// <exception cref="InvalidOperationException">Unable to initialize the value.</exception>
		public void Initialize(Block parent)
		{
			if(parent == null) throw new ArgumentNullException(nameof(parent));

			ParentBlock = parent;
			Length = new ByteBitValue(Size);
			Address = new ByteBitValue(Addr, parent.Addressable.Value);

			if (Length.TotalBits == 0)
				throw new InvalidOperationException($"The value ({Name}) has a length of zero.");

			if(Access.HasValue)
			{
				if (Access.Value == AccessEnum.Read && !ParentBlock.Accessibility.HasFlag(AccessType.Read))
					throw new InvalidOperationException($"The value ({Name}) has read accessibility, but the parent block does not.");
				if (Access.Value == AccessEnum.Write && !ParentBlock.Accessibility.HasFlag(AccessType.Write))
					throw new InvalidOperationException($"The value ({Name}) has write accessibility, but the parent block does not.");
				if (Access.Value == AccessEnum.ReadWrite && (!ParentBlock.Accessibility.HasFlag(AccessType.Write) || !ParentBlock.Accessibility.HasFlag(AccessType.Read)))
					throw new InvalidOperationException($"The value ({Name}) has read/write accessibility, but the parent block does not.");
			}

			if(Units != null)
			{
				if (Type != TypeEnum.Int8 && Type != TypeEnum.Int16 && Type != TypeEnum.Int32 && Type != TypeEnum.Int64 && Type != TypeEnum.Double && Type != TypeEnum.Float
					&& Type != TypeEnum.Uint8 && Type != TypeEnum.Uint16 && Type != TypeEnum.Uint32 && Type != TypeEnum.Uint64)
					throw new InvalidOperationException($"The value ({Name}) contains units ({Units}), but the type does not support units (must be an integer or floating point).");
			}

			if(Conv != null)
			{
				if (!parent.ConvLookup.ContainsKey(Conv))
					throw new InvalidOperationException($"The value ({Name}) points to a conversion option ({Conv}), but the conversion option doesn't exist in the block.");

				if (Type != TypeEnum.Int8 && Type != TypeEnum.Int16 && Type != TypeEnum.Int32 && Type != TypeEnum.Int64 && Type != TypeEnum.Double && Type != TypeEnum.Float
					&& Type != TypeEnum.Uint8 && Type != TypeEnum.Uint16 && Type != TypeEnum.Uint32 && Type != TypeEnum.Uint64)
					throw new InvalidOperationException($"The value ({Name}) contains a conversion option ({Conv}), but the type does not support conversion (must be an integer or floating point).");
			}

			if(Type == TypeEnum.Enum)
			{
				if (!parent.EnumLookup.ContainsKey(Subtype))
					throw new InvalidOperationException($"The value ({Name}) points to an enumeration type ({Subtype}), but the enum doesn't exist in the block.");
				if(Length != parent.EnumLookup[Subtype].Length)
					throw new InvalidOperationException($"The value ({Name}) points to an enumeration type ({Subtype}), but the length of the value does not match the enum type length.");
			}
		}

		#endregion
	}
}
