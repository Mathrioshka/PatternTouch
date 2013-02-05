using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	[PluginInfo(Name = "BlobState", Category = "PatternTouch", Version = "Split", Help = "Split blob state", Tags = "multitouch")]
	public class BlobStateSplitNode: IPluginEvaluate
	{
		[Input("Blob")] 
		private ISpread<Blob> FBlobIn;

		[Output("Position")] 
		private ISpread<Vector2D> FPositionOut;

		[Output("ID")]
		private ISpread<int> FIdOut;

		[Output("Hit ID")]
		private ISpread<int> FHitIdOut;

		[Output("Is New")]
		private ISpread<bool> FIsNewOut;
 
		public void Evaluate(int spreadMax)
		{
			FPositionOut.SliceCount = FIdOut.SliceCount = FHitIdOut.SliceCount = FIsNewOut.SliceCount = spreadMax;

			for (var i = 0; i < spreadMax; i++)
			{
				FPositionOut[i] = FBlobIn[i].Position;
				FIdOut[i] = FBlobIn[i].Id;
				FHitIdOut[i] = FBlobIn[i].HitId;
				FIsNewOut[i] = FBlobIn[i].IsNew;
			}
		}
	}
}
