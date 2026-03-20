using System;
using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class SlangListener : MonoBehaviour
	{
		private GameEventSubscription m_sub;

		private IEnumerator Start()
		{
			while (!Singleton<FurbyDataChannel>.Exists)
			{
				yield return null;
			}
			Singleton<FurbyDataChannel>.Instance.ToneEvent += OnTone;
			m_sub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnTone(FurbyMessageType mesgType, long num)
		{
			if (mesgType == FurbyMessageType.Slang)
			{
				FurbyAction furbyAction = (FurbyAction)num;
				string item = furbyAction.ToString();
				GameData data = Singleton<GameDataStoreObject>.Instance.Data;
				if (!data.m_unlockedSlangs.Contains(item))
				{
					data.m_unlockedSlangs.Add(item);
				}
				data.m_unconfirmedSlangs.Remove(item);
			}
		}

		public void OnDestroy()
		{
			m_sub.Dispose();
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Slang"))
			{
				GameData data = Singleton<GameDataStoreObject>.Instance.Data;
				GUILayout.Label("Unconfirmed:");
				DebugStringList(data.m_unconfirmedSlangs);
				GUILayout.Label("Unlocked:");
				DebugStringList(data.m_unlockedSlangs);
			}
			DebugPanel.EndSection();
		}

		private void DebugStringList(List<string> phrases)
		{
			foreach (string phrase in phrases)
			{
				GUILayout.Label(phrase);
			}
		}
	}
}
