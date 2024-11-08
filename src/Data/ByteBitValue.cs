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
using System.Globalization;

namespace BlockEditGen.Data
{
	public class ByteBitValue
	{
		public int Bytes { get; private set; }
		public int Bits { get; private set; }

		public int TotalBits { get; private set; }

		public ByteBitValue(string value)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));

			if (value.Contains('.'))
			{
				var splits = value.Split('.');
				if (splits.Length != 2)
					throw new ArgumentException($"The value string ({value}) contained a '.' but doesn't seem to have one to divide the bytes from the bits (Ex: <bytes>.<bits>).");
				if (!TryParseValue(splits[0], out uint bytes))
					throw new ArgumentException($"The value string ({value}) bytes portion ({splits[0]}) could not be parsed as such.");
				if (!TryParseValue(splits[1], out uint bits))
					throw new ArgumentException($"The value string ({value}) bits portion ({splits[1]}) could not be parsed as such.");

				Bits = (byte)(bits % 8);
				var temp = bytes + bits / 8;
				if (temp > int.MaxValue)
					throw new ArgumentException($"The value string ({value}) was parsed to a byte value ({temp:N0}) larger than allowed ({int.MaxValue:N0}).");
				Bytes = (int)temp;
			}
			else
			{
				// Parse as bytes.
				if (!TryParseValue(value, out uint allBytes))
					throw new ArgumentException($"Unable to parse the value ({value}) as a number (in bytes).");
				if (allBytes > int.MaxValue)
					throw new ArgumentException($"The value string ({value}) was parsed to a byte value ({allBytes:N0}) larger than allowed ({int.MaxValue:N0}).");
				Bits = 0;
				Bytes = (int)allBytes;
			}

			long sizeInBits = (long)Bytes * 8 + Bits;
			if (sizeInBits > int.MaxValue)
				throw new ArgumentException($"The value string ({value}) was parsed to a number of bits ({sizeInBits:N0}) that is larger than the maximum allowed ({int.MaxValue:N0}");

			TotalBits = (int)sizeInBits;
		}

		public ByteBitValue(int bytes, int bits)
		{
			if (bytes < 0) throw new ArgumentException($"The number of bytes ({bytes}) is less than 0.");
			if (bits < 0) throw new ArgumentException($"The number of bits ({bits}) is less than 0.");

			long sizeInBits = (long)bytes * 8 + bits;
			if (sizeInBits > int.MaxValue)
				throw new ArgumentException($"The number of bytes ({bytes:N0}) and bits ({bits:N0}) is larger than the maximum allowed bits ({int.MaxValue:N0}");
			TotalBits = (int)sizeInBits;

			Bits = bits % 8;
			Bytes = bytes + (bits / 8);
		}

		public ByteBitValue(int bits)
		{
			Bits = bits % 8;
			Bytes = bits / 8;
			TotalBits = bits;
		}

		public void AddBits(int bits)
		{
			Bits += bits;
			Bytes += Bits / 8;
			Bits = Bits % 8;
			TotalBits += bits;
		}

		public void AddBytes(int bytes)
		{
			Bytes += bytes;
			TotalBits += bytes * 8;
		}

		public static bool TryParseValue(string value, out uint result)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));

			value = value.Replace("_", string.Empty).ToLower();
			if (value.Length > 1 && value[value.Length - 1] == 'h') // Number is a hexadecimal type 1 number (FFh).
				return uint.TryParse(value.Substring(0, value.Length - 1), NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out result);

			if (value.Length > 2 && value[0] == '0' && value[1] == 'x') // Number is specified as a hexadecimal type 2 number (0xFF).
				return uint.TryParse(value.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.CurrentCulture, out result);

			if (value.Length > 1 && char.ToLower(value[value.Length - 1]) == 'b') // Number is a binary number.
			{
				try
				{
					result = Convert.ToUInt32(value.Substring(0, value.Length - 1), 2);
					return true;
				}
				catch (Exception e) when (e is FormatException || e is OverflowException)
				{
					result = 0;
					return false;
				}
			}

			// Attempt to parse the number as just an integer.
			return uint.TryParse(value, NumberStyles.Integer | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out result);
		}

		public static bool operator <(ByteBitValue left, ByteBitValue right)
		{
			return left.TotalBits < right.TotalBits;
		}

		public static bool operator >(ByteBitValue left, ByteBitValue right)
		{
			return left.TotalBits > right.TotalBits;
		}

		public static bool operator <=(ByteBitValue left, ByteBitValue right)
		{
			return left.TotalBits <= right.TotalBits;
		}

		public static bool operator >=(ByteBitValue left, ByteBitValue right)
		{
			return left.TotalBits >= right.TotalBits;
		}

		public static bool operator <(ByteBitValue left, int numBytesRight)
		{
			return left.TotalBits < numBytesRight * 8;
		}

		public static bool operator <(int numBytesLeft, ByteBitValue right)
		{
			return numBytesLeft * 8 < right.TotalBits;
		}

		public static bool operator >(ByteBitValue left, int numBytesRight)
		{
			return left.TotalBits > numBytesRight * 8;
		}

		public static bool operator >(int numBytesLeft, ByteBitValue right)
		{
			return numBytesLeft * 8 > right.TotalBits;
		}

		public static bool operator <=(ByteBitValue left, int numBytesRight)
		{
			return left.TotalBits <= numBytesRight * 8;
		}

		public static bool operator <=(int numBytesLeft, ByteBitValue right)
		{
			return numBytesLeft * 8 <= right.TotalBits;
		}

		public static bool operator >=(ByteBitValue left, int numBytesRight)
		{
			return left.TotalBits >= numBytesRight * 8;
		}

		public static bool operator >=(int numBytesLeft, ByteBitValue right)
		{
			return numBytesLeft * 8 >= right.TotalBits;
		}

		public static ByteBitValue operator +(ByteBitValue left, ByteBitValue right)
		{
			return new ByteBitValue(left.TotalBits + right.TotalBits);
		}

		public static ByteBitValue operator -(ByteBitValue left, ByteBitValue right)
		{
			return new ByteBitValue(left.TotalBits - right.TotalBits);
		}

		public static bool operator ==(ByteBitValue left, ByteBitValue right)
		{
			if (left is not null && right is null)
				return false;
			if (left is null && right is not null)
				return false;
			return left.TotalBits == right.TotalBits;
		}

		public static bool operator !=(ByteBitValue left, ByteBitValue right)
		{
			if (left is not null && right is null)
				return true;
			if (left is null && right is not null)
				return true;
			return left.TotalBits != right.TotalBits;
		}

		public override string ToString()
		{
			if (Bits == 0)
				return $"0x{Bytes:X}";
			return $"0x{Bytes:X}.{Bits}";
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
