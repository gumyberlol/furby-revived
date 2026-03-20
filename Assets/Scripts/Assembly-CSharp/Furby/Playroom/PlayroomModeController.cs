using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomModeController : Singleton<PlayroomModeController>
	{
		private bool m_DEBUGMODE;

		public PlayroomMode m_GameMode;

		private bool m_GameModeIsDirty;

		private GameObject m_InteractionOptions;

		private GameObject m_CustomizationOptions;

		public PlayroomMode GameMode
		{
			get
			{
				return m_GameMode;
			}
		}

		private void Awake()
		{
			m_GameMode = PlayroomMode.Interaction;
			m_InteractionOptions = base.gameObject.GetChildGameObject("MenuPopup_PlayroomMain");
			m_CustomizationOptions = base.gameObject.GetChildGameObject("MenuPopup_Customize");
		}

		private void Start()
		{
			Singleton<PlayroomCustomizationModeController>.Instance.Initialise();
			RefreshGameMode();
			if (FurbyGlobals.Player.SelectedFurbyBaby == FurbyGlobals.Player.InProgressFurbyBaby && FurbyGlobals.Player.SelectedFurbyBaby.Progress == FurbyBabyProgresss.P && FurbyGlobals.Player.SelectedFurbyBaby.ShouldProgressToNeighbourhood())
			{
				PlayroomStartup_TransitionToHood playroomStartup_TransitionToHood = (PlayroomStartup_TransitionToHood)Object.FindObjectOfType(typeof(PlayroomStartup_TransitionToHood));
				if (playroomStartup_TransitionToHood == null)
				{
					GameObject gameObject = new GameObject("SENTINEL_TransitionToHood");
					gameObject.AddComponent<PlayroomStartup_TransitionToHood>().InvokeTransition(0f);
				}
			}
		}

		private void Update()
		{
			if (m_GameModeIsDirty)
			{
				if (m_DEBUGMODE)
				{
					Logging.Log("PlayroomModeController: Detected dirtiness..., setting in mode = " + m_GameMode);
				}
				RefreshGameMode();
				m_GameModeIsDirty = false;
			}
		}

		private void RefreshGameMode()
		{
			if (m_GameMode == PlayroomMode.Interaction)
			{
				m_InteractionOptions.SetActive(true);
				m_CustomizationOptions.SetActive(false);
			}
			if (m_GameMode == PlayroomMode.Customization)
			{
				m_InteractionOptions.SetActive(false);
				m_CustomizationOptions.SetActive(true);
			}
		}

		public void SetInCustomizeMode()
		{
			if (m_DEBUGMODE)
			{
				Logging.Log("PlayroomModeController: SetInCustomizeMode");
			}
			m_GameMode = PlayroomMode.Customization;
			m_GameModeIsDirty = true;
			Singleton<PlayroomCustomizationModeController>.Instance.BeginCustomizing();
		}

		public void SetInInteractionMode()
		{
			if (m_DEBUGMODE)
			{
				Logging.Log("PlayroomModeController: SetInInteractionMode");
			}
			m_GameMode = PlayroomMode.Interaction;
			Singleton<PlayroomCustomizationModeController>.Instance.EndCustomizing();
			m_GameModeIsDirty = true;
		}
	}
}
