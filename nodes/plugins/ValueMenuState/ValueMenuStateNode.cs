#region usings
using System;
using System.Linq;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	[PluginInfo(Name = "MenuState", Category = "Value", Help = "Basic template with one value in/out", Tags = "")]
	public class ValueMenuStateNode : IPluginEvaluate
	{
		[Input("Menu Count", DefaultValue = 1.0, IsSingle = true)]
		public IDiffSpread<int> FMenuCountIn;
		
		[Input("Reset Value", IsSingle = true)]
		public ISpread<int> FResetValue;
		
		[Input("Menu Slice")]
		public IDiffSpread<int> FMenuSliceIn;
		
		[Input("Menu Entry Slice")]
		public IDiffSpread<int> FMenuEntrySliceIn;
		
		[Input("Allow Untoggle", IsSingle = true, DefaultValue = 0)]
		public IDiffSpread<bool> FAllowUntoggleIn;
		
		[Input("Untogle To Previous", IsSingle = true, DefaultValue = 0)]
		public IDiffSpread<bool> FUntoggleToPrevious;
		
		[Input("Reset", IsBang = true)]
		public IDiffSpread<bool> FResetIn;

		[Output("Selected Menu Entry")]
		public ISpread<int> FSelectedMenuEntryOut;
		
		[Import()]
		ILogger FLogger;

		public void Evaluate(int SpreadMax)
		{
			var count = FSelectedMenuEntryOut.SliceCount = FMenuCountIn.First();
			
			if(FMenuCountIn.IsChanged)
			{
				for (var i = 0; i < count; i++)
				{
					FSelectedMenuEntryOut[i] = FAllowUntoggleIn[0] ? -1 : 0;
				}
			}

			if(FMenuEntrySliceIn.IsChanged || FMenuSliceIn.IsChanged)
			{
				if(FMenuEntrySliceIn.SliceCount == 0 || FMenuSliceIn.SliceCount == 0) return;
				
				for (var i = 0; i < FMenuSliceIn.SliceCount; i++)
				{
					if( FSelectedMenuEntryOut[FMenuSliceIn[i]] == FMenuEntrySliceIn[i] && FAllowUntoggleIn[0])
					{
						var currentEntry = FSelectedMenuEntryOut[FMenuSliceIn[i]];
						FSelectedMenuEntryOut[FMenuSliceIn[i]] = FUntoggleToPrevious[0] ? Math.Abs(currentEntry - 1) : -1;
					}
					else
					{
						FSelectedMenuEntryOut[FMenuSliceIn[i]] = FMenuEntrySliceIn[i];
					}
				}
			}
			
			if(FResetIn.IsChanged)
			{
				for(var i = 0; i < count; i++)
				{
					var untoggle = FUntoggleToPrevious[0] ? FResetValue[0] : -1;
					if(FResetIn[i]) FSelectedMenuEntryOut[i] = FAllowUntoggleIn[0] ? untoggle : FResetValue[0];
				}
			}
		}
	}
}
