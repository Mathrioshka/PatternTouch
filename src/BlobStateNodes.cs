using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	[PluginInfo(Name = "BlobState", Category = "PatternTouch", Version = "Join", Help = "Pack blob state", Tags = "multitouch")]
	public class BlobStateJoinNode : IPluginEvaluate
	{
		[Input("Position" )]
		public ISpread<Vector2D> PosIn;
		
		[Input("ID")]
		public ISpread<int> IdIn;

		[Input("Hit ID", DefaultValue = -1)]
		public ISpread<int> HitIdIn;

		[Output("Blob")]
		public ISpread<Blob> BlobOut;

		public void Evaluate(int spreadMax)
		{
			BlobOut.SliceCount =  spreadMax;

			for (var i = 0; i < spreadMax; i++)
			{	
				BlobOut[i] = new Blob {Id = IdIn[i], Position = PosIn[i], HitId = HitIdIn[i], IsNew = false};
			}
		}
	}

	[PluginInfo(Name = "BlobState", Category = "PatternTouch", Version = "Split", Help = "Split blob state", Tags = "multitouch")]
	public class BlobStateSplitNode : IPluginEvaluate
	{
		[Input("Blob")]
		public ISpread<Blob> BlobIn;

		[Output("Position")]
		public ISpread<Vector2D> PositionOut;

		[Output("ID")]
		public ISpread<int> IdOut;

		[Output("Hit ID")]
		public ISpread<int> HitIdOut;

		[Output("Is New")]
		public ISpread<bool> IsNewOut;

		public void Evaluate(int spreadMax)
		{
			PositionOut.SliceCount = IdOut.SliceCount = HitIdOut.SliceCount = IsNewOut.SliceCount = spreadMax;

			for (var i = 0; i < spreadMax; i++)
			{
				PositionOut[i] = BlobIn[i].Position;
				IdOut[i] = BlobIn[i].Id;
				HitIdOut[i] = BlobIn[i].HitId;
				IsNewOut[i] = BlobIn[i].IsNew;
			}
		}
	}
}
