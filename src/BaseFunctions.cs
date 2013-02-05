using System.Collections.Generic;
using System.Linq;

namespace VVVV.Nodes.PatternTouch
{
	public class TouchUtils
	{
		public static List<Blob> GetBlobHits(int targetId, IEnumerable<Blob> blobs)
		{
			return blobs.Where(blob => blob.HitId == targetId).ToList();
		}
	}
}
