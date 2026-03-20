using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class Customization_ChooseRug : RelentlessMonoBehaviour
	{
		public PlayroomHintController m_HintController;

		private void Start()
		{
			GameObject gameObject = GameObject.Find("HintController");
			m_HintController = gameObject.GetComponent<PlayroomHintController>();
			m_HintController.SelectItemTop.Enable();
			m_HintController.ScrollTop.Enable();
		}

		private void OnClick()
		{
			m_HintController.SelectItemTop.Disable();
			m_HintController.ScrollTop.Disable();
			Singleton<PlayroomCustomizationModeController>.Instance.RefreshCustomizationMode(PlayroomCustomizationMode.Rug);
			m_HintController.ConfirmChanges.Enable();
		}
	}
}
