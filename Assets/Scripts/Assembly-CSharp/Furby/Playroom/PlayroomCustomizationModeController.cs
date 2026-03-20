using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomCustomizationModeController : Singleton<PlayroomCustomizationModeController>
	{
		private PlayroomCustomizationMode m_CustomizationMode = PlayroomCustomizationMode.Inactive;

		public GameObject m_BottomBar;

		public GameObject[] m_Buttons;

		public GameObject[] m_Carousels;

		public PlayroomCustomizationMode m_ModeOnEnable;

		public PlayroomHintController m_HintController;

		public bool m_CloseCarouselOnSelection;

		public bool CloseCarouselOnSelection
		{
			get
			{
				return m_CloseCarouselOnSelection;
			}
			set
			{
				m_CloseCarouselOnSelection = value;
			}
		}

		private PlayroomCustomizationModeController()
		{
			m_CustomizationMode = PlayroomCustomizationMode.None;
		}

		public void Initialise()
		{
			GameObject gameObject = GameObject.Find("HintController");
			m_HintController = gameObject.GetComponent<PlayroomHintController>();
		}

		public void BeginCustomizing()
		{
			GameObject[] carousels = m_Carousels;
			foreach (GameObject gameObject in carousels)
			{
				gameObject.SetActive(false);
			}
			SetCustomizationMode(m_ModeOnEnable);
			m_HintController.SelectArea.Enable();
		}

		public void EndCustomizing()
		{
			SetCustomizationMode(PlayroomCustomizationMode.Inactive);
			m_HintController.DisableAll();
		}

		public void RefreshCustomizationMode(PlayroomCustomizationMode newMode)
		{
			m_HintController.SelectArea.Disable();
			SetCustomizationMode(newMode);
		}

		public void SetCustomizationMode(PlayroomCustomizationMode newMode)
		{
			if (m_CustomizationMode == newMode)
			{
				return;
			}
			switch (newMode)
			{
			case PlayroomCustomizationMode.Inactive:
				HideBottomBar();
				if (m_CustomizationMode > PlayroomCustomizationMode.None)
				{
					DeactivateCarousel(m_CustomizationMode);
				}
				m_CustomizationMode = newMode;
				break;
			case PlayroomCustomizationMode.None:
				ShowBottomBar();
				if (m_CustomizationMode > PlayroomCustomizationMode.None)
				{
					DeactivateCarousel(m_CustomizationMode);
				}
				m_CustomizationMode = newMode;
				break;
			default:
				if (m_CustomizationMode > PlayroomCustomizationMode.None)
				{
					SwitchActiveCarousel(m_CustomizationMode, newMode);
				}
				else
				{
					ActivateCarousel(newMode);
					ShowBottomBar();
				}
				m_CustomizationMode = newMode;
				break;
			}
		}

		private void ActivateCarousel(PlayroomCustomizationMode mode)
		{
			GameObject gameObject = m_Carousels[(int)mode];
			gameObject.GetComponent<CarouselController>().EnableCarousel();
			m_Buttons[(int)mode].transform.Find("Highlight").gameObject.SetActive(true);
		}

		private void DeactivateCarousel(PlayroomCustomizationMode mode)
		{
			GameObject gameObject = m_Carousels[(int)mode];
			gameObject.GetComponent<CarouselController>().DisableCarousel();
			m_Buttons[(int)mode].transform.Find("Highlight").gameObject.SetActive(false);
		}

		private void ShowBottomBar()
		{
			m_BottomBar.SetActive(true);
		}

		private void HideBottomBar()
		{
			m_BottomBar.SetActive(false);
		}

		private void SwitchActiveCarousel(PlayroomCustomizationMode oldMode, PlayroomCustomizationMode newMode)
		{
			if (oldMode != newMode)
			{
				DeactivateCarousel(oldMode);
				ActivateCarousel(newMode);
			}
		}
	}
}
