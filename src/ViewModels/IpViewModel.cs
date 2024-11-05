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
using BlockEditGen.Interfaces;
using BlockEditGen.Parse;
using System;
using System.Net;

namespace BlockEditGen.ViewModels
{
	public class IpViewModel : StringBasedViewModelBase
	{
		public IpViewModel()
			: this(
				new Value(Value.AccessEnum.ReadWrite, "0x00", null, "Test description of ip", "Test Name", "4", "4", Value.TypeEnum.Ip, null),
				new CachedRegisterBlock<byte>(new RamRegisterBlock<byte>(4096))
			)
		{
		}

		public IpViewModel(Value value, ICachedRegisterBlock block)
			: base(value, block)
		{
			if (value.Type != Value.TypeEnum.Ip)
				throw new ArgumentException($"The value provided ({value.Name}) is not an IP type.");

			if (value.Subtype != "4" && value.Subtype != "6")
				throw new ArgumentException($"The value provided ({value.Name}) did not contain a valid subtype ({value.Subtype}). The subtype should be 4 or 6 to represent IPv4 or IPv6 respectively.");

			if (value.Subtype == "4" && value.Length.TotalBits != 32)
				throw new ArgumentException($"The value provided ({value.Name}) is specified as IPv4, but the length of the value isn't 32 bits.");
			if (value.Subtype == "6" && value.Length.TotalBits != 128)
				throw new ArgumentException($"The value provided ({value.Name}) is specified as IPv6, but the length of the value isn't 128 bits.");
		}

		protected override string GetString()
		{
			var buf = new byte[LengthInBytes];
			_block.ReadSection(_value.Address, _value.Length, buf);
			var addr = new IPAddress(buf);
			return addr.ToString();
		}

		protected override bool TrySetString(string value)
		{
			if (value == null) return false;
			if (!IPAddress.TryParse(value, out IPAddress addr)) return false;
			if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && _value.Length.TotalBits != 32)
				return false;
			if (addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 && _value.Length.TotalBits != 128)
				return false;

			_block.WriteSection(_value.Address, _value.Length, addr.GetAddressBytes());
			return true;
		}
	}
}
