using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.PatternTouch
{
	public class TouchNode : TouchProcessor
	{
		[Output("Touched")] 
		private ISpread<bool> FTouchedOutput;

		private Spread<Spread<Blob>> FBlobs; 
		
		public override void Evaluate(int spreadMax)
		{
			spreadMax = IdIn.Count();

			for (var i = 0; i < spreadMax; i++)
			{
			}
		}
	}
}
