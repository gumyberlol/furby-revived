using System;
using UnityEngine;

namespace Relentless
{
	public class AnimGameEventTranslator : GameEventTranslator<AnimEventReaction>
	{
		public GameObject Target;

		public AnimEventReaction[] m_eventTable;

		protected override AnimEventReaction[] EventTable
		{
			get
			{
				return m_eventTable;
			}
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] list)
		{
			base.OnEvent(enumValue, gameObject, (object)Target, (object)list);
		}
	}
}
