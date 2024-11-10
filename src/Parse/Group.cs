using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockEditGen.Parse
{
	public partial class Group
	{
		public void Initialize(Block parent)
		{
			if (parent == null) throw new ArgumentNullException(nameof(parent));

			foreach (var value in ChildValues)
				value.Initialize(parent);
		}
	}
}
