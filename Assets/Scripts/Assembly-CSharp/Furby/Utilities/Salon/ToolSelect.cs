using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon
{
	public class ToolSelect : MonoBehaviour
	{
		public UISprite selected;

		public PlayMakerFSM m_GameStateMachine;

		private CarouselItem currentItem;

		public CarouselItem selection;

		public GameObject ps;

		private void OnClick()
		{
			m_GameStateMachine.SendEvent("Selected");
			SendMessageUpwards("ResetTimer");
			currentItem = base.gameObject.GetComponent<CarouselItem>();
			base.gameObject.SendGameEvent(SalonGameEvent.ToolSelection);
			selected.spriteName = currentItem.m_SalonItem.Graphic;
			selected.transform.localScale = base.gameObject.transform.localScale / 400f;
			selection.m_SalonItem = currentItem.m_SalonItem;
			ps.SetActive(true);
			base.transform.parent.BroadcastMessage("ResetSprites");
			GetComponent<UISprite>().alpha = 0.5f;
		}

		public void ResetSprites()
		{
			GetComponent<UISprite>().alpha = 1f;
		}
	}
}
