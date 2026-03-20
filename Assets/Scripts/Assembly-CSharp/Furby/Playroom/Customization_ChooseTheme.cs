using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class Customization_ChooseTheme : RelentlessMonoBehaviour
	{
		public PlayroomHintController m_HintController;

		private void Start()
		{
			GameObject gameObject = GameObject.Find("HintController");
			m_HintController = gameObject.GetComponent<PlayroomHintController>();
			m_HintController.SelectItemBot.Enable();
			m_HintController.ScrollBot.Enable();
		}

		private void OnClick()
		{
			m_HintController.SelectItemBot.Disable();
			m_HintController.ScrollBot.Disable();
			Singleton<PlayroomCustomizationModeController>.Instance.RefreshCustomizationMode(PlayroomCustomizationMode.Theme);
			m_HintController.ConfirmChanges.Enable();
		}
	}
}
