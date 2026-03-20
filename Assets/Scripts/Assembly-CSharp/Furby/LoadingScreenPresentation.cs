using UnityEngine;

namespace Furby
{
	public class LoadingScreenPresentation : MonoBehaviour
	{
		public enum LoadingScreenMode
		{
			Plain = 0,
			InteractionMessaging = 1,
			UpsellMessaging = 2
		}

		public PhysicalInteractionMessaging m_FurbyMode;

		public UpsellMessaging m_NoFurbyMode;

		public bool m_ForceOnForDebugging;

		public LoadingScreenMode m_ForceMode = LoadingScreenMode.InteractionMessaging;

		public void Start()
		{
			m_FurbyMode.ResetTimestamp();
			m_NoFurbyMode.ResetScreenLoadCounter();
		}

		public LoadingScreenMode GetLoadingScreenMode()
		{
			if (Debug.isDebugBuild && m_ForceOnForDebugging)
			{
				return m_ForceMode;
			}
			if (m_NoFurbyMode.ShouldTriggerUpsellMessage())
			{
				return LoadingScreenMode.UpsellMessaging;
			}
			if (m_FurbyMode.ShouldShowPhysicalInteractionMessage())
			{
				return LoadingScreenMode.InteractionMessaging;
			}
			return LoadingScreenMode.Plain;
		}

		public void MarkLoadingScreenAsViewed(LoadingScreenMode mode)
		{
			m_NoFurbyMode.IncrementScreenLoad();
			switch (mode)
			{
			case LoadingScreenMode.Plain:
				break;
			case LoadingScreenMode.InteractionMessaging:
				m_FurbyMode.ResetTimestamp();
				break;
			case LoadingScreenMode.UpsellMessaging:
				break;
			}
		}
	}
}
