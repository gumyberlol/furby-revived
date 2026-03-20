using Fabric;
using UnityEngine;

namespace Relentless
{
	[AddComponentMenu("RS System/FabricNGUIPlaySound")]
	public class FabricNGUIPlaySound : RelentlessMonoBehaviour
	{
		public enum Trigger
		{
			OnClick = 0,
			OnMouseOver = 1,
			OnMouseOut = 2,
			OnPress = 3,
			OnRelease = 4
		}

		public string m_eventName;

		public Trigger trigger;

		private void OnHover(bool isOver)
		{
			if (base.enabled && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
			{
				EventManager.Instance.PostEvent(m_eventName, EventAction.PlaySound, null, base.gameObject);
			}
		}

		private void OnPress(bool isPressed)
		{
			if (base.enabled && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
			{
				EventManager.Instance.PostEvent(m_eventName, EventAction.PlaySound, null, base.gameObject);
			}
		}

		private void OnClick()
		{
			if (base.enabled && trigger == Trigger.OnClick)
			{
				EventManager.Instance.PostEvent(m_eventName, EventAction.PlaySound, null, base.gameObject);
			}
		}
	}
}
