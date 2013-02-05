using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.PatternTouch
{
	[PluginInfo(Name = "SetHitId", Category = "PatternTouch", Help = "Set Hit ID to Blob", Tags = "multitouch")]
	public class SetHitIdNode : IPluginEvaluate
	{
		[Input("Blob")]
		private ISpread<Blob> FBlobIn;

		[Input("Hit ID")]
		private ISpread<int> FHitIdIn;

		[Output("Blob")]
		private ISpread<Blob> FBlobOut;
		public void Evaluate(int spreadMax)
		{
			FBlobOut.SliceCount = spreadMax;

			for (var i = 0; i < spreadMax; i++)
			{
				var blob = FBlobIn[i];
				blob.HitId = FHitIdIn[i];
				FBlobOut[i] = blob;
			}
		}
	}
}
