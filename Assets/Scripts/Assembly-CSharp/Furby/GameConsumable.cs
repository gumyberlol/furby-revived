using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class GameConsumable
	{
		[SerializeField]
		public string StoreID;

		[SerializeField]
		public int ContentUnits;
	}
}
