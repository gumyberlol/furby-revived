using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class HintEventTranslator : GameEventTranslator<HintEventReaction>
	{
		public HintEventReaction[] m_eventTable;

		public Dictionary<HintEvents, bool> m_ExclusionList = new Dictionary<HintEvents, bool>();

		protected override HintEventReaction[] EventTable
		{
			get
			{
				return m_eventTable;
			}
		}

		protected override void OnEvent(Enum enumValue, GameObject gObj, params object[] list)
		{
			if (!IsExcluded(enumValue))
			{
				base.OnEvent(enumValue, base.gameObject, list);
			}
		}

		private bool IsExcluded(Enum enumValue)
		{
			HintEvents key = (HintEvents)(object)enumValue;
			bool value;
			if (m_ExclusionList.TryGetValue(key, out value))
			{
				return value;
			}
			return false;
		}
	}
}
