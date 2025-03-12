// ******************************************************************************************************************************
// Filename:    Conv.AutoGen.cs
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
	/// <summary>Specifies conversion parameters to be applied to values pushed or pulled from memory.</summary>
	//****************************************************************************************************************************
	public partial class Conv
	{
		#region Enumerations

		//************************************************************************************************************************
		/// <summary>Represents the formats the Gain can be parsed from</summary>
		//************************************************************************************************************************
		public enum GainFloatFormat
		{
			#region Names

			//********************************************************************************************************************
			/// <summary>Floating point number containing an exponent (Ex: '123E45', '123E+45', or '123E-45').</summary>
			//********************************************************************************************************************
			Exponent,

			//********************************************************************************************************************
			/// <summary>Decimal number (Ex: 1,234.5, 1234.567 or -1234.7).</summary>
			//********************************************************************************************************************
			Decimal,

			#endregion Names
		}

		//************************************************************************************************************************
		/// <summary>Represents the formats the Offset can be parsed from</summary>
		//************************************************************************************************************************
		public enum OffsetFloatFormat
		{
			#region Names

			//********************************************************************************************************************
			/// <summary>Floating point number containing an exponent (Ex: '123E45', '123E+45', or '123E-45').</summary>
			//********************************************************************************************************************
			Exponent,

			//********************************************************************************************************************
			/// <summary>Decimal number (Ex: 1,234.5, 1234.567 or -1234.7).</summary>
			//********************************************************************************************************************
			Decimal,

			#endregion Names
		}

		#endregion Enumerations

		#region Properties

		//************************************************************************************************************************
		/// <summary>
		///   Gets or sets the gain to be applied to the register value. If not provided, then no gain will be applied. Can be
		///   null.
		/// </summary>
		//************************************************************************************************************************
		public double? Gain { get; set; }

		//************************************************************************************************************************
		/// <summary>Determines what format the Gain floating type should be converted to in the XML string</summary>
		//************************************************************************************************************************
		public GainFloatFormat GainFormat { get; set; }

		//************************************************************************************************************************
		/// <summary>
		///   Gets or sets the identification string of this conversion. Must be unique across all conversion elements.
		/// </summary>
		//************************************************************************************************************************
		public string Id { get; set; }

		//************************************************************************************************************************
		/// <summary>
		///   Gets the offset associated with the conversion. If not provided, no offset will be added. Can be null.
		/// </summary>
		//************************************************************************************************************************
		public double? Offset { get; set; }

		//************************************************************************************************************************
		/// <summary>Determines what format the Offset floating type should be converted to in the XML string</summary>
		//************************************************************************************************************************
		public OffsetFloatFormat OffsetFormat { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets the index of this object in relation to the other child element of this object's parent.</summary>
		///
		/// <remarks>
		///   If the value is -1, then this object was not created from an XML node and the property has not been set.
		/// </remarks>
		//************************************************************************************************************************
		public int Ordinal { get; set; }

		#endregion Properties

		#region Methods

		//************************************************************************************************************************
		/// <overloads><summary>Instantiates a new <see cref="Conv"/> object.</summary></overloads>
		///
		/// <summary>Instantiates a new <see cref="Conv"/> object using the provided information.</summary>
		///
		/// <param name="gain">
		///   'gain' Double Precision (64-bit) floating point number attribute contained in the XML element. Can be null.
		/// </param>
		/// <param name="id">'id' String attribute contained in the XML element.</param>
		/// <param name="offset">
		///   'offset' Double Precision (64-bit) floating point number attribute contained in the XML element. Can be null.
		/// </param>
		///
		/// <exception cref="ArgumentException"><paramref name="id"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="id"/> is a null reference.</exception>
		//************************************************************************************************************************
		public Conv(double? gain, string id, double? offset)
		{
			if(id == null)
				throw new ArgumentNullException("id");
			if(id.Length == 0)
				throw new ArgumentException("id is empty");
			Gain = gain;
			Id = id;
			Offset = offset;
			Ordinal = -1;
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Conv"/> object from an <see cref="XmlNode"/> object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException">
		///   <paramref name="node"/> does not correspond to a conv node or is not an 'Element' type node or <paramref
		///   name="ordinal"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public Conv(XmlNode node, int ordinal)
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
			XmlElement returnElement = doc.CreateElement("conv");

			string valueString;

			// gain
			valueString = GetGainString();
			if(valueString != null)
				returnElement.SetAttribute("gain", valueString);

			// id
			valueString = GetIdString();
			returnElement.SetAttribute("id", valueString);

			// offset
			valueString = GetOffsetString();
			if(valueString != null)
				returnElement.SetAttribute("offset", valueString);
			return returnElement;
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Gain.</summary>
		///
		/// <returns>String representing the value. Can be null.</returns>
		//************************************************************************************************************************
		public string GetGainString()
		{
			if(!Gain.HasValue)
				return null;

			if(GainFormat == GainFloatFormat.Exponent)
				return Gain.Value.ToString("E");
				else
				return Gain.Value.ToString("N");
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Id.</summary>
		///
		/// <returns>String representing the value.</returns>
		//************************************************************************************************************************
		public string GetIdString()
		{
			return Id;
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Offset.</summary>
		///
		/// <returns>String representing the value. Can be null.</returns>
		//************************************************************************************************************************
		public string GetOffsetString()
		{
			if(!Offset.HasValue)
				return null;

			if(OffsetFormat == OffsetFloatFormat.Exponent)
				return Offset.Value.ToString("E");
				else
				return Offset.Value.ToString("N");
		}

		//************************************************************************************************************************
		/// <summary>Parses an XML node and populates the data into this object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="node"/> does not correspond to a conv node.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public void ParseXmlNode(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(string.Compare(node.Name, "conv", false) != 0)
				throw new ArgumentException("node does not correspond to a conv node.");

			XmlAttribute attrib;

			// gain
			attrib = node.Attributes["gain"];
			if(attrib == null)
				Gain = null;
			else
				SetGainFromString(attrib.Value);

			// id
			attrib = node.Attributes["id"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (id) is not optional, but was not found in the XML"
					+ " element (conv).");
			SetIdFromString(attrib.Value);

			// offset
			attrib = node.Attributes["offset"];
			if(attrib == null)
				Offset = null;
			else
				SetOffsetFromString(attrib.Value);
			Ordinal = ordinal;
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Gain.</summary>
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
		public void SetGainFromString(string value)
		{
			if(value == null)
			{
				Gain = null;
				return;
			}
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'gain' is an empty string.");
			double returnValue = 0;
			value = value.Trim();
			try
			{

				if(value.Contains("E") || value.Contains("e"))
				{
					// Number is an exponent.
					returnValue = double.Parse(value, NumberStyles.Float);
					GainFormat = GainFloatFormat.Exponent;
				}

				else
				{
					// Attempt to parse the number as just an decimal.
					returnValue = double.Parse(value, NumberStyles.Number);
					GainFormat = GainFloatFormat.Decimal;
				}
			}
			catch(FormatException e)
			{
				throw new InvalidDataException(string.Format("The double value specified ({0}) is not in a valid double string"
					+ " format: {1}.", value, e.Message), e);
			}
			catch(OverflowException e)
			{
				throw new InvalidDataException(string.Format("The double value specified ({0}) was larger or smaller than a"
					+ " double value (Min: {1}, Max: {2}).", value, double.MinValue.ToString(), double.MaxValue.ToString()), e);
			}

			Gain = returnValue;
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Id.</summary>
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
		public void SetIdFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'id' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'id' is an empty string.");
			Id = value;
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Offset.</summary>
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
		public void SetOffsetFromString(string value)
		{
			if(value == null)
			{
				Offset = null;
				return;
			}
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'offset' is an empty string.");
			double returnValue = 0;
			value = value.Trim();
			try
			{

				if(value.Contains("E") || value.Contains("e"))
				{
					// Number is an exponent.
					returnValue = double.Parse(value, NumberStyles.Float);
					OffsetFormat = OffsetFloatFormat.Exponent;
				}

				else
				{
					// Attempt to parse the number as just an decimal.
					returnValue = double.Parse(value, NumberStyles.Number);
					OffsetFormat = OffsetFloatFormat.Decimal;
				}
			}
			catch(FormatException e)
			{
				throw new InvalidDataException(string.Format("The double value specified ({0}) is not in a valid double string"
					+ " format: {1}.", value, e.Message), e);
			}
			catch(OverflowException e)
			{
				throw new InvalidDataException(string.Format("The double value specified ({0}) was larger or smaller than a"
					+ " double value (Min: {1}, Max: {2}).", value, double.MinValue.ToString(), double.MaxValue.ToString()), e);
			}

			Offset = returnValue;
		}

		#endregion Methods
	}
}
