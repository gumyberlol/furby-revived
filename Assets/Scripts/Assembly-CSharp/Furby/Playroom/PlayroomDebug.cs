using System;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomDebug : RelentlessMonoBehaviour
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
			if (DebugPanel.StartSection("Playroom"))
			{
				bool flag = false;
				GUILayout.Label("Levelling Sequence:");
				GUILayout.BeginHorizontal();
				flag = GUILayout.Button("Graduate Furbling to Neighbourhood", GUILayout.ExpandWidth(true));
				GUILayout.EndHorizontal();
				if (flag)
				{
					DEBUG_MoveToHood();
				}
			}
			DebugPanel.EndSection();
		}

		private void DEBUG_MoveToHood()
		{
			if (FurbyGlobals.Player.InProgressFurbyBaby != FurbyGlobals.Player.SelectedFurbyBaby)
			{
				if (FurbyGlobals.Player.InProgressFurbyBaby != null)
				{
					FurbyGlobals.Player.InProgressFurbyBaby.Progress = FurbyBabyProgresss.E;
				}
				FurbyGlobals.Player.InProgressFurbyBaby = FurbyGlobals.Player.SelectedFurbyBaby;
			}
			if (FurbyGlobals.Player.SelectedFurbyBaby.Progress == FurbyBabyProgresss.N)
			{
				FurbyGlobals.Player.SelectedFurbyBaby.Progress = FurbyBabyProgresss.P;
			}
			OnDestroy();
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<PlayroomStartup_TransitionToHood>().InvokeTransition(5f);
		}
	}
}
