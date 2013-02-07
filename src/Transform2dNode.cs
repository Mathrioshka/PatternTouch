using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	[PluginInfo(Name = "Transform", Category = "PatternTouch", Version = "2D", Help = "Drag, scale and rotate 2D object", Tags = "multitouch")]
	public class Transform : IPluginEvaluate
	{
		[Input("Initial Transform")]
		IDiffSpread<Matrix4x4> FInitialTransformIn;

		[Input("ID")]
		IDiffSpread<int> FIdIn;

		[Input("Blobs")]
		ISpread<Blob> FBlobIn;

		[Input("Allow Drag", DefaultBoolean = true, Visibility = PinVisibility.OnlyInspector)]
		ISpread<bool> FAllowDragIn;

		[Input("Allow Scale", DefaultBoolean = true, Visibility = PinVisibility.OnlyInspector)]
		ISpread<bool> FAllowScaleIn;

		[Input("Allow Rotate", DefaultBoolean = true, Visibility = PinVisibility.OnlyInspector)]
		ISpread<bool> FAllowRotateIn;

		[Input("Reset", IsBang = true)]
		private ISpread<bool> FResetIn;
		
		[Output("Tranform")]
		ISpread<Matrix4x4> FTranformOut;

		[Import] 
		private ILogger FLogger;

		private readonly Spread<Blob> FPBlobs = new Spread<Blob>();
		private readonly List<TransformState> FTransformStates = new List<TransformState>();
		
		private bool FReinitTransforms;

		public void Evaluate(int spreadMax)
		{
			spreadMax = Math.Max(FIdIn.SliceCount, FInitialTransformIn.SliceCount);

			TouchUtils.SetIsNew(FBlobIn, FPBlobs);

			if (FIdIn.IsChanged || FInitialTransformIn.IsChanged)
			{
				FReinitTransforms = true;
				FTransformStates.Clear();
			}

			for (var i = 0; i < spreadMax; i++)
			{
				if (FReinitTransforms)
				{
					FTransformStates.Add(new TransformState(FIdIn[i], FInitialTransformIn[i]));
				}

				if (FResetIn[i])
				{
					FTransformStates[i].Reset(FIdIn[i], FInitialTransformIn[i]);
				}

				FTransformStates[i].Update(FBlobIn);

				if (FTransformStates[i].Phase == TransformPhase.Transforming)
				{
					FTransformStates[i].Transformation = TransformObject(FTransformStates[i], i);
				}

				FTransformStates[i].UpdatePBlobs();
			}

			FReinitTransforms = false;
			FPBlobs.SliceCount = FBlobIn.SliceCount;
			FPBlobs.AssignFrom(FBlobIn);

			//Output Data
			FTranformOut.SliceCount = spreadMax;
			for (var i = 0; i < spreadMax; i++)
			{
				FTranformOut[i] = FTransformStates[i].Transformation;
			}
		}

		private Matrix4x4 TransformObject(TransformState transformState, int sliceIndex)
		{
			var deltaScale = TouchUtils.CalculateTransform(transformState, TransformType.Scale);
			transformState.PScale = deltaScale;

			var deltaRotate = TouchUtils.CalculateTransform(transformState, TransformType.Rotate);
			transformState.PRotation = deltaRotate;

			var deltaTranslate = TouchUtils.CalculateTransform(transformState, TransformType.Translate);
			transformState.PTranslation = deltaTranslate;

			FLogger.Log(LogType.Debug, deltaScale.x + " " + deltaRotate.x + " " + deltaTranslate.x);

			Vector3D rotation;
			Vector3D translation;
			Vector3D scale;
			transformState.Transformation.Decompose(out scale, out rotation, out translation);

			return VMath.Transform(new Vector3D(translation.x + deltaTranslate.x, translation.y + deltaTranslate.y, translation.z), new Vector3D(Math.Max(scale.x + deltaScale.x, 0), Math.Max(scale.y + deltaScale.y, 0), Math.Max(scale.z + deltaScale.x, 0)), new Vector3D(rotation.x, rotation.y, rotation.z + deltaRotate.x));
		}
	}
}
