using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.PatternTouch
{
	public abstract class TouchProcessor : IPluginEvaluate
	{
		[Input("ID")]
		protected IDiffSpread<int> IdIn;

		[Input("Blobs")]
		protected ISpread<Blob> FBlobIn;

		protected readonly Spread<Blob> FPBlobs = new Spread<Blob>();

		public abstract void Evaluate(int SpreadMax);
	}
}
