using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	[PluginInfo(Name = "VPA", Category = "PatternTouch", Version = "Join", Help = "Join View, Projection and Aspect Ration transformations", Tags = "multitouch")]
	public class VPAJoinNode:IPluginEvaluate
	{
		[Input("View")] 
		private ISpread<Matrix4x4> FViewIn;

		[Input("Projection")] 
		private ISpread<Matrix4x4> FProjectionIn;

		[Input("Aspect Ration")]
		private ISpread<Matrix4x4> FAspectRatioIn;

		[Output("Output")] private ISpread<VPA> FOutput;
		
		public void Evaluate(int spreadMax)
		{
			FOutput.SliceCount = spreadMax;

			for (var i = 0; i < spreadMax; i++)
			{
				FOutput[i] = new VPA {View = FViewIn[i], Projection = FProjectionIn[i], AspectRatio = FAspectRatioIn[i]};
			}
		}
	}

	[PluginInfo(Name = "VPA", Category = "PatternTouch", Version = "Split", Help = "Split View, Projection and Aspect Ration transformations", Tags = "multitouch")]
	public class VPASplitNode : IPluginEvaluate
	{
		[Input("Input")]
		private ISpread<VPA> FVPAIn;

		[Output("View")]
		private ISpread<Matrix4x4> FViewOut;

		[Output("Projection")]
		private ISpread<Matrix4x4> FProjectionOut;

		[Output("Aspect Ration")]
		private ISpread<Matrix4x4> FAspectRatioOut;

		public void Evaluate(int spreadMax)
		{
			FViewOut.SliceCount = FProjectionOut.SliceCount = FAspectRatioOut.SliceCount = spreadMax;

			for (var i = 0; i < spreadMax; i++)
			{
				var vpa = FVPAIn[i];

				FViewOut[i] = vpa.View;
				FProjectionOut[i] = vpa.Projection;
				FAspectRatioOut[i] = vpa.AspectRatio;
			}
		}
	}
}
