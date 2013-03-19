using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using VVVV.Core.Logging;
using VVVV.Nodes.PatternTouch;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes
{
	public abstract class TransformProcessor : IPluginEvaluate
	{
		[Input("ID")]
		protected IDiffSpread<int> IdIn;

		[Input("Blobs")]
		protected ISpread<Blob> FBlobIn;

		protected readonly Spread<Blob> FPBlobs = new Spread<Blob>();

		[Input("Reset", IsBang = true)]
		protected ISpread<bool> ResetIn;

		[Import]
		protected ILogger Logger;

		protected readonly List<TransformState> TransformStates = new List<TransformState>();

		protected bool ReinitTransforms;

		protected void NeedsReinit()
		{
			ReinitTransforms = true;
			TransformStates.Clear();
		}

		protected virtual int CalculateMax()
		{
			return IdIn.SliceCount;
		}

		public void Evaluate(int spreadMax)
		{
			spreadMax = CalculateMax();

			TouchUtils.SetIsNew(FBlobIn, FPBlobs);

			if (IdIn.IsChanged || CheckForReinit())
			{
				NeedsReinit();
			}

			for (var i = 0; i < spreadMax; i++)
			{
				if (ReinitTransforms)
				{
					Reinit(i);
				}

				if (ResetIn[i])
				{
					Reset(i);
				}

				TransformStates[i].Update(FBlobIn);

				Debug.WriteLine(TransformStates[i].Phase);

				if (TransformStates[i].Phase == TransformPhase.Transforming)
				{
					ProcessTransformation(i);

				}

				TransformStates[i].UpdatePValues();
			}

			ReinitTransforms = false;
			FPBlobs.SliceCount = FBlobIn.SliceCount;

			FPBlobs.AssignFrom(FBlobIn);

			OutputData(spreadMax);
		}

		protected virtual bool CheckForReinit()
		{
			return false;
		}

		protected abstract void Reinit(int index);

		protected abstract void Reset(int index);

		protected virtual void ProcessTransformation(int index) { }

		protected abstract void OutputData(int spreadMax);
	}
}
