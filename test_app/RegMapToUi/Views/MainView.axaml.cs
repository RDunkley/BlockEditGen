using Avalonia.Controls;
using BlockEditGen;
using BlockEditGen.Data;

namespace RegMapToUi.Views;

public partial class MainView : UserControl
{
	public MainView()
	{
		InitializeComponent();

		var panel = this.FindControl<Panel>("mainPanel");

		string path = "../../../../example_xml/dummy.xml";

		if (panel != null)
			PanelFactory.PopulatePanel(new CachedRegisterBlock<byte>(new RamRegisterBlock<byte>(4096, false)), path, panel);
	}
}
