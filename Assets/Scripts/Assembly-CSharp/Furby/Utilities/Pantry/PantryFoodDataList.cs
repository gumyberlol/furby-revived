using System.Collections.Generic;
using UnityEngine;

namespace Furby.Utilities.Pantry
{
	public class PantryFoodDataList : ScriptableObject
	{
		public List<PantryFoodData> Items = new List<PantryFoodData>();

		public PantryFoodData Find(string name)
		{
			foreach (PantryFoodData item in Items)
			{
				if (item.Name == name)
				{
					return item;
				}
			}
			return null;
		}

		public IEnumerator<PantryFoodData> GetEnumerator()
		{
			foreach (PantryFoodData item in Items)
			{
				yield return item;
			}
		}
	}
}
