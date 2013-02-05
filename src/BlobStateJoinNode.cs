#region usings

using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

#endregion usings

namespace VVVV.Nodes.PatternTouch
{
	#region PluginInfo
	[PluginInfo(Name = "BlobState", Category = "PatternTouch", Version = "Join", Help = "Pack blob state", Tags = "multitouch")]
	#endregion PluginInfo
	public class BlobStateJoinNode : IPluginEvaluate
	{
		[Input("Position" )]
		ISpread<Vector2D> FPosIn;
		
		[Input("ID")]
		ISpread<int> FIdIn;

		[Input("Hit ID", DefaultValue = -1)]
		ISpread<int> FHitIdIn;

		[Output("Blob")]
		ISpread<Blob> FBlobOut;

		private Spread<int> FPId = new Spread<int>(); 

		public void Evaluate(int spreadMax)
		{
			FBlobOut.SliceCount = FPId.SliceCount = spreadMax;

			for (var i = 0; i < spreadMax; i++)
			{
				var isNew = FPId[i] != FIdIn[i];
				FBlobOut[i] = new Blob {Id = FIdIn[i], Position = FPosIn[i], HitId = FHitIdIn[i], IsNew = isNew};
			}

			FPId.AssignFrom(FIdIn);
		}
	}
}
