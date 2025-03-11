// ******************************************************************************************************************************
// Filename:    Block.AutoGen.cs
// Owner:       Richard Dunkley
// Generated using XMLToDataClass version 1.1.0 with CSCodeGen.dll version 1.0.0.
// Copyright © Richard Dunkley 2024, Sequent Logic, LLC 2025
//********************************************************************************************************************************
using System.Globalization;
using System.Security;
using System.Xml;

namespace BlockEditGen.Parse
{
	//****************************************************************************************************************************
	/// <summary>Represents a block of byte addressable memory.</summary>
	//****************************************************************************************************************************
	public partial class Block
	{
		#region Enumerations

		//************************************************************************************************************************
		/// <summary>Enumerates the possible values of Access.</summary>
		//************************************************************************************************************************
		public enum AccessEnum
		{
			#region Names

			//********************************************************************************************************************
			/// <summary>Represents the 'RW' string.</summary>
			//********************************************************************************************************************
			ReadWrite,

			//********************************************************************************************************************
			/// <summary>Represents the 'R' string.</summary>
			//********************************************************************************************************************
			Read,

			//********************************************************************************************************************
			/// <summary>Represents the 'W' string.</summary>
			//********************************************************************************************************************
			Write,

			#endregion Names
		}

		//************************************************************************************************************************
		/// <summary>Represents the formats the SizeInBytes can be parsed from</summary>
		//************************************************************************************************************************
		public enum SizeInBytesIntegerFormat
		{
			#region Names

			//********************************************************************************************************************
			/// <summary>Hexadecimal number beginning with an '0x' (Ex: 0xFF).</summary>
			//********************************************************************************************************************
			HexType2,

			//********************************************************************************************************************
			/// <summary>Integer number (Ex: 1,234, 1234 or -1234).</summary>
			//********************************************************************************************************************
			Integer,

			#endregion Names
		}

		//************************************************************************************************************************
		/// <summary>Enumerates the various version types that can be parsed into a version</summary>
		//************************************************************************************************************************
		public enum VersionVersionType
		{
			#region Names

			//********************************************************************************************************************
			/// <summary>Only the major and minor part were provided.</summary>
			//********************************************************************************************************************
			MajorMinor,

			//********************************************************************************************************************
			/// <summary>The major, minor, and build were provided.</summary>
			//********************************************************************************************************************
			MajorMinorBuild,

			//********************************************************************************************************************
			/// <summary>The major, minor, build, and revision were provided.</summary>
			//********************************************************************************************************************
			MajorMinorBuildRevision,

			#endregion Names
		}

		#endregion Enumerations

		#region Fields

		//************************************************************************************************************************
		/// <summary>Default encoding of the XML file generated from this object.</summary>
		//************************************************************************************************************************
		private const string mDefaultXMLEncoding = "UTF-8";

		//************************************************************************************************************************
		/// <summary>Default version of the XML file generated from this object.</summary>
		//************************************************************************************************************************
		private const string mDefaultXMLVersion = "1.0";

		#endregion Fields

		#region Properties

