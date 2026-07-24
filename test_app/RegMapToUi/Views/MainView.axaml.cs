using System.Linq;
using System.Text;
using Avalonia.Controls;
using BlockEditGen;
using BlockEditGen.Data;
using BlockEditGen.Parse;

namespace RegMapToUi.Views;

public partial class MainView : UserControl
{
	public MainView()
	{
		InitializeComponent();

		var panel = this.FindControl<Grid>("mainPanel");
		if (panel == null)
			return;

		var parsedBlock = new Block("../../../../example_xml/dummy.xml");
		parsedBlock.Initialize();

		var block = new CachedRegisterBlock<byte>(new RamRegisterBlock<byte>(parsedBlock.SizeInBytes, false));
		PanelFactory.PopulatePanel(block, parsedBlock, panel);

		SeedSampleData(parsedBlock, block);
	}

	private static void SeedSampleData(Block parsedBlock, CachedRegisterBlock<byte> block)
	{
		var values = parsedBlock.ChildValues
			.Concat(parsedBlock.ChildGroups.SelectMany(group => group.ChildValues))
			.ToDictionary(value => value.Name);

		WriteValue(block, values["Manufacturer Code (Attributes Register 1)"], Encoding.ASCII.GetBytes("K"));
		WriteValue(block, values["String 1"], Encoding.UTF8.GetBytes("Clock Gen"));
		WriteValue(block, values["String 2"], Encoding.Unicode.GetBytes("Rev A2"));

		// A uint array is shown with a UTF-32 column, so code points make a better sample than a ramp.
		WriteValue(block, values["UInt Array"], Encoding.UTF32.GetBytes("µΩ°C"));

		foreach (var value in values.Values.Where(value => value.Type == Value.TypeEnum.Array && value.Name != "UInt Array"))
		{
			var ramp = new byte[value.Length.Bytes];
			for (int i = 0; i < ramp.Length; i++)
				ramp[i] = (byte)(i * 7);
			WriteValue(block, value, ramp);
		}

		// Pushing makes the seeded values the baseline, so they aren't reported back as changes.
		_ = block.PushChangedValuesToRegisterBlockAsync();
	}

	/// <summary>
	///   Writes <paramref name="content"/> to the value's location, letting the block resolve the addressable word size
	///   and any bit-level misalignment.
	/// </summary>
	private static void WriteValue(CachedRegisterBlock<byte> block, Value value, byte[] content)
	{
		var buf = new byte[value.Length.Bytes + ((value.Length.Bits + 7) / 8)];
		content.CopyTo(buf, 0);
		block.WriteSection(value.Address, value.Length, buf);
	}
}
