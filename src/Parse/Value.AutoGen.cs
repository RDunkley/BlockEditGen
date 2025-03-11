// ******************************************************************************************************************************
// Filename:    Value.AutoGen.cs
// Owner:       Richard Dunkley
// Generated using XMLToDataClass version 1.1.0 with CSCodeGen.dll version 1.0.0.
// Copyright © Richard Dunkley 2024
// ******************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BlockEditGen.Parse
{
	//****************************************************************************************************************************
	/// <summary>Represents a displayable value in the block's register space.</summary>
	//****************************************************************************************************************************
	public partial class Value
	{
		#region Enumerations

		//************************************************************************************************************************
		/// <summary>Enumerates the possible values of Access.</summary>
		//************************************************************************************************************************
		public enum AccessEnum
		{
			#region Names

			//********************************************************************************************************************
			/// <summary>Represents the 'R' string.</summary>
			//********************************************************************************************************************
			Read,

			//********************************************************************************************************************
			/// <summary>Represents the 'RW' string.</summary>
			//********************************************************************************************************************
			ReadWrite,

			//********************************************************************************************************************
			/// <summary>Represents the 'W' string.</summary>
			//********************************************************************************************************************
			Write,

			#endregion Names
		}

		//************************************************************************************************************************
		/// <summary>Enumerates the possible values of Type.</summary>
		//************************************************************************************************************************
		public enum TypeEnum
		{
			#region Names

			//********************************************************************************************************************
			/// <summary>Represents the 'bool' string.</summary>
			//********************************************************************************************************************
			Bool,

			//********************************************************************************************************************
			/// <summary>Represents the 'string' string.</summary>
			//********************************************************************************************************************
			String,

			//********************************************************************************************************************
			/// <summary>Represents the 'uint8' string.</summary>
			//********************************************************************************************************************
			Uint8,

			//********************************************************************************************************************
			/// <summary>Represents the 'enum' string.</summary>
			//********************************************************************************************************************
			Enum,

			//********************************************************************************************************************
			/// <summary>Represents the 'int8' string.</summary>
			//********************************************************************************************************************
			Int8,

			//********************************************************************************************************************
			/// <summary>Represents the 'int32' string.</summary>
			//********************************************************************************************************************
			Int32,

			//********************************************************************************************************************
			/// <summary>Represents the 'ip' string.</summary>
			//********************************************************************************************************************
			Ip,

			//********************************************************************************************************************
			/// <summary>Represents the 'mac' string.</summary>
			//********************************************************************************************************************
			Mac,

			//********************************************************************************************************************
			/// <summary>Represents the 'double' string.</summary>
			//********************************************************************************************************************
			Double,

			//********************************************************************************************************************
			/// <summary>Represents the 'float' string.</summary>
			//********************************************************************************************************************
			Float,

			//********************************************************************************************************************
			/// <summary>Represents the 'int16' string.</summary>
			//********************************************************************************************************************
			Int16,

			//********************************************************************************************************************
			/// <summary>Represents the 'uint16' string.</summary>
			//********************************************************************************************************************
			Uint16,

			//********************************************************************************************************************
			/// <summary>Represents the 'uint32' string.</summary>
			//********************************************************************************************************************
			Uint32,

			//********************************************************************************************************************
			/// <summary>Represents the 'int64' string.</summary>
			//********************************************************************************************************************
			Int64,

			//********************************************************************************************************************
			/// <summary>Represents the 'uint64' string.</summary>
			//********************************************************************************************************************
			Uint64,

			#endregion Names
		}

		#endregion Enumerations

		#region Properties

		//************************************************************************************************************************
		/// <summary>Gets or sets the accessability of the value. Can be null.</summary>
		//************************************************************************************************************************
		public AccessEnum? Access { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets or sets the starting location (address) of the value in bytes and bits.</summary>
		//************************************************************************************************************************
		public string Addr { get; set; }

		//************************************************************************************************************************
		/// <summary>
		///   Gets or sets the conversion applied to the value when converting to user displayable to data written. Can be null.
		/// </summary>
		//************************************************************************************************************************
		public string Conv { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets or sets the name of the value. This is what will be displayed in the UI.</summary>
		//************************************************************************************************************************
		public string Name { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets the index of this object in relation to the other child element of this object's parent.</summary>
		///
		/// <remarks>
		///   If the value is -1, then this object was not created from an XML node and the property has not been set.
		/// </remarks>
		//************************************************************************************************************************
		public int Ordinal { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets or sets the size of the value in bytes and bits.</summary>
		//************************************************************************************************************************
		public string Size { get; set; }

		//************************************************************************************************************************
		/// <summary>
		///   Gets or sets the subtype values of the value. The contents are based on and specific to the type. Can be null. Can
		///   be empty.
		/// </summary>
		//************************************************************************************************************************
		public string Subtype { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets or sets a description of the item portrayed in a tooltip in the UI. Can be null. Can be empty.</summary>
		//************************************************************************************************************************
		public string Tooltip { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets or sets the type of the value.</summary>
		//************************************************************************************************************************
		public TypeEnum Type { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets or sets the units of the value. Can be null.</summary>
		//************************************************************************************************************************
		public string Units { get; set; }

		#endregion Properties

		#region Methods

		//************************************************************************************************************************
		/// <overloads><summary>Instantiates a new <see cref="Value"/> object.</summary></overloads>
		///
		/// <summary>Instantiates a new <see cref="Value"/> object using the provided information.</summary>
		///
		/// <param name="access">'access' Custom Enumeration attribute contained in the XML element. Can be null.</param>
		/// <param name="addr">'addr' String attribute contained in the XML element.</param>
		/// <param name="conv">'conv' String attribute contained in the XML element. Can be null.</param>
		/// <param name="name">'name' String attribute contained in the XML element.</param>
		/// <param name="size">'size' String attribute contained in the XML element.</param>
		/// <param name="subtype">'subtype' String attribute contained in the XML element. Can be null. Can be empty.</param>
		/// <param name="tooltip">'tooltip' String attribute contained in the XML element. Can be null. Can be empty.</param>
		/// <param name="type">'type' Custom Enumeration attribute contained in the XML element.</param>
		/// <param name="units">'units' String attribute contained in the XML element. Can be null.</param>
		///
		/// <exception cref="ArgumentException">
		///   <paramref name="addr"/>, <paramref name="conv"/>, <paramref name="name"/>, <paramref name="size"/>, or <paramref
		///   name="units"/> is an empty array.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="addr"/>, <paramref name="name"/>, or <paramref name="size"/> is a null reference.
		/// </exception>
		//************************************************************************************************************************
		public Value(AccessEnum? access, string addr, string conv, string name, string size, string subtype, string tooltip,
			TypeEnum type, string units)
		{
			if(addr == null)
				throw new ArgumentNullException("addr");
			if(addr.Length == 0)
				throw new ArgumentException("addr is empty");
			if(conv != null && conv.Length == 0)
				throw new ArgumentException("conv is empty");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentException("name is empty");
			if(size == null)
				throw new ArgumentNullException("size");
			if(size.Length == 0)
				throw new ArgumentException("size is empty");
			if(units != null && units.Length == 0)
				throw new ArgumentException("units is empty");
			Access = access;
			Addr = addr;
			Conv = conv;
			Name = name;
			Size = size;
			Subtype = subtype;
			Tooltip = tooltip;
			Type = type;
			Units = units;
			Ordinal = -1;
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Value"/> object from an <see cref="XmlNode"/> object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException">
		///   <paramref name="node"/> does not correspond to a value node or is not an 'Element' type node or <paramref
		///   name="ordinal"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public Value(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(ordinal < 0)
				throw new ArgumentException("the ordinal specified is negative.");
			if(node.NodeType != XmlNodeType.Element)
				throw new ArgumentException("node is not of type 'Element'.");

			ParseXmlNode(node, ordinal);
		}

		//************************************************************************************************************************
		/// <summary>Creates an XML element for this object using the provided <see cref="XmlDocument"/> object.</summary>
		///
		/// <param name="doc"><see cref="XmlDocument"/> object to generate the element from.</param>
		///
		/// <returns><see cref="XmlElement"/> object containing this classes data.</returns>
		///
		/// <exception cref="ArgumentNullException"><paramref name="doc"/> is a null reference.</exception>
		//************************************************************************************************************************
		public XmlElement CreateElement(XmlDocument doc)
		{
			if(doc == null)
				throw new ArgumentNullException("doc");
			XmlElement returnElement = doc.CreateElement("value");

			string valueString;

			// access
			valueString = GetAccessString();
			if(valueString != null)
				returnElement.SetAttribute("access", valueString);

			// addr
			valueString = GetAddrString();
			returnElement.SetAttribute("addr", valueString);

			// conv
			valueString = GetConvString();
			if(valueString != null)
				returnElement.SetAttribute("conv", valueString);

			// name
			valueString = GetNameString();
			returnElement.SetAttribute("name", valueString);

			// size
			valueString = GetSizeString();
			returnElement.SetAttribute("size", valueString);

			// subtype
			valueString = GetSubtypeString();
			if(valueString != null)
				returnElement.SetAttribute("subtype", valueString);

			// tooltip
			valueString = GetTooltipString();
			if(valueString != null)
				returnElement.SetAttribute("tooltip", valueString);

			// type
			valueString = GetTypeString();
			returnElement.SetAttribute("type", valueString);

			// units
			valueString = GetUnitsString();
			if(valueString != null)
				returnElement.SetAttribute("units", valueString);
			return returnElement;
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Access.</summary>
		///
		/// <returns>String representing the value. Can be null.</returns>
		//************************************************************************************************************************
		public string GetAccessString()
		{
			if(!Access.HasValue)
				return null;

			switch(Access)
			{
				case AccessEnum.Read:
					return "R";
				case AccessEnum.ReadWrite:
					return "RW";
				case AccessEnum.Write:
					return "W";
				default:
					throw new NotImplementedException("The enumerated type was not recognized as a supported type.");
			}
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Addr.</summary>
		///
		/// <returns>String representing the value.</returns>
		//************************************************************************************************************************
		public string GetAddrString()
		{
			return Addr;
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Conv.</summary>
		///
		/// <returns>String representing the value. Can be null.</returns>
		//************************************************************************************************************************
		public string GetConvString()
		{
			return Conv;
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Name.</summary>
		///
		/// <returns>String representing the value.</returns>
		//************************************************************************************************************************
		public string GetNameString()
		{
			return Name;
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Size.</summary>
		///
		/// <returns>String representing the value.</returns>
		//************************************************************************************************************************
		public string GetSizeString()
		{
			return Size;
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Subtype.</summary>
		///
		/// <returns>String representing the value. Can be null. Can be empty.</returns>
		//************************************************************************************************************************
		public string GetSubtypeString()
		{
			return Subtype;
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Tooltip.</summary>
		///
		/// <returns>String representing the value. Can be null. Can be empty.</returns>
		//************************************************************************************************************************
		public string GetTooltipString()
		{
			return Tooltip;
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Type.</summary>
		///
		/// <returns>String representing the value.</returns>
		//************************************************************************************************************************
		public string GetTypeString()
		{

			switch(Type)
			{
				case TypeEnum.Bool:
					return "bool";
				case TypeEnum.String:
					return "string";
				case TypeEnum.Uint8:
					return "uint8";
				case TypeEnum.Enum:
					return "enum";
				case TypeEnum.Int8:
					return "int8";
				case TypeEnum.Int32:
					return "int32";
				case TypeEnum.Ip:
					return "ip";
				case TypeEnum.Mac:
					return "mac";
				case TypeEnum.Double:
					return "double";
				case TypeEnum.Float:
					return "float";
				case TypeEnum.Int16:
					return "int16";
				case TypeEnum.Uint16:
					return "uint16";
				case TypeEnum.Uint32:
					return "uint32";
				case TypeEnum.Int64:
					return "int64";
				case TypeEnum.Uint64:
					return "uint64";
				default:
					throw new NotImplementedException("The enumerated type was not recognized as a supported type.");
			}
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Units.</summary>
		///
		/// <returns>String representing the value. Can be null.</returns>
		//************************************************************************************************************************
		public string GetUnitsString()
		{
			return Units;
		}

		//************************************************************************************************************************
		/// <summary>Parses an XML node and populates the data into this object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="node"/> does not correspond to a value node.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public void ParseXmlNode(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(string.Compare(node.Name, "value", false) != 0)
				throw new ArgumentException("node does not correspond to a value node.");

			XmlAttribute attrib;

			// access
			attrib = node.Attributes["access"];
			if(attrib == null)
				Access = null;
			else
				SetAccessFromString(attrib.Value);

			// addr
			attrib = node.Attributes["addr"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (addr) is not optional, but was not found in the XML"
					+ " element (value).");
			SetAddrFromString(attrib.Value);

			// conv
			attrib = node.Attributes["conv"];
			if(attrib == null)
				Conv = null;
			else
				SetConvFromString(attrib.Value);

			// name
			attrib = node.Attributes["name"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (name) is not optional, but was not found in the XML"
					+ " element (value).");
			SetNameFromString(attrib.Value);

			// size
			attrib = node.Attributes["size"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (size) is not optional, but was not found in the XML"
					+ " element (value).");
			SetSizeFromString(attrib.Value);

			// subtype
			attrib = node.Attributes["subtype"];
			if(attrib == null)
				Subtype = null;
			else
				SetSubtypeFromString(attrib.Value);

			// tooltip
			attrib = node.Attributes["tooltip"];
			if(attrib == null)
				Tooltip = null;
			else
				SetTooltipFromString(attrib.Value);

			// type
			attrib = node.Attributes["type"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (type) is not optional, but was not found in the XML"
					+ " element (value).");
			SetTypeFromString(attrib.Value);

			// units
			attrib = node.Attributes["units"];
			if(attrib == null)
				Units = null;
			else
				SetUnitsFromString(attrib.Value);
			Ordinal = ordinal;
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Access.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item>The string value is an empty string.</item>
		///     <item>The string value could not be parsed.</item>
		///   </list>
		/// </exception>
		//************************************************************************************************************************
		public void SetAccessFromString(string value)
		{
			if(value == null)
			{
				Access = null;
				return;
			}
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'access' is an empty string.");
			if(string.Compare(value, "R", false) == 0)
			{
				Access = AccessEnum.Read;
				return;
			}
			if(string.Compare(value, "RW", false) == 0)
			{
				Access = AccessEnum.ReadWrite;
				return;
			}
			if(string.Compare(value, "W", false) == 0)
			{
				Access = AccessEnum.Write;
				return;
			}
			throw new InvalidDataException(string.Format("The enum value specified ({0}) is not a recognized enumerated type for"
				+ " access.", value));
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Addr.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item>The string value is a null reference or an empty string.</item>
		///     <item>The string value could not be parsed.</item>
		///   </list>
		/// </exception>
		//************************************************************************************************************************
		public void SetAddrFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'addr' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'addr' is an empty string.");
			Addr = value;
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Conv.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item>The string value is an empty string.</item>
		///     <item>The string value could not be parsed.</item>
		///   </list>
		/// </exception>
		//************************************************************************************************************************
		public void SetConvFromString(string value)
		{
			if(value == null)
			{
				Conv = null;
				return;
			}
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'conv' is an empty string.");
			Conv = value;
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Name.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item>The string value is a null reference or an empty string.</item>
		///     <item>The string value could not be parsed.</item>
		///   </list>
		/// </exception>
		//************************************************************************************************************************
		public void SetNameFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'name' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'name' is an empty string.");
			Name = value;
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Size.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item>The string value is a null reference or an empty string.</item>
		///     <item>The string value could not be parsed.</item>
		///   </list>
		/// </exception>
		//************************************************************************************************************************
		public void SetSizeFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'size' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'size' is an empty string.");
			Size = value;
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Subtype.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">The string value could not be parsed.</exception>
		//************************************************************************************************************************
		public void SetSubtypeFromString(string value)
		{
			if(value == null)
			{
				Subtype = null;
				return;
			}
			Subtype = value;
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Tooltip.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">The string value could not be parsed.</exception>
		//************************************************************************************************************************
		public void SetTooltipFromString(string value)
		{
			if(value == null)
			{
				Tooltip = null;
				return;
			}
			Tooltip = value;
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Type.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item>The string value is a null reference or an empty string.</item>
		///     <item>The string value could not be parsed.</item>
		///   </list>
		/// </exception>
		//************************************************************************************************************************
		public void SetTypeFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'type' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'type' is an empty string.");
			if(string.Compare(value, "bool", false) == 0)
			{
				Type = TypeEnum.Bool;
				return;
			}
			if(string.Compare(value, "string", false) == 0)
			{
				Type = TypeEnum.String;
				return;
			}
			if(string.Compare(value, "uint8", false) == 0)
			{
				Type = TypeEnum.Uint8;
				return;
			}
			if(string.Compare(value, "enum", false) == 0)
			{
				Type = TypeEnum.Enum;
				return;
			}
			if(string.Compare(value, "int8", false) == 0)
			{
				Type = TypeEnum.Int8;
				return;
			}
			if(string.Compare(value, "int32", false) == 0)
			{
				Type = TypeEnum.Int32;
				return;
			}
			if(string.Compare(value, "ip", false) == 0)
			{
				Type = TypeEnum.Ip;
				return;
			}
			if(string.Compare(value, "mac", false) == 0)
			{
				Type = TypeEnum.Mac;
				return;
			}
			if(string.Compare(value, "double", false) == 0)
			{
				Type = TypeEnum.Double;
				return;
			}
			if(string.Compare(value, "float", false) == 0)
			{
				Type = TypeEnum.Float;
				return;
			}
			if(string.Compare(value, "int16", false) == 0)
			{
				Type = TypeEnum.Int16;
				return;
			}
			if(string.Compare(value, "uint16", false) == 0)
			{
				Type = TypeEnum.Uint16;
				return;
			}
			if(string.Compare(value, "uint32", false) == 0)
			{
				Type = TypeEnum.Uint32;
				return;
			}
			if(string.Compare(value, "int64", false) == 0)
			{
				Type = TypeEnum.Int64;
				return;
			}
			if(string.Compare(value, "uint64", false) == 0)
			{
				Type = TypeEnum.Uint64;
				return;
			}
			throw new InvalidDataException(string.Format("The enum value specified ({0}) is not a recognized enumerated type for"
				+ " type.", value));
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Units.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item>The string value is an empty string.</item>
		///     <item>The string value could not be parsed.</item>
		///   </list>
		/// </exception>
		//************************************************************************************************************************
		public void SetUnitsFromString(string value)
		{
			if(value == null)
			{
				Units = null;
				return;
			}
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'units' is an empty string.");
			Units = value;
		}

		#endregion Methods
	}
}