		//************************************************************************************************************************
		/// <summary>Gets the read and write access of the block. Can be null.</summary>
		//************************************************************************************************************************
		public AccessEnum? Access { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets addressable size of the addressing in the block. Can be null.</summary>
		///
		/// <remarks>
		///   This determines whether the address specified in the configuration file are byte, 16-bit word, 32-bit word, or
		///   64-bit word addressable. Only 1, 2, 4, and 8 are allowed. Defaults to 1.
		/// </remarks>
		//************************************************************************************************************************
		public int? Addressable { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets or sets the child XML elements.</summary>
		//************************************************************************************************************************
		public Conv[] ChildConvs { get; private set; }

		//************************************************************************************************************************
		/// <summary>Gets or sets the child XML elements.</summary>
		//************************************************************************************************************************
		public Enum[] ChildEnums { get; private set; }

		//************************************************************************************************************************
		/// <summary>Gets or sets the child XML elements.</summary>
		//************************************************************************************************************************
		public Group[] ChildGroups { get; private set; }

		//************************************************************************************************************************
		/// <summary>Gets or sets the child XML elements.</summary>
		//************************************************************************************************************************
		public Value[] ChildValues { get; private set; }

		//************************************************************************************************************************
		/// <summary>Description of the memory block. Can be null. Can be empty.</summary>
		//************************************************************************************************************************
		public string Description { get; set; }

		//************************************************************************************************************************
		/// <summary>
		///   Gets or sets the id of the block. This ID should be unique across different block files so the block can be
		///   referenced from other configurations.
		/// </summary>
		//************************************************************************************************************************
		public string Id { get; set; }

		//************************************************************************************************************************
		/// <summary>Gets or sets the user displayable name of the block.</summary>
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
		/// <summary>Gets the size of the memory space in bytes.</summary>
		//************************************************************************************************************************
		public int SizeInBytes { get; set; }

		//************************************************************************************************************************
		/// <summary>Determines what format the SizeInBytes integral type should be converted to in the XML string</summary>
		//************************************************************************************************************************
		public SizeInBytesIntegerFormat SizeInBytesFormat { get; set; }

		//************************************************************************************************************************
		/// <summary>Version of the register block configuration.</summary>
		//************************************************************************************************************************
		public Version Version { get; set; }

		//************************************************************************************************************************
		/// <summary>Stores how the Version version is converted to an XML string</summary>
		//************************************************************************************************************************
		public VersionVersionType VersionParsedType { get; set; }

		//************************************************************************************************************************
		/// <summary>Encoding of the XML file this root node will be contained in.</summary>
		///
		/// <remarks>Defaults to 'UTF-8'</remarks>
		//************************************************************************************************************************
		public string XmlFileEncoding { get; set; }

		//************************************************************************************************************************
		/// <summary>Version of the XML file this root node will be contained in.</summary>
		///
		/// <remarks>Defaults to '1.0'</remarks>
		//************************************************************************************************************************
		public string XmlFileVersion { get; set; }

		#endregion Properties

		#region Methods

		//************************************************************************************************************************
		/// <overloads><summary>Instantiates a new <see cref="Block"/> object.</summary></overloads>
		///
		/// <summary>Instantiates a new <see cref="Block"/> object using the provided information.</summary>
		///
		/// <param name="access">'access' Custom Enumeration attribute contained in the XML element. Can be null.</param>
		/// <param name="addressable">
		///   'addressable' 32-bit signed integer attribute contained in the XML element. Can be null.
		/// </param>
		/// <param name="description">
		///   'description' String attribute contained in the XML element. Can be null. Can be empty.
		/// </param>
		/// <param name="id">'id' String attribute contained in the XML element.</param>
		/// <param name="name">'name' String attribute contained in the XML element.</param>
		/// <param name="sizeInBytes">'size_in_bytes' 32-bit signed integer attribute contained in the XML element.</param>
		/// <param name="version">'version' Version attribute contained in the XML element.</param>
		/// <param name="childConvs">Array of conv elements which are child elements of this node. Can be empty.</param>
		/// <param name="childEnums">Array of enum elements which are child elements of this node. Can be empty.</param>
		/// <param name="childGroups">Array of group elements which are child elements of this node. Can be empty.</param>
		/// <param name="childValues">Array of value elements which are child elements of this node. Can be empty.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="id"/>, or <paramref name="name"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="id"/>, <paramref name="name"/>, <paramref name="version"/>, <paramref name="childConvs"/>,
		///   <paramref name="childEnums"/>, <paramref name="childGroups"/>, or <paramref name="childValues"/> is a null
		///   reference.
		/// </exception>
		//************************************************************************************************************************
		public Block(AccessEnum? access, int? addressable, string description, string id, string name, int sizeInBytes,
			Version version, Conv[] childConvs, Enum[] childEnums, Group[] childGroups, Value[] childValues)
		{
			if(id == null)
				throw new ArgumentNullException("id");
			if(id.Length == 0)
				throw new ArgumentException("id is empty");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentException("name is empty");
			if(version == null)
				throw new ArgumentNullException("version");
			if(childConvs == null)
				throw new ArgumentNullException("childConvs");
			if(childEnums == null)
				throw new ArgumentNullException("childEnums");
			if(childGroups == null)
				throw new ArgumentNullException("childGroups");
			if(childValues == null)
				throw new ArgumentNullException("childValues");
			Access = access;
			Addressable = addressable;
			Description = description;
			Id = id;
			Name = name;
			SizeInBytes = sizeInBytes;
			Version = version;
			ChildConvs = childConvs;
			ChildEnums = childEnums;
			ChildGroups = childGroups;
			ChildValues = childValues;
			Ordinal = -1;

			// Compute the maximum index used on any child items.
			int maxIndex = 0;
			foreach(Conv item in ChildConvs)
			{
				if(item.Ordinal >= maxIndex)
					maxIndex = item.Ordinal + 1; // Set to first index after this index.
			}
			foreach(Enum item in ChildEnums)
			{
				if(item.Ordinal >= maxIndex)
					maxIndex = item.Ordinal + 1; // Set to first index after this index.
			}
			foreach(Group item in ChildGroups)
			{
				if(item.Ordinal >= maxIndex)
					maxIndex = item.Ordinal + 1; // Set to first index after this index.
			}
			foreach(Value item in ChildValues)
			{
				if(item.Ordinal >= maxIndex)
					maxIndex = item.Ordinal + 1; // Set to first index after this index.
			}

			// Assign ordinal for any child items that don't have it set (-1).
			foreach(Conv item in ChildConvs)
			{
				if(item.Ordinal == -1)
					item.Ordinal = maxIndex++;
			}
			foreach(Enum item in ChildEnums)
			{
				if(item.Ordinal == -1)
					item.Ordinal = maxIndex++;
			}
			foreach(Group item in ChildGroups)
			{
				if(item.Ordinal == -1)
					item.Ordinal = maxIndex++;
			}
			foreach(Value item in ChildValues)
			{
				if(item.Ordinal == -1)
					item.Ordinal = maxIndex++;
			}
			XmlFileVersion = mDefaultXMLVersion;
			XmlFileEncoding = mDefaultXMLEncoding;
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Block"/> empty object.</summary>
		///
		/// <param name="access">'access' Custom Enumeration attribute contained in the XML element. Can be null.</param>
		/// <param name="addressable">
		///   'addressable' 32-bit signed integer attribute contained in the XML element. Can be null.
		/// </param>
		/// <param name="description">
		///   'description' String attribute contained in the XML element. Can be null. Can be empty.
		/// </param>
		/// <param name="id">'id' String attribute contained in the XML element.</param>
		/// <param name="name">'name' String attribute contained in the XML element.</param>
		/// <param name="sizeInBytes">'size_in_bytes' 32-bit signed integer attribute contained in the XML element.</param>
		/// <param name="version">'version' Version attribute contained in the XML element.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="id"/>, or <paramref name="name"/> is an empty array.</exception>
		/// <exception cref="ArgumentNullException">
		///   <paramref name="id"/>, <paramref name="name"/>, or <paramref name="version"/> is a null reference.
		/// </exception>
		//************************************************************************************************************************
		public Block(AccessEnum? access, int? addressable, string description, string id, string name, int sizeInBytes,
			Version version)
		{
			if(id == null)
				throw new ArgumentNullException("id");
			if(id.Length == 0)
				throw new ArgumentException("id is empty");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentException("name is empty");
			if(version == null)
				throw new ArgumentNullException("version");
			Access = access;
			Addressable = addressable;
			Description = description;
			Id = id;
			Name = name;
			SizeInBytes = sizeInBytes;
			Version = version;
			ChildConvs = new Conv[0];
			ChildEnums = new Enum[0];
			ChildGroups = new Group[0];
			ChildValues = new Value[0];
			Ordinal = -1;
			XmlFileVersion = mDefaultXMLVersion;
			XmlFileEncoding = mDefaultXMLEncoding;
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Block"/> object from an <see cref="XmlNode"/> object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException">
		///   <paramref name="node"/> does not correspond to a block node or is not an 'Element' type node or <paramref
		///   name="ordinal"/> is negative.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public Block(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(ordinal < 0)
				throw new ArgumentException("the ordinal specified is negative.");
			if(node.NodeType != XmlNodeType.Element)
				throw new ArgumentException("node is not of type 'Element'.");

			ParseXmlNode(node, ordinal);
			XmlFileVersion = mDefaultXMLVersion;
			XmlFileEncoding = mDefaultXMLEncoding;
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Block"/> object from an XML file.</summary>
		///
		/// <param name="filePath">Path to the XML file containing the data to be imported.</param>
		///
		/// <exception cref="ArgumentException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item><paramref name="filePath"/> is invalid or an error occurred while accessing it.</item>
		///     <item><paramref name="filePath"/> is an empty array.</item>
		///   </list>
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="filePath"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">An error occurred while parsing the XML data.</exception>
		//************************************************************************************************************************
		public Block(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentException("filePath is empty");

			ImportFromXML(filePath);
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Block"/> object from an XML file.</summary>
		///
		/// <param name="stream">Stream containing the XML file data.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="stream"/> did not contain valid XML.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">An error occurred while parsing the XML data.</exception>
		//************************************************************************************************************************
		public Block(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			ImportFromXML(stream);
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Block"/> object from an XML file.</summary>
		///
		/// <param name="reader">TextReader object containing the XML file data.</param>
		///
		/// <exception cref="ArgumentException">
		///   A parsing error occurred while attempting to load the XML from <paramref name="reader"/>.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">An error occurred while parsing the XML data.</exception>
		//************************************************************************************************************************
		public Block(TextReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			ImportFromXML(reader);
		}

		//************************************************************************************************************************
		/// <summary>Instantiates a new <see cref="Block"/> object from an XML file.</summary>
		///
		/// <param name="reader">XmlReader object containing the XML file data.</param>
		///
		/// <exception cref="ArgumentException">
		///   A parsing error occurred while attempting to load the XML from <paramref name="reader"/>.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">An error occurred while parsing the XML data.</exception>
		//************************************************************************************************************************
		public Block(XmlReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			ImportFromXML(reader);
		}

		//************************************************************************************************************************
		/// <summary>Adds a <see cref="Conv"/> to <see cref="ChildConvs"/>.</summary>
		///
		/// <param name="item"><see cref="Conv"/> to be added. If null, then no changes will occur. Can be null.</param>
		//************************************************************************************************************************
		public void AddConv(Conv item)
		{
			if (item == null) return;

			// Compute the maximum index used on any child items.
			int maxIndex = 0;
			foreach(Conv child in ChildConvs)
			{
				if (child.Ordinal >= maxIndex)
					maxIndex = child.Ordinal + 1; // Set to first index after this index.
			}

			var list = new List<Conv>(ChildConvs);
			list.Add(item);
			item.Ordinal = maxIndex;
			ChildConvs = list.ToArray();
		}

		//************************************************************************************************************************
		/// <summary>Adds a <see cref="Enum"/> to <see cref="ChildEnums"/>.</summary>
		///
		/// <param name="item"><see cref="Enum"/> to be added. If null, then no changes will occur. Can be null.</param>
		//************************************************************************************************************************
		public void AddEnum(Enum item)
		{
			if (item == null) return;

			// Compute the maximum index used on any child items.
			int maxIndex = 0;
			foreach(Enum child in ChildEnums)
			{
				if (child.Ordinal >= maxIndex)
					maxIndex = child.Ordinal + 1; // Set to first index after this index.
			}

			var list = new List<Enum>(ChildEnums);
			list.Add(item);
			item.Ordinal = maxIndex;
			ChildEnums = list.ToArray();
		}

		//************************************************************************************************************************
		/// <summary>Adds a <see cref="Group"/> to <see cref="ChildGroups"/>.</summary>
		///
		/// <param name="item"><see cref="Group"/> to be added. If null, then no changes will occur. Can be null.</param>
		//************************************************************************************************************************
		public void AddGroup(Group item)
		{
			if (item == null) return;

			// Compute the maximum index used on any child items.
			int maxIndex = 0;
			foreach(Group child in ChildGroups)
			{
				if (child.Ordinal >= maxIndex)
					maxIndex = child.Ordinal + 1; // Set to first index after this index.
			}

			var list = new List<Group>(ChildGroups);
			list.Add(item);
			item.Ordinal = maxIndex;
			ChildGroups = list.ToArray();
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
			XmlElement returnElement = doc.CreateElement("block");

			string valueString;

			// access
			valueString = GetAccessString();
			if(valueString != null)
				returnElement.SetAttribute("access", valueString);

			// addressable
			valueString = GetAddressableString();
			if(valueString != null)
				returnElement.SetAttribute("addressable", valueString);

			// description
			valueString = GetDescriptionString();
			if(valueString != null)
				returnElement.SetAttribute("description", valueString);

			// id
			valueString = GetIdString();
			returnElement.SetAttribute("id", valueString);

			// name
			valueString = GetNameString();
			returnElement.SetAttribute("name", valueString);

			// size_in_bytes
			valueString = GetSizeInBytesString();
			returnElement.SetAttribute("size_in_bytes", valueString);

			// version
			valueString = GetVersionString();
			returnElement.SetAttribute("version", valueString);
			// Build up dictionary of indexes and corresponding items.
			Dictionary<int, object> lookup = new Dictionary<int, object>();

			foreach(Conv child in ChildConvs)
			{
				if(lookup.ContainsKey(child.Ordinal))
					throw new InvalidOperationException("An attempt was made to generate the XML element with two child elements"
						+ " with the same ordinal.Ordinals must be unique across all child objects.");
				lookup.Add(child.Ordinal, child);
			}

			foreach(Enum child in ChildEnums)
			{
				if(lookup.ContainsKey(child.Ordinal))
					throw new InvalidOperationException("An attempt was made to generate the XML element with two child elements"
						+ " with the same ordinal.Ordinals must be unique across all child objects.");
				lookup.Add(child.Ordinal, child);
			}

			foreach(Group child in ChildGroups)
			{
				if(lookup.ContainsKey(child.Ordinal))
					throw new InvalidOperationException("An attempt was made to generate the XML element with two child elements"
						+ " with the same ordinal.Ordinals must be unique across all child objects.");
				lookup.Add(child.Ordinal, child);
			}

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
				if(lookup[key] is Conv)
					returnElement.AppendChild(((Conv)lookup[key]).CreateElement(doc));
				if(lookup[key] is Enum)
					returnElement.AppendChild(((Enum)lookup[key]).CreateElement(doc));
				if(lookup[key] is Group)
					returnElement.AppendChild(((Group)lookup[key]).CreateElement(doc));
				if(lookup[key] is Value)
					returnElement.AppendChild(((Value)lookup[key]).CreateElement(doc));
			}
			return returnElement;
		}

		//************************************************************************************************************************
		/// <summary>Exports data to an XML file.</summary>
		///
		/// <param name="stream">Stream to write the XML to.</param>
		///
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is a null reference.</exception>
		//************************************************************************************************************************
		public void ExportToXML(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			XmlDocument doc = new XmlDocument();
			XmlDeclaration dec = doc.CreateXmlDeclaration(XmlFileVersion, XmlFileEncoding, null);
			doc.InsertBefore(dec, doc.DocumentElement);

			XmlElement root = CreateElement(doc);
			doc.AppendChild(root);
			doc.Save(stream);
		}

		//************************************************************************************************************************
		/// <summary>Exports data to an XML file.</summary>
		///
		/// <param name="filePath">Path to the XML file to be written to. If file exists all contents will be destroyed.</param>
		///
		/// <exception cref="ArgumentException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item><paramref name="filePath"/> is invalid or an error occurred while accessing it.</item>
		///     <item><paramref name="filePath"/> is an empty array.</item>
		///   </list>
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="filePath"/> is a null reference.</exception>
		//************************************************************************************************************************
		public void ExportToXML(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentException("filePath is empty");
			XmlDocument doc = new XmlDocument();
			XmlDeclaration dec = doc.CreateXmlDeclaration(XmlFileVersion, XmlFileEncoding, null);
			doc.InsertBefore(dec, doc.DocumentElement);

			XmlElement root = CreateElement(doc);
			doc.AppendChild(root);
			doc.Save(filePath);
		}

		//************************************************************************************************************************
		/// <summary>Exports data to an XML file.</summary>
		///
		/// <param name="writer">TextWriter object to write the XML to.</param>
		///
		/// <exception cref="ArgumentNullException"><paramref name="writer"/> is a null reference.</exception>
		//************************************************************************************************************************
		public void ExportToXML(TextWriter writer)
		{
			if(writer == null)
				throw new ArgumentNullException("writer");
			XmlDocument doc = new XmlDocument();
			XmlDeclaration dec = doc.CreateXmlDeclaration(XmlFileVersion, XmlFileEncoding, null);
			doc.InsertBefore(dec, doc.DocumentElement);

			XmlElement root = CreateElement(doc);
			doc.AppendChild(root);
			doc.Save(writer);
		}

		//************************************************************************************************************************
		/// <summary>Exports data to an XML file.</summary>
		///
		/// <param name="writer">XmlWriter object to write the XML to.</param>
		///
		/// <exception cref="ArgumentNullException"><paramref name="writer"/> is a null reference.</exception>
		//************************************************************************************************************************
		public void ExportToXML(XmlWriter writer)
		{
			if(writer == null)
				throw new ArgumentNullException("writer");
			XmlDocument doc = new XmlDocument();
			XmlDeclaration dec = doc.CreateXmlDeclaration(XmlFileVersion, XmlFileEncoding, null);
			doc.InsertBefore(dec, doc.DocumentElement);

			XmlElement root = CreateElement(doc);
			doc.AppendChild(root);
			doc.Save(writer);
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
				case AccessEnum.ReadWrite:
					return "RW";
				case AccessEnum.Read:
					return "R";
				case AccessEnum.Write:
					return "W";
				default:
					throw new NotImplementedException("The enumerated type was not recognized as a supported type.");
			}
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Addressable.</summary>
		///
		/// <returns>String representing the value. Can be null.</returns>
		//************************************************************************************************************************
		public string GetAddressableString()
		{
			if(!Addressable.HasValue)
				return null;

			return Addressable.Value.ToString();
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Description.</summary>
		///
		/// <returns>String representing the value. Can be null. Can be empty.</returns>
		//************************************************************************************************************************
		public string GetDescriptionString()
		{
			return Description;
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
		/// <summary>Gets a string representation of Name.</summary>
		///
		/// <returns>String representing the value.</returns>
		//************************************************************************************************************************
		public string GetNameString()
		{
			return Name;
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of SizeInBytes.</summary>
		///
		/// <returns>String representing the value.</returns>
		//************************************************************************************************************************
		public string GetSizeInBytesString()
		{

			if(SizeInBytesFormat == SizeInBytesIntegerFormat.HexType2)
				return string.Format("0x{0}", SizeInBytes.ToString("X"));
				else
				return SizeInBytes.ToString();
		}

		//************************************************************************************************************************
		/// <summary>Gets a string representation of Version.</summary>
		///
		/// <returns>String representing the value.</returns>
		//************************************************************************************************************************
		public string GetVersionString()
		{
			if(VersionParsedType == VersionVersionType.MajorMinorBuild)
				return Version.ToString(3);
			else if(VersionParsedType == VersionVersionType.MajorMinorBuildRevision)
				return Version.ToString(4);
			return Version.ToString(2);
		}

		//************************************************************************************************************************
		/// <summary>Imports data from an XML stream.</summary>
		///
		/// <param name="stream">Stream containing the XML file data.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="stream"/> did not contain valid XML.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="stream"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   The XML was valid, but an error occurred while extracting the data from it.
		/// </exception>
		//************************************************************************************************************************
		public void ImportFromXML(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(stream);
			}
			catch(XmlException e)
			{
				throw new ArgumentException(string.Format("Unable to parse the XML from the stream. Error message: {0}.",
					e.Message), nameof(stream), e);
			}

			// Pull the version and encoding
			XmlDeclaration dec = doc.FirstChild as XmlDeclaration;
			if(dec != null)
			{
				XmlFileVersion = dec.Version;
				XmlFileEncoding = dec.Encoding;
			}
			else
			{
				XmlFileVersion = mDefaultXMLVersion;
				XmlFileEncoding = mDefaultXMLEncoding;
			}

			XmlElement root = doc.DocumentElement;
			if(root.NodeType != XmlNodeType.Element)
				throw new InvalidDataException("The root node is not an element node.");
			if(string.Compare(root.Name, "block", false) != 0)
				throw new InvalidDataException(string.Format("The root element name is not the one expected (Actual: '{0}',"
					+ " Expected: 'block').", root.Name));

			ParseXmlNode(root, 0);
		}

		//************************************************************************************************************************
		/// <summary>Imports data from an XML file.</summary>
		///
		/// <param name="filePath">Path to the XML file containing the data to be imported.</param>
		///
		/// <exception cref="ArgumentException">
		///   <list type="bullet">
		///     <listheader>One of the following:</listheader>
		///     <item><paramref name="filePath"/> is invalid or an error occurred while accessing it.</item>
		///     <item><paramref name="filePath"/> is an empty array.</item>
		///   </list>
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="filePath"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   The XML was valid, but an error occurred while extracting the data from it.
		/// </exception>
		//************************************************************************************************************************
		public void ImportFromXML(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentException("filePath is empty");

			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(filePath);
			}
			catch(PathTooLongException e)
			{
				throw new ArgumentException(string.Format("The file path specified ({0}) is not valid ({1}).", filePath,
					e.Message), nameof(filePath), e);
			}
			catch(DirectoryNotFoundException e)
			{
				throw new ArgumentException(string.Format("The file path specified ({0}) is not valid ({1}).", filePath,
					e.Message), nameof(filePath), e);
			}
			catch(NotSupportedException e)
			{
				throw new ArgumentException(string.Format("The file path specified ({0}) is not valid ({1}).", filePath,
					e.Message), nameof(filePath), e);
			}
			catch(FileNotFoundException e)
			{
				throw new ArgumentException(string.Format("The file could not be located at the path specified ({0}).", filePath),
					nameof(filePath), e);
			}
			catch(IOException e)
			{
				throw new ArgumentException(string.Format("An I/O error occurred ({0}) while opening the file specified ({1}).",
					e.Message, filePath), nameof(filePath), e);
			}
			catch(UnauthorizedAccessException e)
			{
				throw new ArgumentException(string.Format("Unable to access the file path specified ({0}).", filePath),
					nameof(filePath), e);
			}
			catch(SecurityException e)
			{
				throw new ArgumentException(string.Format("The caller doesn't have the required permissions to access the file"
					+ " path specified ({0}).", filePath), nameof(filePath), e);
			}
			catch(XmlException e)
			{
				throw new ArgumentException(string.Format("Unable to parse the XML from the file specified ({0}). Error message:"
					+ " {1}.", filePath, e.Message), nameof(filePath), e);
			}

			// Pull the version and encoding
			XmlDeclaration dec = doc.FirstChild as XmlDeclaration;
			if(dec != null)
			{
				XmlFileVersion = dec.Version;
				XmlFileEncoding = dec.Encoding;
			}
			else
			{
				XmlFileVersion = mDefaultXMLVersion;
				XmlFileEncoding = mDefaultXMLEncoding;
			}

			XmlElement root = doc.DocumentElement;
			if(root.NodeType != XmlNodeType.Element)
				throw new InvalidDataException("The root node is not an element node.");
			if(string.Compare(root.Name, "block", false) != 0)
				throw new InvalidDataException(string.Format("The root element name is not the one expected (Actual: '{0}',"
					+ " Expected: 'block').", root.Name));

			ParseXmlNode(root, 0);
		}

		//************************************************************************************************************************
		/// <summary>Imports data from an XML text reader.</summary>
		///
		/// <param name="reader">TextReader object containing the XML file data.</param>
		///
		/// <exception cref="ArgumentException">
		///   A parsing error occurred while attempting to load the XML from <paramref name="reader"/>.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   The XML was valid, but an error occurred while extracting the data from it.
		/// </exception>
		//************************************************************************************************************************
		public void ImportFromXML(TextReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(reader);
			}
			catch(XmlException e)
			{
				throw new ArgumentException(string.Format("Unable to parse the XML from the reader. Error message: {0}.",
					e.Message), nameof(reader), e);
			}

			// Pull the version and encoding
			XmlDeclaration dec = doc.FirstChild as XmlDeclaration;
			if(dec != null)
			{
				XmlFileVersion = dec.Version;
				XmlFileEncoding = dec.Encoding;
			}
			else
			{
				XmlFileVersion = mDefaultXMLVersion;
				XmlFileEncoding = mDefaultXMLEncoding;
			}

			XmlElement root = doc.DocumentElement;
			if(root.NodeType != XmlNodeType.Element)
				throw new InvalidDataException("The root node is not an element node.");
			if(string.Compare(root.Name, "block", false) != 0)
				throw new InvalidDataException(string.Format("The root element name is not the one expected (Actual: '{0}',"
					+ " Expected: 'block').", root.Name));

			ParseXmlNode(root, 0);
		}

		//************************************************************************************************************************
		/// <summary>Imports data from an XML reader.</summary>
		///
		/// <param name="reader">XmlReader object containing the XML file data.</param>
		///
		/// <exception cref="ArgumentException">
		///   A parsing error occurred while attempting to load the XML from <paramref name="reader"/>.
		/// </exception>
		/// <exception cref="ArgumentNullException"><paramref name="reader"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   The XML was valid, but an error occurred while extracting the data from it.
		/// </exception>
		//************************************************************************************************************************
		public void ImportFromXML(XmlReader reader)
		{
			if(reader == null)
				throw new ArgumentNullException("reader");

			XmlDocument doc = new XmlDocument();
			try
			{
				doc.Load(reader);
			}
			catch(XmlException e)
			{
				throw new ArgumentException(string.Format("Unable to parse the XML from the reader. Error message: {0}.",
					e.Message), nameof(reader), e);
			}

			// Pull the version and encoding
			XmlDeclaration dec = doc.FirstChild as XmlDeclaration;
			if(dec != null)
			{
				XmlFileVersion = dec.Version;
				XmlFileEncoding = dec.Encoding;
			}
			else
			{
				XmlFileVersion = mDefaultXMLVersion;
				XmlFileEncoding = mDefaultXMLEncoding;
			}

			XmlElement root = doc.DocumentElement;
			if(root.NodeType != XmlNodeType.Element)
				throw new InvalidDataException("The root node is not an element node.");
			if(string.Compare(root.Name, "block", false) != 0)
				throw new InvalidDataException(string.Format("The root element name is not the one expected (Actual: '{0}',"
					+ " Expected: 'block').", root.Name));

			ParseXmlNode(root, 0);
		}

		//************************************************************************************************************************
		/// <summary>Parses an XML node and populates the data into this object.</summary>
		///
		/// <param name="node"><see cref="XmlNode"/> containing the data to extract.</param>
		/// <param name="ordinal">Index of the <see cref="XmlNode"/> in it's parent elements.</param>
		///
		/// <exception cref="ArgumentException"><paramref name="node"/> does not correspond to a block node.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="node"/> is a null reference.</exception>
		/// <exception cref="InvalidDataException">
		///   An error occurred while reading the data into the node, or one of it's child nodes.
		/// </exception>
		//************************************************************************************************************************
		public void ParseXmlNode(XmlNode node, int ordinal)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			if(string.Compare(node.Name, "block", false) != 0)
				throw new ArgumentException("node does not correspond to a block node.");

			XmlAttribute attrib;

			// access
			attrib = node.Attributes["access"];
			if(attrib == null)
				Access = null;
			else
				SetAccessFromString(attrib.Value);

			// addressable
			attrib = node.Attributes["addressable"];
			if(attrib == null)
				Addressable = null;
			else
				SetAddressableFromString(attrib.Value);

			// description
			attrib = node.Attributes["description"];
			if(attrib == null)
				Description = null;
			else
				SetDescriptionFromString(attrib.Value);

			// id
			attrib = node.Attributes["id"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (id) is not optional, but was not found in the XML"
					+ " element (block).");
			SetIdFromString(attrib.Value);

			// name
			attrib = node.Attributes["name"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (name) is not optional, but was not found in the XML"
					+ " element (block).");
			SetNameFromString(attrib.Value);

			// size_in_bytes
			attrib = node.Attributes["size_in_bytes"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (size_in_bytes) is not optional, but was not found in the"
					+ " XML element (block).");
			SetSizeInBytesFromString(attrib.Value);

			// version
			attrib = node.Attributes["version"];
			if(attrib == null)
				throw new InvalidDataException("An XML string Attribute (version) is not optional, but was not found in the XML"
					+ " element (block).");
			SetVersionFromString(attrib.Value);

			// Read the child objects.
			List<Conv> childConvsList = new List<Conv>();
			List<Enum> childEnumsList = new List<Enum>();
			List<Group> childGroupsList = new List<Group>();
			List<Value> childValuesList = new List<Value>();
			int index = 0;
			foreach(XmlNode child in node.ChildNodes)
			{
				if(child.NodeType == XmlNodeType.Element && child.Name == "conv")
					childConvsList.Add(new Conv(child, index++));
				if(child.NodeType == XmlNodeType.Element && child.Name == "enum")
					childEnumsList.Add(new Enum(child, index++));
				if(child.NodeType == XmlNodeType.Element && child.Name == "group")
					childGroupsList.Add(new Group(child, index++));
				if(child.NodeType == XmlNodeType.Element && child.Name == "value")
					childValuesList.Add(new Value(child, index++));
			}
			ChildConvs = childConvsList.ToArray();
			ChildEnums = childEnumsList.ToArray();
			ChildGroups = childGroupsList.ToArray();
			ChildValues = childValuesList.ToArray();

			Ordinal = ordinal;
		}

		//************************************************************************************************************************
		/// <summary>Removes a <see cref="Conv"/> from <see cref="ChildConvs"/>.</summary>
		///
		/// <param name="item"><see cref="Conv"/> to be removed. Can be null.</param>
		//************************************************************************************************************************
		public void RemoveConv(Conv item)
		{
			if (item == null) return;

			var list = new List<Conv>(ChildConvs);
			list.Remove(item);
			ChildConvs = list.ToArray();
		}

		//************************************************************************************************************************
		/// <summary>Removes a <see cref="Enum"/> from <see cref="ChildEnums"/>.</summary>
		///
		/// <param name="item"><see cref="Enum"/> to be removed. Can be null.</param>
		//************************************************************************************************************************
		public void RemoveEnum(Enum item)
		{
			if (item == null) return;

			var list = new List<Enum>(ChildEnums);
			list.Remove(item);
			ChildEnums = list.ToArray();
		}

		//************************************************************************************************************************
		/// <summary>Removes a <see cref="Group"/> from <see cref="ChildGroups"/>.</summary>
		///
		/// <param name="item"><see cref="Group"/> to be removed. Can be null.</param>
		//************************************************************************************************************************
		public void RemoveGroup(Group item)
		{
			if (item == null) return;

			var list = new List<Group>(ChildGroups);
			list.Remove(item);
			ChildGroups = list.ToArray();
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
			if(string.Compare(value, "RW", false) == 0)
			{
				Access = AccessEnum.ReadWrite;
				return;
			}
			if(string.Compare(value, "R", false) == 0)
			{
				Access = AccessEnum.Read;
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
		/// <summary>Parses a string value and stores the data in Addressable.</summary>
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
		public void SetAddressableFromString(string value)
		{
			if(value == null)
			{
				Addressable = null;
				return;
			}
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'addressable' is an empty string.");
			int returnValue = 0;
			bool parsed = false;
			try
			{

				// Attempt to parse the number as just an integer.
				returnValue = int.Parse(value, NumberStyles.Integer | NumberStyles.AllowThousands);
				parsed = true;
			}
			catch(FormatException e)
			{
				throw new InvalidDataException(string.Format("The int value specified ({0}) is not in a valid int string format:"
					+ " {1}.", value, e.Message), e);
			}
			catch(OverflowException e)
			{
				throw new InvalidDataException(string.Format("The int value specified ({0}) was larger or smaller than a int"
					+ " value (Min: {1}, Max: {2}).", value, int.MinValue.ToString(), int.MaxValue.ToString()), e);
			}

			if(!parsed)
				throw new InvalidDataException(string.Format("The int value specified ({0}) is not in a valid int string format.",
					value));

			// Verify that the int value has not exceeded the maximum size.
			if(returnValue > 8)
				throw new InvalidDataException(string.Format("The int value specified ({0}) was larger than the maximum value"
					+ " allowed for this type (8).", value));

			// Verify that the int value is not lower than the minimum size.
			if(returnValue < 1)
				throw new InvalidDataException(string.Format("The int value specified ({0}) was less than the minimum value"
					+ " allowed for this type (1).", value));

			Addressable = returnValue;
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Description.</summary>
		///
		/// <param name="value">String representation of the value.</param>
		///
		/// <exception cref="InvalidDataException">The string value could not be parsed.</exception>
		//************************************************************************************************************************
		public void SetDescriptionFromString(string value)
		{
			if(value == null)
			{
				Description = null;
				return;
			}
			Description = value;
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
		/// <summary>Parses a string value and stores the data in SizeInBytes.</summary>
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
		public void SetSizeInBytesFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'size_in_bytes' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'size_in_bytes' is an empty string.");
			int returnValue = 0;
			bool parsed = false;
			try
			{

				if(value.Length > 2 && value[0] == '0' && char.ToLower(value[1]) == 'x')
				{
					// Number is specified as a hexadecimal type 2 number (0xFF).
					returnValue = int.Parse(value.Substring(2), NumberStyles.AllowHexSpecifier);
					SizeInBytesFormat = SizeInBytesIntegerFormat.HexType2;
					parsed = true;
				}

				else
				{
					// Attempt to parse the number as just an integer.
					returnValue = int.Parse(value, NumberStyles.Integer | NumberStyles.AllowThousands);
					SizeInBytesFormat = SizeInBytesIntegerFormat.Integer;
					parsed = true;
				}
			}
			catch(FormatException e)
			{
				throw new InvalidDataException(string.Format("The int value specified ({0}) is not in a valid int string format:"
					+ " {1}.", value, e.Message), e);
			}
			catch(OverflowException e)
			{
				throw new InvalidDataException(string.Format("The int value specified ({0}) was larger or smaller than a int"
					+ " value (Min: {1}, Max: {2}).", value, int.MinValue.ToString(), int.MaxValue.ToString()), e);
			}

			if(!parsed)
				throw new InvalidDataException(string.Format("The int value specified ({0}) is not in a valid int string format.",
					value));

			// Verify that the int value is not lower than the minimum size.
			if(returnValue < 1)
				throw new InvalidDataException(string.Format("The int value specified ({0}) was less than the minimum value"
					+ " allowed for this type (1).", value));

			SizeInBytes = returnValue;
		}

		//************************************************************************************************************************
		/// <summary>Parses a string value and stores the data in Version.</summary>
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
		public void SetVersionFromString(string value)
		{
			if(value == null)
				throw new InvalidDataException("The string value for 'version' is a null reference.");
			if(value.Length == 0)
				throw new InvalidDataException("The string value for 'version' is an empty string.");
			string[] splits = value.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
			if(splits.Length < 2)
				throw new InvalidDataException(string.Format("The Version value ({0}) for 'version' does not contain at least two"
					+ " components separated by a period (Ex: <major>.<minor>)", value));
			if(splits.Length > 4)
				throw new InvalidDataException(string.Format("The Version value ({0}) for 'version' has more than 4 components"
					+ " separated by a period. A version is limited to four at most (Ex: <major>.<minor>.<build>.<revision>)",
					value));

			if(splits.Length == 3)
				VersionParsedType = VersionVersionType.MajorMinorBuild;
			else if(splits.Length == 4)
				VersionParsedType = VersionVersionType.MajorMinorBuildRevision;
			else
			VersionParsedType = VersionVersionType.MajorMinor;

			try
			{
				int major = int.Parse(splits[0]);
				int minor = int.Parse(splits[1]);
				if(splits.Length == 2)
				{
					Version = new Version(major, minor);
					return;
				}
				int build = int.Parse(splits[2]);
				if(splits.Length == 3)
				{
					Version = new Version(major, minor, build);
					return;
				}
				Version = new Version(major, minor, build, int.Parse(splits[3]));
				return;
			}
			catch(Exception e)
			{
				throw new InvalidDataException(string.Format("The Version value ({0}) for 'version' is not valid. See Inner"
					+ " Exception.", value), e);
			}
		}

		#endregion Methods
	}
}
