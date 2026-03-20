using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class VideoSettings : MonoBehaviour
	{
		private GameEventSubscription m_debugPanelSubscription;

		public bool m_showVideos;

		private void OnEnable()
		{
			m_debugPanelSubscription = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDestroy()
		{
			m_debugPanelSubscription.Dispose();
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Video Settings"))
			{
				m_showVideos = GUILayout.Toggle(m_showVideos, "Show Videos");
			}
			DebugPanel.EndSection();
		}
	}
}
