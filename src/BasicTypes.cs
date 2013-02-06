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

	public struct VPA
	{
		public Matrix4x4 View;
		public Matrix4x4 Projection;
		public Matrix4x4 AspectRatio;
	}

	public enum GestureType
	{
		Transform
	}

	public enum TransformType
	{
		Scale,
		Rotate,
		Translate
	}

	public enum TransformPhase
	{
		Idle,
		Transforming
	}
}
