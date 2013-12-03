using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.PatternTouch
{
	[PluginInfo(Name = "SetHitId", Category = "PatternTouch", Help = "Set Hit ID to Blob", Tags = "multitouch")]
	public class SetHitIdNode : IPluginEvaluate
	{
		[Input("Blob")]
		public ISpread<Blob> BlobIn;

		[Input("Hit ID")]
		public ISpread<int> HitIdIn;

		[Output("Blob")]
		public ISpread<Blob> BlobOut;
		
		public void Evaluate(int spreadMax)
		{
			BlobOut.SliceCount = spreadMax;

			for (var i = 0; i < spreadMax; i++)
			{
				var blob = BlobIn[i];
				blob.HitId = HitIdIn[i];
				BlobOut[i] = blob;
			}
		}
	}
}
