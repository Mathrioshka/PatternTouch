using System;
using System.Collections.Generic;
using System.Linq;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	public static class TouchUtils
	{
		public static List<Blob> GetBlobHits(int targetId, IEnumerable<Blob> blobs)
		{
			return blobs.Where(blob => blob.HitId == targetId).ToList();
		}

		public static bool IsAnyNew(IEnumerable<Blob> blobs)
		{
			return blobs.Any(blob => blob.IsNew);
		}

		public static void UpdateBlobs(ISpread<Blob> allBlobs, ISpread<Blob> currentBlobs)
		{
			for (var i = 0; i < currentBlobs.Count(); i++)
			{
				var found = false;

				for (int j = 0; j < allBlobs.Count(); j++)
				{
					if (currentBlobs[i].Id != allBlobs[j].Id) continue;
					currentBlobs[i] = allBlobs[j];
					found = true;
					break;
				}

				if(!found) currentBlobs.RemoveAt(i);
			}
		}

		public static bool AddNewHits(ISpread<Blob> hits, ISpread<Blob> currentBlob)
		{
			var isAdded = false;

			for (var i = 0; i < hits.SliceCount; i++)
			{
				if (!hits[i].IsNew) continue;
				
				currentBlob.Add(hits[i]);
				isAdded = true;
			}

			return isAdded;
		}

		public static void SetIsNew(ISpread<Blob> blobs, ISpread<Blob> pBlobs)
		{
			if (pBlobs.SliceCount == 0)
			{
				for (var i = 0; i < blobs.SliceCount; i++)
				{
					blobs[i] = new Blob{Position = blobs[i].Position, HitId = blobs[i].HitId, Id = blobs[i].Id, IsNew = true};
				}
			}
			else
			{
				for (var i = 0; i < blobs.SliceCount; i++)
				{
					blobs[i] = new Blob{Position = blobs[i].Position, HitId = blobs[i].HitId, Id = blobs[i].Id, IsNew = pBlobs[i].Id != blobs[i].Id};
				}
			}

			pBlobs.SliceCount = blobs.SliceCount;
			pBlobs.AssignFrom(blobs);
		}

		public static double FindAngle(Blob firstBlob, Blob secondBlob)
		{
			var angle = Math.Atan2(secondBlob.Position.y - firstBlob.Position.y,
								   secondBlob.Position.x - firstBlob.Position.x);
			return Frac(angle * VMath.RadToCyc);
		}

		public static double SubtractCycles(double first, double second)
		{
			var value = first - second - 0.5;
			return (value - Math.Floor(value)) - 0.5;
		}

		public static Vector2D FindCentroid(ISpread<Blob> blobs)
		{
			var summ = new Vector2D();
			for (var i = 0; i < blobs.Count(); i++)
			{
				summ += blobs[i].Position;
			}

			return summ / blobs.Count();
		}

		public static double Frac(double value) { return value - Math.Truncate(value); }

		public static int ToInt(this bool value)
		{
			return value ? 1 : 0;
		}
	}
}
