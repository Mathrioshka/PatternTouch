using System.Collections.Generic;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	public struct Blob
	{
		public Vector2D Position;
		public int Id;
		public int HitId;
		public bool IsNew;
	}

	public class TransformState
	{
		public Matrix4x4 Transformation { get; set; }
		public List<Blob> Blobs { get; set; }
		public List<Blob> PBlobs { get; set; }
		public TransformPhase Phase { get; set; }

		public TransformState(Matrix4x4 transformation)
		{
			Reset(transformation);
		}

		public void StrartTransformtation(List<Blob> hits)
		{
			Phase = TransformPhase.Transforming;
			PBlobs = new List<Blob>(hits);
			Blobs = hits;
		}

		public void StopTransformation()
		{
			Phase = TransformPhase.Idle;

			Blobs = null;
			PBlobs = null;
		}

		public void Reset(Matrix4x4 transformtation)
		{
			Phase = TransformPhase.Idle;
			Transformation = transformtation;
			Blobs = new List<Blob>();
			PBlobs = new List<Blob>();
		}
	}

	public enum GestureType
	{
		Transform
	}

	public enum TransformPhase
	{
		Idle,
		Transforming
	}
}
