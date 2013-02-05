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

	public struct TransformState
	{
		public Matrix4x4 Transformation;
		public List<Blob> Blobs;
		public List<Blob> PBlobs;
		public TransformPhase Phase;
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
