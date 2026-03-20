using System.Collections.Generic;
using UnityEngine;

namespace Furby.Playroom
{
	public class SelectableFeatureList : ScriptableObject
	{
		public List<SelectableFeature> Features = new List<SelectableFeature>();

		public IEnumerator<SelectableFeature> GetEnumerator()
		{
			foreach (SelectableFeature feature in Features)
			{
				yield return feature;
			}
		}
	}
}
