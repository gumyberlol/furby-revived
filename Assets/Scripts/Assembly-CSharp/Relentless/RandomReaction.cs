using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class RandomReaction : GameEventReaction
	{
		[SerializeField]
		private SerialisableEnum[] EventSelection = new SerialisableEnum[0];

		public override void React(GameObject gameObject, params object[] arguments)
		{
			int num = EventSelection.Length;
			if (num != 0)
			{
				int num2 = UnityEngine.Random.Range(0, num);
				Enum eventType = EventSelection[num2];
				GameEventRouter.SendEvent(eventType, gameObject, arguments);
			}
		}
	}
}
