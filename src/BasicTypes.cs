using System.Collections.Generic;
using System.Linq;
using VVVV.PluginInterfaces.V2;
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
		public Spread<Blob> Blobs { get; set; }
		public Spread<Blob> PBlobs { get; set; }
		public TransformPhase Phase { get; set; }
		public int Id { get; set; }

		public TransformState(int id, Matrix4x4 transformation)
		{
			Reset(id, transformation);
		}

		private void StrartTransformtation(List<Blob> hits)
		{
			var newBlobs = hits.Where(blob => blob.IsNew).ToList();
			
			Blobs.AssignFrom(newBlobs);
			Phase = TransformPhase.Transforming;
		}

		public void StopTransformation()
		{
			Blobs.SliceCount = 0;
			Phase = TransformPhase.Idle;
		}

		public void Reset(int id, Matrix4x4 transformtation)
		{
			Transformation = transformtation;
			Id = id;
			Blobs.SliceCount = 0;
			PBlobs.SliceCount = 0;

			Phase = TransformPhase.Idle;
		}

		public void Update(ISpread<Blob> availableBlobs)
		{
			var hits = TouchUtils.GetBlobHits(Id, availableBlobs);

			switch (Phase)
			{
				case TransformPhase.Idle:
					if (hits.Count > 0 && TouchUtils.IsNew(hits))
					{
						StrartTransformtation(hits);
					}
					break;
				case TransformPhase.Transforming:
					TouchUtils.CleanDeadBlobs(availableBlobs, Blobs);
					TouchUtils.AddNewHits(hits.ToSpread(), Blobs);
					break;
			}
		}

		public void UpdatePBlobs()
		{
			PBlobs.SliceCount = Blobs.SliceCount;
			PBlobs.AssignFrom(Blobs);
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
