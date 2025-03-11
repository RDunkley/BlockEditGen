// ******************************************************************************************************************************
// Filename:    Group.AutoGen.cs
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
	/// <summary>In memory representation of the XML element "group".</summary>
	//****************************************************************************************************************************
	public partial class Group
	{
		#region Properties

		//************************************************************************************************************************
		/// <summary>Gets or sets the child XML elements.</summary>
		//************************************************************************************************************************
		public Value[] ChildValues { get; private set; }

		//************************************************************************************************************************
		/// <summary>Gets or sets the value of the child name component.</summary>
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

		#endregion Properties

		#region Methods

		//************************************************************************************************************************
		/// <overloads><summary>Instantiates a new <see cref="Group"/> object.</summary></overloads>
		///
		/// <summary>Instantiates a new <see cref="Group"/> object using the provided information.</summary>
		///
		/// <param name="name">'name' String attribute contained in the XML element.</param>
		/// <param name="childValues">Array of value elements which are child elements of this node. Can be empty.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="name"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="name"/>, or <paramref name="childValues"/> is a null reference.
		/// </exception>
		//************************************************************************************************************************
		public Group(string name, Value[] childValues)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentException("name is empty");
			if(childValues == null)
				throw new ArgumentNullException("childValues");
			Name = name;
			ChildValues = childValues;
			Ordinal = -1;

			// Compute the maximum index used on any child items.
			int maxIndex = 0;
			foreach(Value item in ChildValues)
			{
				if(item.Ordinal >= maxIndex)
					maxIndex = item.Ordinal + 1; // Set to first index after this index.
			}

			// Assign ordinal for any child items that don't have it set (-1).
			foreach(Value item in ChildValues)
			{
				if(item.Ordinal == -1)
					item.Ordinal = maxIndex++;
			}
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Group"/> empty object.</summary>
		///
		/// <param name="name">'name' String attribute contained in the XML element.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="name"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="name"/> is a null reference.</exception>
		//************************************************************************************************************************
		public Group(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentException("name is empty");
			Name = name;
			ChildValues = new Value[0];
			Ordinal = -1;
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Group"/> object from an <see cref="XmlNode"/> object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException">
		///   <paramref name="node"/> does not correspond to a group node or is not an 'Element' type node or <paramref
		///   name="ordinal"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public Group(XmlNode node, int ordinal)
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
		/// <summary>Adds a <see cref="Value"/> to <see cref="ChildValues"/>.</summary>
		///
		/// <param name="item"><see cref="Value"/> to be added. If null, then no changes will occur. Can be null.</param>
		//************************************************************************************************************************
		public void AddValue(Value item)
		{
			if (item == null) return;

			// Compute the maximum index used on any child items.
			int maxIndex = 0;
			foreach(Value child in ChildValues)
			{
				if (child.Ordinal >= maxIndex)
					maxIndex = child.Ordinal + 1; // Set to first index after this index.
			}

			var list = new List<Value>(ChildValues);
			list.Add(item);
			item.Ordinal = maxIndex;
			ChildValues = list.ToArray();
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
			XmlElement returnElement = doc.CreateElement("group");

			string valueString;

			// name
			valueString = GetNameString();
			returnElement.SetAttribute("name", valueString);
			// Build up dictionary of indexes and corresponding items.
			Dictionary<int, object> lookup = new Dictionary<int, object>();

			foreach(Value child in ChildValues)
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
				if(lookup[key] is Value)
					returnElement.AppendChild(((Value)lookup[key]).CreateElement(doc));
			}
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
		/// <summary>Parses an XML node and populates the data into this object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="node"/> does not correspond to a group node.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public void ParseXmlNode(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(string.Compare(node.Name, "group", false) != 0)
				throw new ArgumentException("node does not correspond to a group node.");

			XmlAttribute attrib;

			// name
			attrib = node.Attributes["name"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (name) is not optional, but was not found in the XML"
					+ " element (group).");
			SetNameFromString(attrib.Value);

			// Read the child objects.
			List<Value> childValuesList = new List<Value>();
			int index = 0;
			foreach(XmlNode child in node.ChildNodes)
			{
				if(child.NodeType == XmlNodeType.Element && child.Name == "value")
					childValuesList.Add(new Value(child, index++));
			}
			ChildValues = childValuesList.ToArray();

			Ordinal = ordinal;
		}

		//************************************************************************************************************************
		/// <summary>Removes a <see cref="Value"/> from <see cref="ChildValues"/>.</summary>
		///
		/// <param name="item"><see cref="Value"/> to be removed. Can be null.</param>
		//************************************************************************************************************************
		public void RemoveValue(Value item)
		{
			if (item == null) return;

			var list = new List<Value>(ChildValues);
			list.Remove(item);
			ChildValues = list.ToArray();
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

		#endregion Methods
	}
}
