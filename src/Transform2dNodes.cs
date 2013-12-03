using System;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	public abstract class Transform2D<T> : TransformProcessor where T : struct
	{
		[Input("Initial Value")]
		public ISpread<T> InitialValueIn;

		[Input("Aspect Ration")]
		protected ISpread<Matrix4x4> AspectRatio;

		protected int PSliceCount;

		protected override int CalculateMax()
		{
			return Math.Max(IdIn.SliceCount, InitialValueIn.SliceCount);
		}

		protected override bool CheckForReinit()
		{
			var result = InitialValueIn.SliceCount != PSliceCount;

			PSliceCount = InitialValueIn.SliceCount;
			return result;
		}
	}
	
	[PluginInfo(Name = "Rotate", Category = "PatternTouch", Version = "2D", Help = "Rotate object", Tags = "multitouch")]
	public class RotateTransformNode : Transform2D<double>
	{
		[Output("Rotation")] 
		public ISpread<double> RotationOut;

		protected override void Reinit(int index)
		{
			TransformStates.Add(new TransformState(IdIn[index], new Vector2D(InitialValueIn[index])));
		}

		protected override void Reset(int index)
		{
			TransformStates[index].Reset(IdIn[index], InitialValueIn[index]);
		}

		protected override void ProcessTransformation(int index)
		{
			var value = TouchUtils.CalculateTransform(TransformStates[index], TransformType.Rotate);
			TransformStates[index].PDelta = value;

			TransformStates[index].TransformationValue += value;
		}

		protected override void OutputData(int spreadMax)
		{
			RotationOut.SliceCount = spreadMax;
			for (var i = 0; i < spreadMax; i++)
			{
				RotationOut[i] = TransformStates[i].TransformationValue.x;
			}
		}

		
	}

	[PluginInfo(Name = "Translate", Category = "PatternTouch", Version = "2D", Help = "Translate object", Tags = "multitouch")]
	public class TranslateTransformNode : Transform2D<Vector2D> 
	{
		[Output("Translate")]
		public ISpread<Vector2D> TranslateOut;

		protected override void Reinit(int index)
		{
			TransformStates.Add(new TransformState(IdIn[index], InitialValueIn[index]));
		}

		protected override void Reset(int index)
		{
			TransformStates[index].Reset(IdIn[index], InitialValueIn[index]);
		}

		protected override void ProcessTransformation(int index)
		{
			var value = TouchUtils.CalculateTransform(TransformStates[index], TransformType.Translate);
			TransformStates[index].PDelta = value;

			TransformStates[index].TransformationValue += value;
		}

		protected override void OutputData(int spreadMax)
		{
			TranslateOut.SliceCount = spreadMax;
			for (var i = 0; i < spreadMax; i++)
			{
				TranslateOut[i] = TransformStates[i].TransformationValue;
			}
		}
	}

	[PluginInfo(Name = "Scale", Category = "PatternTouch", Version = "2D", Help = "Scale object", Tags = "multitouch")]
	public class ScaleTransformNode : Transform2D<double> 
	{
		[Output("Scale")]
		public ISpread<double> ScaleOut;

		protected override void Reinit(int index)
		{
			TransformStates.Add(new TransformState(IdIn[index], new Vector2D(InitialValueIn[index])));
		}

		protected override void Reset(int index)
		{
			TransformStates[index].Reset(IdIn[index], InitialValueIn[index]);
		}

		protected override void ProcessTransformation(int index)
		{
			var value = TouchUtils.CalculateTransform(TransformStates[index], TransformType.Scale);
			TransformStates[index].PDelta = value;

			TransformStates[index].TransformationValue += value;
		}

		protected override void OutputData(int spreadMax)
		{
			ScaleOut.SliceCount = spreadMax;
			for (var i = 0; i < spreadMax; i++)
			{
				ScaleOut[i] = TransformStates[i].TransformationValue.x;
			}
		}
	}
}
