using System.Collections.Generic;
using System.Linq;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	public class TransformState
	{
		public Vector2D TransformationValue { get; set; }
		public Spread<Blob> Blobs { get; set; }
		public Spread<Blob> PBlobs { get; set; }
		public TransformPhase Phase { get; set; }
		public int Id { get; set; }
		public Vector2D PDelta { get; set; }

		public TransformState(int id, Vector2D value)
		{
			Blobs = new Spread<Blob>();
			PBlobs = new Spread<Blob>();

			Reset(id, value);
		}

		private void StrartTransformtation(IEnumerable<Blob> hits)
		{
			var newBlobs = hits.Where(blob => blob.IsNew).ToList();

			Blobs.AssignFrom(newBlobs);
			Phase = TransformPhase.Transforming;
		}

		public void StopTransformation()
		{
			Blobs.SliceCount = 0;
			PDelta = new Vector2D();

			Phase = TransformPhase.Idle;
		}

		public void Reset(int id, double value)
		{
			Reset(id, new Vector2D(value));
		}

		public void Reset(int id, Vector2D value)
		{
			TransformationValue = value;
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

		public void UpdatePValues()
		{
			PBlobs.SliceCount = Blobs.SliceCount;
			PBlobs.AssignFrom(Blobs);
		}
	}
}
