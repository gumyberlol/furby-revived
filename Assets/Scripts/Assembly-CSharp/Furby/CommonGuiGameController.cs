using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class CommonGuiGameController : GameEventReceiver
	{
		public LayerMask m_ExcludeLayers;

		[SerializeField]
		private string m_BackScreen;

		public override Type EventType
		{
			get
			{
				return typeof(SharedGuiEvents);
			}
		}

		protected void SetObjectStates(bool objectState)
		{
			Type typeFromHandle = typeof(BoxCollider);
			int value = m_ExcludeLayers.value;
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeFromHandle);
			for (int i = 0; i < array.Length; i++)
			{
				Collider collider = (Collider)array[i];
				if ((value & (1 << collider.gameObject.layer)) == 0)
				{
					collider.enabled = objectState;
				}
			}
			Time.timeScale = ((!objectState) ? 0f : 1f);
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((SharedGuiEvents)(object)enumValue)
			{
			case SharedGuiEvents.Pause:
				SetObjectStates(false);
				break;
			case SharedGuiEvents.Resume:
				SetObjectStates(true);
				break;
			case SharedGuiEvents.Restart:
				Application.LoadLevelAsync(Application.loadedLevelName);
				break;
			case SharedGuiEvents.Quit:
				if (!string.IsNullOrEmpty(m_BackScreen))
				{
					FurbyGlobals.ScreenSwitcher.BackToScreen(m_BackScreen);
				}
				else
				{
					FurbyGlobals.ScreenSwitcher.BackScreen();
				}
				break;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			Time.timeScale = 1f;
		}
	}
}
