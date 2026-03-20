using System.Collections.Generic;
using UnityEngine;

namespace Furby.Data
{
	public class FurbyPersonalityTable : ScriptableObject
	{
		public List<FurbyPersonality> Personalities = new List<FurbyPersonality>();
	}
}
