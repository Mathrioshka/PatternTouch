using System.Collections.Generic;
using System.ComponentModel.Composition;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	public abstract class Transform : IPluginEvaluate
	{
		[Input("ID")]
		protected IDiffSpread<int> IdIn;

		[Input("Blobs")]
		private ISpread<Blob> FBlobIn;

		[Input("Reset", IsBang = true)]
		protected ISpread<bool> ResetIn;

		[Import] 
		protected ILogger Logger;

		private readonly Spread<Blob> FPBlobs = new Spread<Blob>();
		protected readonly List<TransformState> TransformStates = new List<TransformState>();
		
		private bool FReinitTransforms;

		public void Evaluate(int spreadMax)
		{
			spreadMax = IdIn.SliceCount;

			TouchUtils.SetIsNew(FBlobIn, FPBlobs);

			if (IdIn.IsChanged)
			{
				FReinitTransforms = true;
				TransformStates.Clear();
			}

			for (var i = 0; i < spreadMax; i++)
			{
				if (FReinitTransforms)
				{
					Reinit(i);
				}

				if (ResetIn[i])
				{
					Reset(i);
				}

				TransformStates[i].Update(FBlobIn);

				if (TransformStates[i].Phase == TransformPhase.Transforming)
				{
					TransformStates[i].TransformationValue += GetTransformation(TransformStates[i]);
				}

				TransformStates[i].UpdatePValues();
			}

			FReinitTransforms = false;
			FPBlobs.SliceCount = FBlobIn.SliceCount;

			FPBlobs.AssignFrom(FBlobIn);

			OutputData(spreadMax);
		}

		
		protected abstract void Reinit(int index);

		protected abstract void Reset(int index);

		protected abstract Vector2D GetTransformation(TransformState transformState);

		protected abstract void OutputData(int spreadMax);


	}

	[PluginInfo(Name = "Rotate", Category = "PatternTouch", Version = "2D", Help = "Rotate object", Tags = "multitouch")]
	public class RotateTransformNode : Transform
	{
		[Input("Initial Value")] 
		private ISpread<double> FInitialValueIn;

		[Output("Rotation")] 
		private ISpread<double> FRotationOut; 

		protected override void Reinit(int index)
		{
			TransformStates.Add(new TransformState(IdIn[index], new Vector2D(FInitialValueIn[index])));
		}

		protected override void Reset(int index)
		{
			TransformStates[index].Reset(IdIn[index], FInitialValueIn[index]);
		}

		protected override Vector2D GetTransformation(TransformState transformState)
		{
			var value = TouchUtils.CalculateTransform(transformState, TransformType.Rotate);
			transformState.PDelta = value;
			return value;
		}

		protected override void OutputData(int spreadMax)
		{
			FRotationOut.SliceCount = spreadMax;
			for (var i = 0; i < spreadMax; i++)
			{
				FRotationOut[i] = TransformStates[i].TransformationValue.x;
			}
		}
	}

	[PluginInfo(Name = "Translate", Category = "PatternTouch", Version = "2D", Help = "Translate object", Tags = "multitouch")]
	public class TranslateTransformNode : Transform
	{
		[Input("Initial Value")]
		private ISpread<Vector2D> FInitialValueIn;

		[Output("Translate")]
		private ISpread<Vector2D> FTranslateOut;

		protected override void Reinit(int index)
		{
			TransformStates.Add(new TransformState(IdIn[index], FInitialValueIn[index]));
		}

		protected override void Reset(int index)
		{
			TransformStates[index].Reset(IdIn[index], FInitialValueIn[index]);
		}

		protected override Vector2D GetTransformation(TransformState transformState)
		{
			return TouchUtils.CalculateTransform(transformState, TransformType.Translate);
		}

		protected override void OutputData(int spreadMax)
		{
			FTranslateOut.SliceCount = spreadMax;
			for (var i = 0; i < spreadMax; i++)
			{
				FTranslateOut[i] = TransformStates[i].TransformationValue;
			}
		}
	}

	[PluginInfo(Name = "Scale", Category = "PatternTouch", Version = "2D", Help = "Scale object", Tags = "multitouch")]
	public class ScaleTransformNode : Transform
	{
		[Input("Initial Value")]
		private ISpread<Vector2D> FInitialValueIn;

		[Output("Scale")]
		private ISpread<double> FScaleOut;

		protected override void Reinit(int index)
		{
			TransformStates.Add(new TransformState(IdIn[index], FInitialValueIn[index]));
		}

		protected override void Reset(int index)
		{
			TransformStates[index].Reset(IdIn[index], FInitialValueIn[index]);
		}

		protected override Vector2D GetTransformation(TransformState transformState)
		{
			return TouchUtils.CalculateTransform(transformState, TransformType.Scale);
		}

		protected override void OutputData(int spreadMax)
		{
			FScaleOut.SliceCount = spreadMax;
			for (var i = 0; i < spreadMax; i++)
			{
				FScaleOut[i] = TransformStates[i].TransformationValue.x;
			}
		}
	}
}
