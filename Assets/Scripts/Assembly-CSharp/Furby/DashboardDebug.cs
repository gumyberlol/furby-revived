using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class DashboardDebug : RelentlessMonoBehaviour
	{
		private GameEventSubscription m_DebugPanelSub;

		private void OnEnable()
		{
			m_DebugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDestroy()
		{
			m_DebugPanelSub.Dispose();
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Dashboard"))
			{
				GUILayout.Label("ComAir reaction tones:");
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				flag = GUILayout.Button("Egg Needs Attention");
				GUILayout.Label("(Imprinting)");
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				flag2 = GUILayout.Button("Baby Needs Attention");
				GUILayout.Label("(Neglected)");
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				flag3 = GUILayout.Button("Virtual Friend Unlocked");
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				flag4 = GUILayout.Button("Mini-game Unlocked");
				GUILayout.EndHorizontal();
				GUILayout.EndHorizontal();
				if (flag)
				{
					GameEventRouter.SendEvent(PlayerFurbyEvent.EggNeedsAttention);
				}
				if (flag2)
				{
					GameEventRouter.SendEvent(PlayerFurbyEvent.BabyNeedsAttention);
				}
				if (flag3)
				{
					GameEventRouter.SendEvent(PlayerFurbyEvent.AdultNewVirtualFriendUnlocked);
				}
				if (flag4)
				{
					GameEventRouter.SendEvent(PlayerFurbyEvent.AdultNewMinigameUnlocked);
				}
			}
			DebugPanel.EndSection();
		}
	}
}
