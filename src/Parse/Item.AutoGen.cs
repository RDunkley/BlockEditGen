// ******************************************************************************************************************************
// Filename:    Item.AutoGen.cs
// Owner:       Richard Dunkley
// Generated using XMLToDataClass version 1.1.0 with CSCodeGen.dll version 1.0.0.
// Copyright © Richard Dunkley 2024
// ******************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BlockEditGen.Parse
{
	//****************************************************************************************************************************
	/// <summary>Item in the enumeration.</summary>
	//****************************************************************************************************************************
	public partial class Item
	{
		#region Enumerations

		//************************************************************************************************************************
		/// <summary>Represents the formats the Value can be parsed from</summary>
		//************************************************************************************************************************
		public enum ValueIntegerFormat
		{
			#region Names

			//********************************************************************************************************************
			/// <summary>Hexadecimal number ending with an 'h' (Ex: FFh).</summary>
			//********************************************************************************************************************
			HexType1,

			//********************************************************************************************************************
			/// <summary>Hexadecimal number beginning with an '0x' (Ex: 0xFF).</summary>
			//********************************************************************************************************************
			HexType2,

			//********************************************************************************************************************
			/// <summary>Binary number ending with an 'b' (Ex: 0110b).</summary>
			//********************************************************************************************************************
			Binary,

			//********************************************************************************************************************
			/// <summary>Integer number (Ex: 1,234, 1234 or -1234).</summary>
			//********************************************************************************************************************
			Integer,

			#endregion Names
		}

		#endregion Enumerations

		#region Properties

		//************************************************************************************************************************
		/// <summary>Gets the name of the enumeration that is displayed.</summary>
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
		/// <summary>Gets or sets a description of the item portrayed in a tooltip in the UI. Can be null. Can be empty.</summary>
		//************************************************************************************************************************
		public string Tooltip { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets the numeric value of the item represented in the register bits.</summary>
		//************************************************************************************************************************
		public ulong Value { get; set; }

		//************************************************************************************************************************
		/// <summary>Determines what format the Value integral type should be converted to in the XML string</summary>
		//************************************************************************************************************************
		public ValueIntegerFormat ValueFormat { get; set; }

		#endregion Properties

		#region Methods

		//************************************************************************************************************************
		/// <overloads><summary>Instantiates a new <see cref="Item"/> object.</summary></overloads>
		///
		/// <summary>Instantiates a new <see cref="Item"/> object using the provided information.</summary>
		///
		/// <param name="name">'name' String attribute contained in the XML element.</param>
		/// <param name="tooltip">'tooltip' String attribute contained in the XML element. Can be null. Can be empty.</param>
		/// <param name="valueValue">'value' 64-bit unsigned integer attribute contained in the XML element.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="name"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is a null reference.</exception>
		//************************************************************************************************************************
		public Item(string name, string tooltip, ulong valueValue)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentException("name is empty");
			Name = name;
			Tooltip = tooltip;
			Value = valueValue;
			Ordinal = -1;
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Item"/> object from an <see cref="XmlNode"/> object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException">
		///   <paramref name="node"/> does not correspond to a item node or is not an 'Element' type node or <paramref
		///   name="ordinal"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public Item(XmlNode node, int ordinal)
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
			XmlElement returnElement = doc.CreateElement("item");

			string valueString;

			// name
			valueString = GetNameString();
			returnElement.SetAttribute("name", valueString);

			// tooltip
			valueString = GetTooltipString();
			if(valueString != null)
				returnElement.SetAttribute("tooltip", valueString);

			// value
			valueString = GetValueString();
			returnElement.SetAttribute("value", valueString);
			return returnElement;
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
		/// <summary>Gets a string representation of Tooltip.</summary>
		///
		/// <returns>String representing the value. Can be null. Can be empty.</returns>
		//************************************************************************************************************************
		public string GetTooltipString()
		{
			return Tooltip;
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Value.</summary>
		///
		/// <returns>String representing the value.</returns>
		//************************************************************************************************************************
		public string GetValueString()
		{

			if(ValueFormat == ValueIntegerFormat.HexType1)
				return string.Format("{0}h", Value.ToString("X"));
			else if(ValueFormat == ValueIntegerFormat.HexType2)
				return string.Format("0x{0}", Value.ToString("X"));
			else if(ValueFormat == ValueIntegerFormat.Binary)
				return string.Format("{0}b", Convert.ToString((long)Value, 2));
			else
				return Value.ToString();
		}

		//************************************************************************************************************************
		/// <summary>Parses an XML node and populates the data into this object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="node"/> does not correspond to a item node.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public void ParseXmlNode(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(string.Compare(node.Name, "item", false) != 0)
				throw new ArgumentException("node does not correspond to a item node.");

			XmlAttribute attrib;

			// name
			attrib = node.Attributes["name"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (name) is not optional, but was not found in the XML"
					+ " element (item).");
			SetNameFromString(attrib.Value);

			// tooltip
			attrib = node.Attributes["tooltip"];
			if(attrib == null)
				Tooltip = null;
			else
				SetTooltipFromString(attrib.Value);

			// value
			attrib = node.Attributes["value"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (value) is not optional, but was not found in the XML"
					+ " element (item).");
			SetValueFromString(attrib.Value);
			Ordinal = ordinal;
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
		/// <summary>Parses a string value and stores the data in Value.</summary>
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
		public void SetValueFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'value' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'value' is an empty string.");
			ulong returnValue = 0;
			bool parsed = false;
			try
			{

				if(value.Length > 1 && char.ToLower(value[value.Length - 1]) == 'h')
				{
					// Number is a hexadecimal type 1 number (FFh).
					returnValue = ulong.Parse(value.Substring(0, value.Length - 1), NumberStyles.AllowHexSpecifier);
					ValueFormat = ValueIntegerFormat.HexType1;
					parsed = true;
				}

				else if(value.Length > 2 && value[0] == '0' && char.ToLower(value[1]) == 'x')
				{
					// Number is specified as a hexadecimal type 2 number (0xFF).
					returnValue = ulong.Parse(value.Substring(2), NumberStyles.AllowHexSpecifier);
					ValueFormat = ValueIntegerFormat.HexType2;
					parsed = true;
				}

				else if(value.Length > 1 && char.ToLower(value[value.Length - 1]) == 'b')
				{
					// Number is a binary number.
					returnValue = Convert.ToUInt64(value.Substring(0, value.Length - 1), 2);
					ValueFormat = ValueIntegerFormat.Binary;
					parsed = true;
				}

				else
				{
					// Attempt to parse the number as just an integer.
					returnValue = ulong.Parse(value, NumberStyles.Integer | NumberStyles.AllowThousands);
					ValueFormat = ValueIntegerFormat.Integer;
					parsed = true;
				}
			}
			catch(FormatException e)
			{
				throw new InvalidDataException(string.Format("The ulong value specified ({0}) is not in a valid ulong string"
					+ " format: {1}.", value, e.Message), e);
			}
			catch(OverflowException e)
			{
				throw new InvalidDataException(string.Format("The ulong value specified ({0}) was larger or smaller than a ulong"
					+ " value (Min: {1}, Max: {2}).", value, ulong.MinValue.ToString(), ulong.MaxValue.ToString()), e);
			}

			if(!parsed)
				throw new InvalidDataException(string.Format("The ulong value specified ({0}) is not in a valid ulong string"
					+ " format.", value));

			Value = returnValue;
		}

		#endregion Methods
	}
}
