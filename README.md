# BlockEditGen
Takes in an XML file at runtime to dynamically generate an Avalonia panel to modify values in configuration register blocks.

## XML Configuration
See example_xml folder for example XML file. The following sections outline the elements used in the XML file and possible attributes.

### Size and Offset Format
Sizes and offsets are represented as bytes and bits separated by a period (i.e. <bytes>.<bits>). Bytes can be represented as a hexidecimal or numeric integer and is required. Bits is optional and must be represented as a numeric value. Below are some examples of the format:

* 0.25 - 25-bits
* 0x00.13 - 13-bits
* 0x28.3 - 40 bytes and 3 bits.
* 15.31 - 15 bytes and 31 bits.

### Accessibility
The block and each value in the block has an optional 'access' attribute that allows the registers or values to be read-only or read/write. This determines what graphical control to use. On read-only the graphical control does not allow user modification. Read/write values will allow user modification.

The following options are valid for access:
* R - Read-only
* RW - Read/Write

### Elements

**block**

Main element in the XML file. This is a virtual representation of the register block the UI panel will represent.

Attributes:
* version - Version of the configuration file.
* size_in_bytes - Size of the register block in bytes.
* access - Determines if the register block is read-only or read/write. R for read-only, and RW for read/write.
* id - Unique ID of the panel. This allows it to be identified when multiple configuration XML files are used.
* name - Name of the panel.
* description - Description of the panel.

Child Elements:
* enum
* conv
* group
* value

**enum**

Defines an enumeration of items that represent different values of bits in a register. Enumerations can be referenced multiple times.

Attributes:
* width - Width of the values in the enumeration.
* id - Unique identifier of the enumeration. Referenced by 'value' elements to show the values in a drop down list instead of numeric entry.

Child Elements:
* item

**item**

Defines an item in the enumeration.

Attributes:
* value - Value the item represents.
* name - Name of the item. This is what is displayed to the user.
* tooltip - Tooltip to display when mousing over the item in the list.

Child Elements:
None

**conv**

Defines a conversion to use when reading/writing the register value. This allows linear conversions of register values to a user representable value.

Attributes:
* gain - Gain to be multiplied to the value in the register.
* offset - Offset to be added to the value in the register after the gain is applied.
* id - Unique identifier used to identify the conversion. Referenced by 'value' elements to apply the conversion.

Child Elements:
None

**group**

Allows for grouping of values. The grouping will contain a name to identify the group.

Attributes:
* name - Name of the group to be displayed to the user.

Child Elements:
value

**value**

Represents a value in a section of the register space. Values are what the user can modify to drive changes to register bits.

Attributes:
* addr - Offset of the value in the register space.
* size - Size of the value in the register space.
* access - Determines the accessibility of the value (described above).
* type - Type of data the value represents. This determines what UI control represents the value. The following types are supported:
  * bool - Boolean value. Must be a single bit. When this type is used, the 'subtype' attribute must have two names separated by a comma that describe the value when it is a zero or one (respectively).
  * string - Represents a value of bytes that represent a string. Size must be a multiple of the encoding type specified. subtype attribute will reflect the encoding type ('ASCII', 'UTF8', or 'Unicode').
  * uint# - Represents an unsigned integer of # bits (supported values are uint8, uint16, uint32, or uint64). Size must be equal to the number of bits (or smaller). subtype can have one or two components (separated by a comma). (Example: 'hex,be' or 'num'.) The first is a value of how it should be displayed ('hex', 'bin', or 'num'). 'hex' displays the numeric value in hexadecimal format. 'bin' displays the numeric value in binary format. 'num' displays the numeric value in a standard number format. The second component of subtype is optional and determines the endian of the value. 'be' interprets the value as big endian. 'le' inteprets the value as little endian.
  * int# - Represents an integer of # bits (supported values are int8, int16, int32, or int64). Size must be equal to the number of bits (or smaller). Signed integers are always represented in standard number format. subtype contains the endianness of the value. 'le' for little endian, 'be' for big endian.
  * enum - Value should be represented as a list of enumerated items. The subtype is the ID of the enumeration to use.
  * ip - Represents an IP address. Size must equal to 32 bits for IPV4 and 128 bits for IPV6. The subtype will be '4' for IPV4 or '6' for IPV6.
  * mac - Represents a MAC address. Size must be equal to 48 bits. The subtype is not used.
  * double - Double precision floating point number. Size can be 64-bits or less. The subtype contains the integer number of decimal places to display.
  * float - Single precision floating point number. Size can be 32-bits or less. The subtype contains the integer number of decimal places to display.
* subtype - Additional format options for the give type. Supported subtypes are described under their corresponding type.
* name - Name of the value.
* tooltip - Tooltip to be displayed when the user mouses over the generated UI control.
* units - Optional attribute. Will display the text after the value to represent the units of the value.
* conv - Conversion to use to convert the register value prior to displaying it.
