using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class GameEventReaction
	{
		public SerialisableEnum GameEvent;

		public virtual void React(GameObject gameObject, params object[] paramlist)
		{
		}
	}
}
