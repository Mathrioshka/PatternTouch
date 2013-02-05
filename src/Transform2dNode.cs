using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using VVVV.Core.Logging;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	[PluginInfo(Name = "Tranform", Category = "PatternTouch", Version = "2D", Help = "Drag, scale and rotate 2D object", Tags = "multitouch")]
	public class Transform : IPluginEvaluate
	{
		[Input("Initial Transform")]
		IDiffSpread<Matrix4x4> FInitialTransformIn;

		[Input("ID")]
		IDiffSpread<int> FIdIn;

		[Input("Blobs")]
		ISpread<Blob> FBlobIn;

		[Input("Allow Drag")]
		ISpread<bool> FAllowDragIn;

		[Input("Allow Scale")]
		ISpread<bool> FAllowScaleIn;

		[Input("Allow Rotate")]
		ISpread<bool> FAllowRotateIn;

		[Input("Reset", IsBang = true)]
		private ISpread<bool> FResetIn;
		
		[Output("Tranform")]
		ISpread<Matrix4x4> FTranformOut;

		[Import] 
		private ILogger FLogger;

		private List<TransformState> FTransformStates = new List<TransformState>(); 
		
		private bool FReinitTransforms;

		public void Evaluate(int spreadMax)
		{
			spreadMax = Math.Max(FIdIn.SliceCount, FInitialTransformIn.SliceCount);

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
					FTransformStates[i].Transformation = TransformObject(FTransformStates[i]);
				}

				FTransformStates[i].UpdatePBlobs();
			}

			FReinitTransforms = false;

			//Output Data
			FTranformOut.SliceCount = spreadMax;
			for (var i = 0; i < spreadMax; i++)
			{
				FTranformOut[i] = FTransformStates[i].Transformation;
			}
		}

		private Matrix4x4 TransformObject(TransformState transformState)
		{
			
			if (transformState.Blobs.SliceCount < 2 || transformState.PBlobs.SliceCount == 0) return transformState.Transformation;

			var distance = VMath.Dist(transformState.Blobs[0].Position, transformState.Blobs[1].Position);
			var pDistance = VMath.Dist(transformState.PBlobs[0].Position, transformState.PBlobs[1].Position);

			var delta = (distance - pDistance) / 10;

			if (Math.Abs(delta - 0) < 0.001) delta = 0;

			Vector3D rotation;
			Vector3D translation;
			Vector3D scale;

			transformState.Transformation.Decompose(out scale, out rotation, out translation);

			FLogger.Log(LogType.Debug, (scale.x + delta).ToString());

			return VMath.Transform(translation, scale + delta, rotation);
		}
	}
}
