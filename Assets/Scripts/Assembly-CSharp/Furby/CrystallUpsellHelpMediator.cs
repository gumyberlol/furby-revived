using System;
using Furby.Utilities.Help;
using UnityEngine;

namespace Furby
{
	public class CrystallUpsellHelpMediator : MonoBehaviour
	{
		public HelpPanel m_TargetPanel;

		public HelpData m_HelpData;

		private GameEventSubscription m_EventSubscription;

		public void Awake()
		{
			DebugUtils.Log_InCyan("CrystallUpsellInfo Listening...");
			m_EventSubscription = new GameEventSubscription(OnInfoRequested, CrystallUpsellInfoEvents.SpawnFindOutMore);
		}

		private void OnInfoRequested(Enum eventType, GameObject obj, params object[] parameters)
		{
			DebugUtils.Log_InCyan("CrystallUpsellInfo.OnInfoRequested");
			if (eventType.Equals(CrystallUpsellInfoEvents.SpawnFindOutMore))
			{
				DebugUtils.Log_InCyan("CrystallUpsellInfo.OnInfoRequested HANDLED");
				m_TargetPanel.gameObject.SetActive(true);
				m_TargetPanel.SetupFrom(m_HelpData);
				m_TargetPanel.Start();
			}
		}
	}
}
