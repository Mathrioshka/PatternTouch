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

		private List<Matrix4x4> FTransforms = new List<Matrix4x4>();
		private List<TransformState> FTransformStates = new List<TransformState>();
		private List<List<Blob>> FPreviousBlobs = new List<List<Blob>>();
		
		private bool FReinitTransforms;

		public void Evaluate(int spreadMax)
		{
			spreadMax = Math.Max(FIdIn.SliceCount, FInitialTransformIn.SliceCount);

			if (FIdIn.IsChanged || FInitialTransformIn.IsChanged)
			{
				FReinitTransforms = true;
				
				FTransforms.Clear();
				FTransformStates.Clear();
				FPreviousBlobs.Clear();
			}

			for (var i = 0; i < spreadMax; i++)
			{
				if (FReinitTransforms)
				{
					FTransforms.Add(FInitialTransformIn[i]);
					FTransformStates.Add(TransformState.Idle);
					FPreviousBlobs.Add(new List<Blob>());
				}

				if (FResetIn[0])
				{
					FTransforms[i] = FInitialTransformIn[i];
				}

				var currentId = FIdIn[i];

				var hits = TouchUtils.GetBlobHits(currentId, FBlobIn);
				
				if (hits.Count == 0 && FTransformStates[i] == TransformState.Idle )
				{
					continue;
				}

				switch (FTransformStates[i])
				{
					case TransformState.Idle:
						FTransformStates[i] = TransformState.Transforming;
						FPreviousBlobs[i] = new List<Blob>(hits);
						continue;
					case TransformState.Transforming:
						FTransforms[i] = TransformObject(hits, FPreviousBlobs[i], FTransforms[i]);
						FPreviousBlobs[i] = new List<Blob>(hits);
						break;
				}
			}

			FReinitTransforms = false;

			FTranformOut.SliceCount = spreadMax;
			FTranformOut.AssignFrom(FTransforms);
		}

		private Matrix4x4 TransformObject(List<Blob> blobs, List<Blob> pBlobs, Matrix4x4 transformation)
		{
			if (blobs.Count < 2 || pBlobs.Count < 2) return transformation;

			var distance = VMath.Dist(blobs[0].Position, blobs[1].Position);
			var pDistance = VMath.Dist(pBlobs[0].Position, pBlobs[1].Position);

			var delta = (distance - pDistance) / 10;

			if (Math.Abs(delta - 0) < 0.001) delta = 0;

			Vector3D rotation;
			Vector3D translation;
			Vector3D scale;

			transformation.Decompose(out scale, out rotation, out translation);

			FLogger.Log(LogType.Debug, (scale.x + delta).ToString());

			return VMath.Transform(translation, scale + delta, rotation); ;
		}
	}
}
