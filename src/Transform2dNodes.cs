using System;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	public abstract class Transform2d<T> : TransformProcessor where T : struct
	{
		[Input("Initial Value")]
		private ISpread<T> FInitialValueIn;

		[Input("Aspect Ration")]
		protected ISpread<Matrix4x4> AspectRatio;

		protected int PSliceCount;

		protected override int CalculateMax()
		{
			return Math.Max(IdIn.SliceCount, FInitialValueIn.SliceCount);
		}

		protected override bool CheckForReinit()
		{
			var result = FInitialValueIn.SliceCount != PSliceCount;

			PSliceCount = FInitialValueIn.SliceCount;
			return result;
		}
	}
	
	[PluginInfo(Name = "Rotate", Category = "PatternTouch", Version = "2D", Help = "Rotate object", Tags = "multitouch")]
	public class RotateTransformNode : Transform2d<double>
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

		protected override void ProcessTransformation(int index)
		{
			var value = TouchUtils.CalculateTransform(TransformStates[index], TransformType.Rotate);
			TransformStates[index].PDelta = value;

			TransformStates[index].TransformationValue += value;
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
	public class TranslateTransformNode : Transform2d<Vector2D> 
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

		protected override void ProcessTransformation(int index)
		{
			var value = TouchUtils.CalculateTransform(TransformStates[index], TransformType.Translate);
			TransformStates[index].PDelta = value;

			TransformStates[index].TransformationValue += value;
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
	public class ScaleTransformNode : Transform2d<double> 
	{
		[Input("Initial Value", DefaultValues = new double[]{1, 1})]
		private ISpread<double> FInitialValueIn;

		[Output("Scale")]
		private ISpread<double> FScaleOut;

		protected override void Reinit(int index)
		{
			TransformStates.Add(new TransformState(IdIn[index], new Vector2D(FInitialValueIn[index])));
		}

		protected override void Reset(int index)
		{
			TransformStates[index].Reset(IdIn[index], FInitialValueIn[index]);
		}

		protected override void ProcessTransformation(int index)
		{
			var value = TouchUtils.CalculateTransform(TransformStates[index], TransformType.Scale);
			TransformStates[index].PDelta = value;

			TransformStates[index].TransformationValue += value;
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
