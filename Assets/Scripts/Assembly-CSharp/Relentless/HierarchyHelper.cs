using System;
using UnityEngine;

namespace Relentless
{
	public static class HierarchyHelper
	{
		public static T FindParent<T>(Transform t) where T : Component
		{
			try
			{
				T component = t.GetComponent<T>();
				if (component != null)
				{
					return component;
				}
				if (t.parent != null)
				{
					return FindParent<T>(t.parent);
				}
				return (T)null;
			}
			catch (Exception ex)
			{
				Logging.LogError(ex.ToString());
				return (T)null;
			}
		}
	}
}
