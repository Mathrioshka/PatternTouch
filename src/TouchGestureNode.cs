using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	[PluginInfo(Name = "Touch", Category = "PatternTouch", Version = "Gesture", Help = "Detects when you touching object", Tags = "multitouch")]
	public class TouchGestureNode : TransformProcessor
	{
		[Input("Number Of Fingers", DefaultValue = 1, Visibility = PinVisibility.OnlyInspector)] 
		private ISpread<int> FMinFingersIn;
		
		[Output("Touched")] 
		private ISpread<bool> FTouchedOutput;

		protected override void Reinit(int index)
		{
			TransformStates.Add(new TransformState(IdIn[index], new Vector2D()));
		}

		protected override void Reset(int index)
		{
			TransformStates[index].Reset(IdIn[index], 0);
		}

		protected override void OutputData(int spreadMax)
		{
			FTouchedOutput.SliceCount = spreadMax;

			for (var i = 0; i < spreadMax; i++)
			{
				FTouchedOutput[i] = (TransformStates[i].Phase == TransformPhase.Transforming) && TransformStates[i].Blobs.SliceCount >= FMinFingersIn[i];
			}
		}
	}
}
