using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	[PluginInfo(Name = "VPA", Category = "PatternTouch", Version = "Join", Help = "Join View, Projection and Aspect Ration transformations", Tags = "multitouch")]
	public class VPAJoinNode:IPluginEvaluate
	{
		[Input("View")] 
		public ISpread<Matrix4x4> ViewIn;

		[Input("Projection")] 
		public ISpread<Matrix4x4> ProjectionIn;

		[Input("Aspect Ration")]
		public ISpread<Matrix4x4> AspectRatioIn;

		[Output("Output")] 
		public ISpread<VPA> Output;
		
		public void Evaluate(int spreadMax)
		{
			Output.SliceCount = spreadMax;

			for (var i = 0; i < spreadMax; i++)
			{
				Output[i] = new VPA {View = ViewIn[i], Projection = ProjectionIn[i], AspectRatio = AspectRatioIn[i]};
			}
		}
	}

	[PluginInfo(Name = "VPA", Category = "PatternTouch", Version = "Split", Help = "Split View, Projection and Aspect Ration transformations", Tags = "multitouch")]
	public class VPASplitNode : IPluginEvaluate
	{
		[Input("Input")]
		public ISpread<VPA> VPAIn;

		[Output("View")]
		public ISpread<Matrix4x4> ViewOut;

		[Output("Projection")]
		public ISpread<Matrix4x4> ProjectionOut;

		[Output("Aspect Ration")]
		public ISpread<Matrix4x4> AspectRatioOut;

		public void Evaluate(int spreadMax)
		{
			ViewOut.SliceCount = ProjectionOut.SliceCount = AspectRatioOut.SliceCount = spreadMax;

			//TODO need a bug fix, if VPAIn is empty.

			for (var i = 0; i < spreadMax; i++)
			{
				var vpa = VPAIn[i];

				ViewOut[i] = vpa.View;
				ProjectionOut[i] = vpa.Projection;
				AspectRatioOut[i] = vpa.AspectRatio;
			}
		}
	}
}
