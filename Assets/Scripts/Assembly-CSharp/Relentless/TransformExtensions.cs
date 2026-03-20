using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public static class TransformExtensions
	{
		public static string GetPath(this Transform xform)
		{
			if (xform.parent == null)
			{
				return "/" + xform.name;
			}
			return xform.parent.GetPath() + "/" + xform.name;
		}

		public static Transform GetNamedChildTransform(this Transform xform, string name)
		{
			Queue<Transform> queue = new Queue<Transform>();
			foreach (Transform item3 in xform)
			{
				queue.Enqueue(item3);
			}
			while (queue.Count != 0)
			{
				Transform transform = queue.Dequeue();
				if (transform.name == name)
				{
					return transform;
				}
				foreach (Transform item4 in transform)
				{
					queue.Enqueue(item4);
				}
			}
			return null;
		}
	}
}
