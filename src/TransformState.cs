using System.Collections.Generic;
using System.Linq;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	public class TransformState
	{
		public Matrix4x4 Transformation { get; set; }
		public Spread<Blob> Blobs { get; set; }
		public Spread<Blob> PBlobs { get; set; }
		public TransformPhase Phase { get; set; }
		public int Id { get; set; }
		public Vector2D PTranslation { get; set; }
		public double PScale { get; set; }
		public double PRotation { get; set; }

		public TransformState(int id, Matrix4x4 transformation)
		{
			Blobs = new Spread<Blob>();
			PBlobs = new Spread<Blob>();

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
			PRotation = 0;
			PScale = 0;
			PTranslation = new Vector2D();

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
					if (hits.Count > 0 && TouchUtils.IsAnyNew(hits))
					{
						StrartTransformtation(hits);
					}
					break;
				case TransformPhase.Transforming:
					var pCount = Blobs.Count();

					TouchUtils.UpdateBlobs(availableBlobs, Blobs);
					TouchUtils.AddNewHits(hits.ToSpread(), Blobs);

					if (pCount != Blobs.Count()) PBlobs.SliceCount = 0;
					if (Blobs.SliceCount == 0) StopTransformation();
					break;
			}
		}

		public void UpdatePBlobs()
		{
			PBlobs.SliceCount = Blobs.SliceCount;
			PBlobs.AssignFrom(Blobs);
		}
	}
}
