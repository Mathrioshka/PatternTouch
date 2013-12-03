using System.Collections.Generic;
using System.ComponentModel.Composition;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.PatternTouch
{
	public abstract class TransformProcessor : IPluginEvaluate
	{
		[Input("ID")]
		protected IDiffSpread<int> IdIn;

		[Input("Blobs")]
		protected ISpread<Blob> BlobIn;

		protected readonly Spread<Blob> PBlobs = new Spread<Blob>();

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
			var sMax = CalculateMax();

			TouchUtils.SetIsNew(BlobIn, PBlobs);

			if (IdIn.IsChanged || CheckForReinit())
			{
				NeedsReinit();
			}

			for (var i = 0; i < sMax; i++)
			{
				if (ReinitTransforms)
				{
					Reinit(i);
				}

				if (ResetIn[i])
				{
					Reset(i);
				}

				TransformStates[i].Update(BlobIn);

				if (TransformStates[i].Phase == TransformPhase.Transforming)
				{
					ProcessTransformation(i);

				}

				TransformStates[i].UpdatePValues();
			}

			ReinitTransforms = false;
			PBlobs.SliceCount = BlobIn.SliceCount;

			PBlobs.AssignFrom(BlobIn);

			OutputData(sMax);
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
