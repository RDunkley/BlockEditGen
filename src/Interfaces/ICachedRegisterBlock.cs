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
using System.Threading.Tasks;

namespace BlockEditGen.Interfaces
{
    /// <summary>
    ///   Represents a cached register block that allows for read/writing to the cache and then pushing all the changes at once.
    /// </summary>
    public interface ICachedRegisterBlock
	{
		/// <summary>
		///   Raised when the values are updated from the underlying register block or the cached values are pushed to the
		///   underlying block (signaling a state change).
		/// </summary>
		event EventHandler CacheChanged;

		/// <summary>
		///   Reads a section of the cache.
		/// </summary>
		/// <param name="address">Address to begin reading at.</param>
		/// <param name="length">Number of bytes/bits to read.</param>
		/// <param name="dst">Byte <see cref="Span{T}"/> to store the read data in. Misaligned data is written aligned to this buffer.</param>
		void ReadSection(ByteBitValue address, ByteBitValue length, Span<byte> dst);

		/// <summary>
		///   Writes a section of the cache.
		/// </summary>
		/// <param name="address">Address to begin writing at.</param>
		/// <param name="length">Number of bytes/bits to write.</param>
		/// <param name="src">Source <see cref="Span{T}"/> to write the data from. Data is read from this array aligned and possibly written misaligned to the cache.</param>
		void WriteSection(ByteBitValue address, ByteBitValue length, Span<byte> src);

		/// <summary>
		///   Returns the state of the section of interest.
		/// </summary>
		/// <param name="address">Address to begin the section of interest at.</param>
		/// <param name="length">Length of the section to determine the state of.</param>
		/// <returns>
		///   <see cref="DataControlState.Error"/> if any of the bytes in the section are in error, <see cref="DataControlState.Modified"/> or <see cref="DataControlState.Updated"/>
		///   if any of the bytes are updated or modified, or <see cref="DataControlState.Default"/> if all of the bytes in the section or in the default state.
		/// </returns>
		DataControlState GetSectionState(ByteBitValue address, ByteBitValue length);

		/// <summary>
		///   Instructs this object to pull all the values from the underlying register block and overwrite any values in the cache.
		/// </summary>
		/// <remarks>Any bytes that have changed from the last time the registers were pulled will be marked as <see cref="DataControlState.Updated"/>.</remarks>
		Task UpdateValuesFromRegisterBlockAsync();

		/// <summary>
		///   Instructs this object to push all the changed values from the cache to the underlying register block.
		/// </summary>
		/// <remarks>
		///   Any bytes that are currently in the <see cref="DataControlState.Modified"/> state will get pushed and change to a <see cref="DataControlState.Default"/> state.
		///   Bytes in an <see cref="DataControlState.Error"/> state are ignored.
		/// </remarks>
		Task PushChangedValuesToRegisterBlockAsync();
	}
}
