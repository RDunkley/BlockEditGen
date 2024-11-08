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
using System.Numerics;

namespace BlockEditGen.Data
{
	public class RamRegisterBlock<T> : IRegisterBlock<T> where T : struct, INumber<T>, IUnsignedNumber<T>
	{
		private Memory<T> _buf;
		private int _sizeOfReg;

		public bool CanWrite { get; private set; }
		public int SizeInBytes { get; private set; }

		public RamRegisterBlock(int sizeInBytes, bool isReadOnly = false)
		{
			if (typeof(T) == typeof(byte))
				_sizeOfReg = sizeof(byte);
			else if (typeof(T) == typeof(ushort))
				_sizeOfReg = sizeof(ushort);
			else if (typeof(T) == typeof(uint))
				_sizeOfReg = sizeof(uint);
			else if (typeof(T) == typeof(ulong))
				_sizeOfReg = sizeof(ulong);
			else
				throw new InvalidOperationException($"The type of the object is not valid.");

			_buf = new T[SizeInBytes / _sizeOfReg];
			CanWrite = !isReadOnly;
			SizeInBytes = sizeInBytes;
		}

		public async Task ReadAsync(int byteAddress, Memory<T> dst)
		{
			await Task.Run(() =>
			{
				_buf.Slice(byteAddress / _sizeOfReg, dst.Length).CopyTo(dst);
			});
		}

		public async Task WriteAsync(int byteAddress, Memory<T> src)
		{
			await Task.Run(() =>
			{
				src.CopyTo(_buf.Slice(byteAddress / _sizeOfReg, src.Length));
			});
		}
	}
}
