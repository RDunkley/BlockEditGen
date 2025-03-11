// ******************************************************************************************************************************
// Filename:    Enum.AutoGen.cs
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
	/// <summary>Enumerates various possible values for a set of bits.</summary>
	//****************************************************************************************************************************
	public partial class Enum
	{
		#region Properties

		//************************************************************************************************************************
		/// <summary>Gets or sets the child XML elements.</summary>
		//************************************************************************************************************************
		public Item[] ChildItems { get; private set; }

		//************************************************************************************************************************
		/// <summary>
		///   Gets or sets the identification string of this enumeration. Must be unique across all enumeration elements.
		/// </summary>
		//************************************************************************************************************************
		public string Id { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets the index of this object in relation to the other child element of this object's parent.</summary>
		///
		/// <remarks>
		///   If the value is -1, then this object was not created from an XML node and the property has not been set.
		/// </remarks>
		//************************************************************************************************************************
		public int Ordinal { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets the width of the enumerated values.</summary>
		//************************************************************************************************************************
		public string Width { get; set; }

		#endregion Properties

		#region Methods

		//************************************************************************************************************************
		/// <overloads><summary>Instantiates a new <see cref="Enum"/> object.</summary></overloads>
		///
		/// <summary>Instantiates a new <see cref="Enum"/> object using the provided information.</summary>
		///
		/// <param name="id">'id' String attribute contained in the XML element.</param>
		/// <param name="width">'width' String attribute contained in the XML element.</param>
		/// <param name="childItems">Array of item elements which are child elements of this node. Can be empty.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="id"/>, or <paramref name="width"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="id"/>, <paramref name="width"/>, or <paramref name="childItems"/> is a null reference.
		/// </exception>
		//************************************************************************************************************************
		public Enum(string id, string width, Item[] childItems)
		{
			if(id == null)
				throw new ArgumentNullException("id");
			if(id.Length == 0)
				throw new ArgumentException("id is empty");
			if(width == null)
				throw new ArgumentNullException("width");
			if(width.Length == 0)
				throw new ArgumentException("width is empty");
			if(childItems == null)
				throw new ArgumentNullException("childItems");
			Id = id;
			Width = width;
			ChildItems = childItems;
			Ordinal = -1;

			// Compute the maximum index used on any child items.
			int maxIndex = 0;
			foreach(Item item in ChildItems)
			{
				if(item.Ordinal >= maxIndex)
					maxIndex = item.Ordinal + 1; // Set to first index after this index.
			}

			// Assign ordinal for any child items that don't have it set (-1).
			foreach(Item item in ChildItems)
			{
				if(item.Ordinal == -1)
					item.Ordinal = maxIndex++;
			}
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Enum"/> empty object.</summary>
		///
		/// <param name="id">'id' String attribute contained in the XML element.</param>
		/// <param name="width">'width' String attribute contained in the XML element.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="id"/>, or <paramref name="width"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="id"/>, or <paramref name="width"/> is a null reference.
		/// </exception>
		//************************************************************************************************************************
		public Enum(string id, string width)
		{
			if(id == null)
				throw new ArgumentNullException("id");
			if(id.Length == 0)
				throw new ArgumentException("id is empty");
			if(width == null)
				throw new ArgumentNullException("width");
			if(width.Length == 0)
				throw new ArgumentException("width is empty");
			Id = id;
			Width = width;
			ChildItems = new Item[0];
			Ordinal = -1;
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Enum"/> object from an <see cref="XmlNode"/> object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException">
		///   <paramref name="node"/> does not correspond to a enum node or is not an 'Element' type node or <paramref
		///   name="ordinal"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public Enum(XmlNode node, int ordinal)
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
		/// <summary>Adds a <see cref="Item"/> to <see cref="ChildItems"/>.</summary>
		///
		/// <param name="item"><see cref="Item"/> to be added. If null, then no changes will occur. Can be null.</param>
		//************************************************************************************************************************
		public void AddItem(Item item)
		{
			if (item == null) return;

			// Compute the maximum index used on any child items.
			int maxIndex = 0;
			foreach(Item child in ChildItems)
			{
				if (child.Ordinal >= maxIndex)
					maxIndex = child.Ordinal + 1; // Set to first index after this index.
			}

			var list = new List<Item>(ChildItems);
			list.Add(item);
			item.Ordinal = maxIndex;
			ChildItems = list.ToArray();
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
			XmlElement returnElement = doc.CreateElement("enum");

			string valueString;

			// id
			valueString = GetIdString();
			returnElement.SetAttribute("id", valueString);

			// width
			valueString = GetWidthString();
			returnElement.SetAttribute("width", valueString);
			// Build up dictionary of indexes and corresponding items.
			Dictionary<int, object> lookup = new Dictionary<int, object>();

			foreach(Item child in ChildItems)
			{
				if(lookup.ContainsKey(child.Ordinal))
					throw new InvalidOperationException("An attempt was made to generate the XML element with two child elements"
						+ " with the same ordinal.Ordinals must be unique across all child objects.");
				lookup.Add(child.Ordinal, child);
			}

			// Sort the keys.
			List<int> keys = lookup.Keys.ToList();
			keys.Sort();

			foreach (int key in keys)
			{
				if(lookup[key] is Item)
					returnElement.AppendChild(((Item)lookup[key]).CreateElement(doc));
			}
			return returnElement;
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
		/// <summary>Gets a string representation of Width.</summary>
		///
		/// <returns>String representing the value.</returns>
		//************************************************************************************************************************
		public string GetWidthString()
		{
			return Width;
		}

		//************************************************************************************************************************
		/// <summary>Parses an XML node and populates the data into this object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="node"/> does not correspond to a enum node.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public void ParseXmlNode(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(string.Compare(node.Name, "enum", false) != 0)
				throw new ArgumentException("node does not correspond to a enum node.");

			XmlAttribute attrib;

			// id
			attrib = node.Attributes["id"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (id) is not optional, but was not found in the XML"
					+ " element (enum).");
			SetIdFromString(attrib.Value);

			// width
			attrib = node.Attributes["width"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (width) is not optional, but was not found in the XML"
					+ " element (enum).");
			SetWidthFromString(attrib.Value);

			// Read the child objects.
			List<Item> childItemsList = new List<Item>();
			int index = 0;
			foreach(XmlNode child in node.ChildNodes)
			{
				if(child.NodeType == XmlNodeType.Element && child.Name == "item")
					childItemsList.Add(new Item(child, index++));
			}
			ChildItems = childItemsList.ToArray();

			Ordinal = ordinal;
		}

		//************************************************************************************************************************
		/// <summary>Removes a <see cref="Item"/> from <see cref="ChildItems"/>.</summary>
		///
		/// <param name="item"><see cref="Item"/> to be removed. Can be null.</param>
		//************************************************************************************************************************
		public void RemoveItem(Item item)
		{
			if (item == null) return;

			var list = new List<Item>(ChildItems);
			list.Remove(item);
			ChildItems = list.ToArray();
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
		/// <summary>Parses a string value and stores the data in Width.</summary>
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
		public void SetWidthFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'width' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'width' is an empty string.");
			Width = value;
		}

		#endregion Methods
	}
}
