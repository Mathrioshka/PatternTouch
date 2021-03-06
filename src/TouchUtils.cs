﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;

namespace VVVV.Nodes.PatternTouch
{
	public static class TouchUtils
	{
		private const double CutOff = 0.57;

		public static List<Blob> GetBlobHits(int targetId, IEnumerable<Blob> blobs)
		{
			return blobs.Where(blob => blob.HitId == targetId).ToList();
		}

		public static bool IsAnyNew(IEnumerable<Blob> blobs)
		{
			return blobs.Any(blob => blob.IsNew);
		}

		public static void UpdateBlobs(ISpread<Blob> allBlobs, ISpread<Blob> currentBlobs, int id)
		{
			for (var i = 0; i < currentBlobs.Count(); i++)
			{
				var found = false;

				for (var j = 0; j < allBlobs.Count(); j++)
				{
					//Check if blob exists
					if (currentBlobs[i].Id != allBlobs[j].Id) continue;

					//Check if blob not hited another object
					//TODO: Find, if this needed or not.
					//if(allBlobs[j].HitId > 0 && allBlobs[j].HitId != id) continue;

					currentBlobs[i] = allBlobs[j];
					found = true;
					break;
				}

				if(!found) currentBlobs.RemoveAt(i);
			}
		}

		public static bool AddNewHits(ISpread<Blob> hits, ISpread<Blob> currentBlobs)
		{
			var isAdded = false;

			foreach (var hit in hits.Where(hit => hit.IsNew))
			{
				currentBlobs.Add(hit);
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
					//TODO: Proper new blob detection
					blobs[i] = new Blob { Position = blobs[i].Position, HitId = blobs[i].HitId, Id = blobs[i].Id, IsNew = pBlobs.All(blob => blob.Id != blobs[i].Id)};
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
			
			return blobs.Aggregate(summ, (current, blob) => current + blob.Position) / blobs.Count();
		}

		public static double Frac(double value) { return value - Math.Truncate(value); }

		public static int ToInt(this bool value)
		{
			return value ? 1 : 0;
		}

		public static double ButterworthLowPassFilter(double value, double pValue, double cutOff = CutOff)
		{
			return  value - cutOff * pValue;
		}

		public static Vector2D ButterworthLowPassFilter(Vector2D value, Vector2D pValue, double cutOff = CutOff)
		{
			return value - cutOff * pValue;
		}

		public static Vector3D ButterworthLowPassFilter(Vector3D value, Vector3D pValue, double cutOff = CutOff)
		{
			return value - cutOff * pValue;
		}

		public static double LinearEasing(this double value, double pValue, double easing = 0.1)
		{
			return pValue + (value - pValue) * easing;
		}

		public static Vector2D LinearEasing(this Vector2D value, Vector2D pValue, double easing = 0.1)
		{
			return pValue + (value - pValue) * easing;
		}

		public static Vector3D LinearEasing(this Vector3D value, Vector3D pValue, double easing = 0.1)
		{
			return pValue + (value - pValue) * easing;
		}

		public static Vector2D CalculateTransform(TransformState state, TransformType type)
		{
			var blobs = state.Blobs;
			var pBlobs = state.PBlobs;

			if (pBlobs.Count() != blobs.Count())
			{
				return new Vector2D();
			}

			var value = new Vector2D();
			var pValue = new Vector2D();
			var delta = new Vector2D();

			switch (type)
			{
				case TransformType.Scale:
					if (blobs.Count() < 2)
					{
						Debug.WriteLine("[WARNING] Scale state have less than 2 fingers");
						return new Vector2D();
					}
					value.x = value.y = VMath.Dist(blobs.First().Position, blobs.Last().Position);
					pValue.x = pValue.y = VMath.Dist(pBlobs.First().Position, pBlobs.Last().Position);
					delta = value - pValue;
					//Debug.WriteLine(blobs.Count());
					break;
				case TransformType.Rotate:
					if (blobs.Count() < 2)
					{
						//Debug.WriteLine("[WARNING] Rotate state have less than 2 fingers");
						return new Vector2D();
					}

					value.x = value.y = FindAngle(blobs.First(), blobs.Last());
					pValue.x = pValue.y = FindAngle(pBlobs.First(), pBlobs.Last());
					delta.x = delta.y = SubtractCycles(value.x, pValue.x);
					break;
				case TransformType.Translate:
					value = FindCentroid(blobs);
					pValue = FindCentroid(pBlobs);
					delta = value - pValue;
					break;
			}
			
			//TODO:Check if Min Max needed
			//if (Math.Abs(delta.x) > 0.1 || Math.Abs(delta.y) > 0.1) delta = new Vector2D();

			return delta;
			//return delta.LinearEasing(pDelta);
		}
	}
}
