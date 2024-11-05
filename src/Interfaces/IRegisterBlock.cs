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
using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace BlockEditGen.Interfaces
{
	/// <summary>
	///   Represents an underlying register block to be read and written from.
	/// </summary>
	/// <typeparam name="T">Must be a byte, ushort, uint, or ulong.</typeparam>
	public interface IRegisterBlock<T> where T : struct, INumber<T>, IUnsignedNumber<T>
	{
		/// <summary>
		///   Gets whether the block can be written to, or whether it is read-only.
		/// </summary>
		bool CanWrite { get; }

		/// <summary>
		///   Gets the size of the block, in bytes.
		/// </summary>
		int SizeInBytes { get; }

		/// <summary>
		///   Reads a span of <typeparamref name="T"/> values at the specified byte address.
		/// </summary>
		/// <param name="byteAddress">Address to begin reading at. Will always be on a word boundary.</param>
		/// <param name="dst">Destination span to write the values to.</param>
		/// <remarks>The <paramref name="byteAddress"/> will always be on a word boundary based on size of <typeparamref name="T"/>.</remarks>
		/// <exception cref="IOException">An error occurred writing to the underlying block.</exception>
		Task ReadAsync(int byteAddress, Memory<T> dst);

		/// <summary>
		///   Reads a span of <typeparamref name="T"/> values at the specified byte address.
		/// </summary>
		/// <param name="byteAddress">Address to begin reading at. Will always be on a word boundary.</param>
		/// <param name="count">Number of <typeparamref name="T"/> words to be read.</param>
		/// <returns><see cref="Span{T}"/> of <typeparamref name="T"/> containing the read values.</returns>
		/// <exception cref="IOException">An error occurred writing to the underlying block.</exception>
		//Task<Memory<T>> ReadAsync(int byteAddress, int count);

		/// <summary>
		///   Writes a <see cref="Span{T}"/> of <typeparamref name="T"/> to the block at the specified byte address.
		/// </summary>
		/// <param name="byteAddress">Address to begin writing at. Will always be on a word boundary.</param>
		/// <param name="src"><see cref="Span{T}"/> of <typeparamref name="T"/> to be written to the block.</param>
		/// <exception cref="IOException">An error occurred writing to the underlying block.</exception>
		Task WriteAsync(int byteAddress, Memory<T> src);
	}
}
