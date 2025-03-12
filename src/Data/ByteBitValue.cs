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
using System.Globalization;

namespace BlockEditGen.Data
{
	/// <summary>
	///   Contains a numeric value representing a number of bytes and a number of bits.
	/// </summary>
	public class ByteBitValue
	{
		#region Properties

		/// <summary>
		///   Number of bytes in the value.
		/// </summary>
		public int Bytes { get; private set; }

		/// <summary>
		///   Number of bits in the value (in addition to the bytes). Only 0-7.
		/// </summary>
		public int Bits { get; private set; }

		/// <summary>
		///   Total number of bits in the value.
		/// </summary>
		public int TotalBits { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		///   Generates a <see cref="ByteBitValue"/> object from the provided string.
		/// </summary>
		/// <param name="value">
		///   String value containing the byte/word size and number of bits separated by a period. The first value can be hexadecimal or integer. The number of bits is optional.
		///   If a second is provided it is separated by a period and the second value represents the number of bits.
		/// </param>
		/// <param name="addressMultiplier">
		///   If an address multiplier is provided (other than 1) then the first number that is provided in <paramref name="value"/> will be multiplied by this value. This enables
		///   the use of addresses that are word addressable to be provided and internally are converted to bytes.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
		/// <exception cref="ArgumentException"><paramref name="addressMultiplier"/> is not 1, 2, 4, or 8 or <paramref name="value"/> could not be parsed.</exception>
		public ByteBitValue(string value, int addressMultiplier = 1)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));
			if (addressMultiplier != 1 && addressMultiplier != 2 && addressMultiplier != 4 && addressMultiplier != 8)
				throw new ArgumentException($"The provided address multiplier ({addressMultiplier}) is not 1, 2, 4, or 8");

			if (value.Contains('.'))
			{
				var splits = value.Split('.');
				if (splits.Length != 2)
					throw new ArgumentException($"The value string ({value}) contained a '.' but doesn't seem to have one to divide the bytes from the bits (Ex: <bytes>.<bits>).");
				if (!TryParseValue(splits[0], out uint bytes))
					throw new ArgumentException($"The value string ({value}) bytes portion ({splits[0]}) could not be parsed as such.");
				if (!TryParseValue(splits[1], out uint bits))
					throw new ArgumentException($"The value string ({value}) bits portion ({splits[1]}) could not be parsed as such.");

				// If an address multiplier other than 1 is provided, then it is assumed the number preceeding the period is a word address value so we need
				// to convert it to bytes.
				if (addressMultiplier != 1)
					bytes = (uint)(bytes * addressMultiplier);

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

				// If an address multiplier other than 1 is provided, then it is assumed the number preceeding the period is a word address value so we need
				// to convert it to bytes.
				if (addressMultiplier != 1)
					allBytes = (uint)(allBytes * addressMultiplier);

				if (allBytes > int.MaxValue)
					throw new ArgumentException($"The value string ({value}) was parsed to a byte value ({allBytes:N0}) larger than allowed ({int.MaxValue:N0}).");
				Bytes = (int)allBytes;
			}

			long sizeInBits = (long)Bytes * 8 + Bits;
			if (sizeInBits > int.MaxValue)
				throw new ArgumentException($"The value string ({value}) was parsed to a number of bits ({sizeInBits:N0}) that is larger than the maximum allowed ({int.MaxValue:N0}");

			TotalBits = (int)sizeInBits;
		}

		/// <summary>
		///   Creates a new <see cref="ByteBitValue"/> object from specified bytes and bits.
		/// </summary>
		/// <param name="bytes">Number of bytes in the value.</param>
		/// <param name="bits">Number of bits in the value in addition to the <paramref name="bytes"/>.</param>
		/// <exception cref="ArgumentException"><paramref name="bits"/> or <paramref name="bytes"/> is less than 0 or larger than the maximum allowed.</exception>
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

		#endregion
	}
}
