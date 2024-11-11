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
using BlockEditGen.Interfaces;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BlockEditGen.Data
{
	/// <summary>
	///   Register block that provides a caching and state mechnism for an underlying register block.
	/// </summary>
	/// <typeparam name="T">Must be byte, ushort, uint, or ulong.</typeparam>
	public class CachedRegisterBlock<T> : INotifyPropertyChanged, ICachedRegisterBlock where T : struct, INumber<T>, IUnsignedNumber<T>
	{
		#region Fields

		/// <summary>
		///   Incremented when a byte moves from a <see cref="DataControlState.Default"/> to a <see cref="DataControlState.Modified"/>.
		///   Decremented when a byte moves back to the <see cref="DataControlState.Default"/> state.
		/// </summary>
		private uint _changeCounter = 0;

		/// <summary>
		///   The maximum size of block in bytes.
		/// </summary>
		private const int _maxArraySize = 1024 * 1024;

		/// <summary>
		///   Stores the previous values of the block pulled from the registers.
		/// </summary>
		private Memory<byte> _prev;

		/// <summary>
		///   Stores the cache version of the values that are manipulated using <see cref="ReadSection(ByteBitValue, ByteBitValue, Span{byte})"/>
		///   and <see cref="WriteSection(ByteBitValue, ByteBitValue, Span{byte})"/> calls.
		/// </summary>
		private Memory<byte> _cache;

		/// <summary>
		///   Stores the state of each byte in the cache.
		/// </summary>
		private Memory<DataControlState> _state;

		/// <summary>
		///   Stores the underlying register block that values will be pushed and pulled from.
		/// </summary>
		protected IRegisterBlock<T> _block;

		#endregion

		#region Properties

		/// <summary>
		///   Gets the size of the register in bytes (sizeof(T)).
		/// </summary>
		public int SizeOfRegister { get; private set; }

		/// <summary>
		///   Gets the size of the block in bytes.
		/// </summary>
		public int SizeInBytes { get { return _cache.Length; } }

		/// <summary>
		///   Gets whether some of the bytes have been modified.
		/// </summary>
		public bool HasChanges { get { return _changeCounter != 0; } }

		#endregion

		#region Events

		/// <summary>
		///   Raised then values in the cache have changed using a <see cref="UpdateValuesFromRegisterBlock"/> or <see cref="PushChangedValuesToRegisterBlock"/> call.
		/// </summary>
		public event EventHandler CacheChanged;

		/// <summary>
		///   Raised when <see cref="HasChanges"/> has been updated.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		/// <summary>
		///   Instantiates a new <see cref="CachedRegisterBlock{T}"/> object.
		/// </summary>
		/// <param name="sizeInBytes"></param>
		/// <param name="block"></param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public CachedRegisterBlock(IRegisterBlock<T> block)
		{
			if (block == null)
				throw new ArgumentNullException(nameof(block));
			if (block.SizeInBytes < 1 || block.SizeInBytes > _maxArraySize)
				throw new ArgumentException($"The register block's size ({block.SizeInBytes}) is less than 1 or greater than 1,048,576.");
			if (typeof(T) == typeof(byte))
				SizeOfRegister = sizeof(byte);
			else if (typeof(T) == typeof(ushort))
				SizeOfRegister = sizeof(ushort);
			else if (typeof(T) == typeof(uint))
				SizeOfRegister = sizeof(uint);
			else if (typeof(T) == typeof(ulong))
				SizeOfRegister = sizeof(ulong);
			else
				throw new ArgumentException($"The type is not valid. The type must be byte, ushort, uint or ulong.");

			_prev = new byte[block.SizeInBytes];
			_cache = new byte[block.SizeInBytes];
			_state = new DataControlState[block.SizeInBytes];
			_block = block;
		}

		/// <summary>
		///   Reads a section of the cache.
		/// </summary>
		/// <param name="address">Address to begin reading at.</param>
		/// <param name="length">Number of bytes/bits to read.</param>
		/// <param name="dst">Byte <see cref="Span{T}"/> to store the read data in. Misaligned data is written aligned to this buffer.</param>
		/// <exception cref="ArgumentNullException"><paramref name="address"/>, <paramref name="dst"/> or <paramref name="length"/> is null.</exception>
		/// <exception cref="ArgumentException">The number of bytes <paramref name="length"/> takes up does not match the destination array length.</exception>
		public void ReadSection(ByteBitValue address, ByteBitValue length, Span<byte> dst)
		{
			if(address == null) throw new ArgumentNullException(nameof(address));
			if(length == null) throw new ArgumentNullException(nameof(length));
			if (dst == null) throw new ArgumentNullException(nameof(dst));

			int dstSize = ComputeBufferSize(length);
			if (dst.Length != dstSize)
				throw new ArgumentException($"The length provided ({length}) does not match the destination array length ({dst.Length})");

			if (address.Bits == 0)
			{
				// Cache location is on a byte boundary.
				if (length.Bytes != 0)
					_cache.Slice(address.Bytes, length.Bytes).Span.CopyTo(dst);

				// Copy over the rest of the bits if needed.
				if (length.Bits != 0)
					dst[length.Bytes] = (byte)(_cache.Span[address.Bytes + length.Bytes] & GetBitMask(length.Bits));
				return;
			}

			// Clear the buffer.
			for (int i = 0; i < dst.Length; i++)
				dst[i] = 0;

			// Copy over in bits (this is much slower).
			var written = new ByteBitValue(0);
			var srcBitIndex = address.Bits;
			var dstBitIndex = 0;
			while(written.TotalBits < length.TotalBits)
			{
				int numBitsToWrite = 8 - srcBitIndex; // Amount left in the source byte.
				if (numBitsToWrite > (8 - dstBitIndex)) // Reduce to amount left in destination byte.
					numBitsToWrite = 8 - dstBitIndex;
				if (numBitsToWrite > (length.TotalBits - written.TotalBits)) // Reduce to number of bits left.
					numBitsToWrite = (int)(length.TotalBits - written.TotalBits);

				dst[written.Bytes] |= (byte)(((_cache.Span[address.Bytes + written.Bytes] >> srcBitIndex) & GetBitMask(numBitsToWrite)) << dstBitIndex);
				dstBitIndex += numBitsToWrite;
				dstBitIndex %= 8;
				srcBitIndex += numBitsToWrite;
				srcBitIndex %= 8;
				written.AddBits(numBitsToWrite);
			}
		}

		private void UpdateState(int byteAddress)
		{
			// Check what the state should now be.
			DataControlState state;
			if (_cache.Span[byteAddress] != _prev.Span[byteAddress])
				state = DataControlState.Modified;
			else
				state = DataControlState.Default;

			// Check if we need to update the counter.
			if (state == DataControlState.Modified && _state.Span[byteAddress] != DataControlState.Modified)
				UpdateChangeCounter(true);
			if (state == DataControlState.Default && _state.Span[byteAddress] == DataControlState.Modified)
				UpdateChangeCounter(false);

			// Set the new state.
			_state.Span[byteAddress] = state;
		}

		private void UpdateChangeCounter(bool increment)
		{
			var current = _changeCounter;
			if (increment)
				_changeCounter++;
			else
				_changeCounter--;

			if ((current == 0 && _changeCounter != 0) || (current != 0 && _changeCounter == 0))
				OnPropertyChanged(nameof(HasChanges));
		}

		/// <summary>
		///   Writes a section of the cache.
		/// </summary>
		/// <param name="address">Address to begin writing at.</param>
		/// <param name="length">Number of bytes/bits to write.</param>
		/// <param name="src">Source <see cref="Span{T}"/> to write the data from. Data is read from this array aligned and possibly written misaligned to the cache.</param>
		/// <exception cref="ArgumentNullException"><paramref name="address"/>, <paramref name="src"/> or <paramref name="length"/> is null.</exception>
		/// <exception cref="ArgumentException">The number of bytes <paramref name="length"/> takes up does not match the source array length.</exception>
		public void WriteSection(ByteBitValue address, ByteBitValue length, Span<byte> src)
		{
			if (address == null) throw new ArgumentNullException(nameof(address));
			if (length == null) throw new ArgumentNullException(nameof(length));
			if (src == null) throw new ArgumentNullException(nameof(src));

			int srcSize = ComputeBufferSize(length);
			if (src.Length != srcSize)
				throw new ArgumentException($"The length provided ({length}) does not match the source array length ({src.Length})");

			if(address.Bits == 0)
			{
				// Cache location is on a byte boundary.
				for(int i = 0; i < length.Bytes; i++)
				{
					_cache.Span[address.Bytes + i] = src[i];
					UpdateState(address.Bytes + i);
				}

				// Copy over the rest of the bits if needed.
				if (length.Bits != 0)
				{
					byte mask = GetBitMask(length.Bits);
					_cache.Span[address.Bytes + length.Bytes] &= (byte)~mask; // Mask the bits to zero.
					_cache.Span[address.Bytes + length.Bytes] |= (byte)(src[length.Bytes] & mask); // Add the final source bits.
					UpdateState(address.Bytes + length.Bytes);
				}
				OnCacheChanged();
				return;
			}

			// Copy over in bits (this is much slower).
			var written = new ByteBitValue(0);
			var srcBitIndex = 0;
			var dstBitIndex = address.Bits;
			while (written.TotalBits < length.TotalBits)
			{
				int numBitsToWrite = 8 - srcBitIndex; // Amount left in the source byte.
				if (numBitsToWrite > (8 - dstBitIndex)) // Reduce to amount left in destination byte.
					numBitsToWrite = 8 - dstBitIndex;
				if (numBitsToWrite > (length.TotalBits - written.TotalBits)) // Reduce to number of bits left.
					numBitsToWrite = (int)(length.TotalBits - written.TotalBits);

				var mask = GetBitMask(numBitsToWrite) << dstBitIndex;
				_cache.Span[address.Bytes + written.Bytes] &= (byte)~mask; // Clear the bits we are going to overwrite.
				_cache.Span[address.Bytes + written.Bytes] |= (byte)(((src[written.Bytes] >> srcBitIndex) << dstBitIndex) & mask);
				UpdateState (address.Bytes + written.Bytes);

				dstBitIndex += numBitsToWrite;
				dstBitIndex %= 8;
				srcBitIndex += numBitsToWrite;
				srcBitIndex %= 8;
				written.AddBits(numBitsToWrite);
			}
			OnCacheChanged();
		}

		/// <summary>
		///   Returns the state of the section of interest.
		/// </summary>
		/// <param name="address">Address to begin the section of interest at.</param>
		/// <param name="length">Length of the section to determine the state of.</param>
		/// <returns>
		///   <see cref="DataControlState.Error"/> if any of the bytes in the section are in error, <see cref="DataControlState.Modified"/> or <see cref="DataControlState.Updated"/>
		///   if any of the bytes are updated or modified, or <see cref="DataControlState.Default"/> if all of the bytes in the section or in the default state.
		/// </returns>
		public DataControlState GetSectionState(ByteBitValue address, ByteBitValue length)
		{
			if (address == null) throw new ArgumentNullException(nameof(address));
			if (length == null) throw new ArgumentNullException(nameof(length));

			// Check for updated first since we can only check whole bytes.
			int byteSize = ComputeBufferSize(new ByteBitValue(length.Bytes, address.Bits + length.Bits));
			bool updated = false;
			for (int i = 0; i < byteSize; i++)
			{
				if (_state.Span[address.Bytes + i] == DataControlState.Updated)
					updated = true;
			}

			// Check for modified down to the bit level.
			bool modified = false;
			if (address.Bits == 0)
			{
				// Cache location is on a byte boundary.
				for (int i = 0; i < length.Bytes; i++)
				{
					if (_state.Span[address.Bytes + i] == DataControlState.Modified)
					{
						modified = true;
						break;
					}
				}

				// Check the remaining bits.
				if(length.Bits != 0 && !modified)
				{
					byte mask = GetBitMask(length.Bits);
					byte cacheByte = (byte)(_cache.Span[address.Bytes + length.Bytes] & mask);
					byte prevByte = (byte)(_prev.Span[address.Bytes + length.Bytes] & mask);
					if (cacheByte != prevByte)
						modified = true;
				}

				if (modified) return DataControlState.Modified;
				if (updated) return DataControlState.Updated;
				return DataControlState.Default;
			}

			// Check over in bits (this is much slower).
			var checkedBits = new ByteBitValue(0);
			var bitIndex = address.Bits;
			while (checkedBits.TotalBits < length.TotalBits)
			{
				int numBitsToCheck = 8 - bitIndex; // Amount left in the current byte.
				if (numBitsToCheck > (length.TotalBits - checkedBits.TotalBits)) // Reduce to number of bits left.
					numBitsToCheck = (int)(length.TotalBits - checkedBits.TotalBits);

				var mask = GetBitMask(numBitsToCheck) << bitIndex;
				byte cacheByte = (byte)(_cache.Span[address.Bytes + checkedBits.Bytes] & mask);
				byte prevByte = (byte)(_prev.Span[address.Bytes + checkedBits.Bytes] & mask);
				if (cacheByte != prevByte)
				{
					modified = true;
					break;
				}

				bitIndex += numBitsToCheck;
				bitIndex %= 8;
				checkedBits.AddBits(numBitsToCheck);
			}

			if (modified) return DataControlState.Modified;
			if (updated) return DataControlState.Updated;
			return DataControlState.Default;
		}

		private DataControlState GetRegState(int regAddress)
		{
			bool modified = false;
			bool updated = false;
			regAddress *= SizeOfRegister;
			for (int i = 0; i < SizeOfRegister; i++)
			{
				var span = _state.Span;
				if (span[regAddress + i] == DataControlState.Modified)
					modified = true;
				else if (span[regAddress + i] == DataControlState.Updated)
					updated = true;
			}

			if (modified)
				return DataControlState.Modified;
			if (updated)
				return DataControlState.Updated;
			return DataControlState.Default;
		}

		private void ClearRegState(int startByte, int byteCount)
		{
			for(int i = 0; i < byteCount; i++)
			{

				if (_state.Span[startByte + i] == DataControlState.Modified)
					UpdateChangeCounter(false);
				_state.Span[startByte + i] = DataControlState.Default;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public async Task UpdateValuesFromRegisterBlockAsync()
		{
			// Write the new values to cache.
			if(_block is IRegisterBlock<byte> byteBlock)
				await byteBlock.ReadAsync(0, _cache);
			else
				await _block.ReadAsync(0, new Memory<T>(MemoryMarshal.Cast<byte, T>(_cache.Span).ToArray()));

			// Check what values have changes since the last push.
			for(int i = 0; i < _cache.Length; i++)
			{
				if (_cache.Span[i] != _prev.Span[i])
					_state.Span[i] = DataControlState.Updated;
				else
					_state.Span[i] = DataControlState.Default;
			}

			// Update the previous values.
			_cache.Span.CopyTo(_prev.Span);
			_changeCounter = 0;

			OnCacheChanged();
			OnPropertyChanged(nameof(HasChanges));
		}

		public async Task PushChangedValuesToRegisterBlockAsync()
		{
			// Write the modified values to the register block.
			int numRegs = _cache.Length / SizeOfRegister;
			int regIndex = 0;
			while (regIndex < numRegs)
			{
				if (GetRegState(regIndex) == DataControlState.Modified)
				{
					var startReg = regIndex++;
					while (GetRegState(regIndex) == DataControlState.Modified && regIndex < numRegs)
					{
						regIndex++;
					}

					var startByte = startReg * SizeOfRegister;
					var byteCount = (regIndex - startReg) * SizeOfRegister;

					await _block.WriteAsync(startByte, new Memory<T>(MemoryMarshal.Cast<byte, T>(_cache.Span.Slice(startByte, byteCount)).ToArray()));
					ClearRegState(startByte, byteCount);
					_cache.Slice(startByte, byteCount).CopyTo(_prev.Slice(startByte, byteCount)); // Copy the cache to prev.
					regIndex++;
				}
				else
				{
					regIndex++;
				}
			}

			OnCacheChanged();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private byte GetBitMask(int numBits)
		{
			byte mask = 0;
			for (int i = 0; i < numBits; i++)
				mask |= (byte)(1 << i);
			return mask;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int ComputeBufferSize(ByteBitValue length)
		{
			return length.Bytes + ((length.Bits + 7) / 8);
		}

		private void OnCacheChanged()
		{
			CacheChanged?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
