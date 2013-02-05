using System.Collections.Generic;
using System.Linq;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.PatternTouch
{
	public class TouchUtils
	{
		public static List<Blob> GetBlobHits(int targetId, IEnumerable<Blob> blobs)
		{
			return blobs.Where(blob => blob.HitId == targetId).ToList();
		}

		public static bool IsNew(IEnumerable<Blob> blobs)
		{
			return blobs.Any(blob => blob.IsNew);
		}

		public static void CleanBlobs(ISpread<Blob> allBlobs, List<Blob> currentBlobs)
		{
			for (var i = 0; i < currentBlobs.Count(); i++)
			{
				if (!allBlobs.Contains(currentBlobs[i]))
				{
					currentBlobs.RemoveAt(i);
				}
			}
		}
	}
}
