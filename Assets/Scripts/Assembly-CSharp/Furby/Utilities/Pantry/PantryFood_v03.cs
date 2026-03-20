using Relentless;
using UnityEngine;

namespace Furby.Utilities.Pantry
{
	public class PantryFood_v03 : MonoBehaviour
	{
		public PantryFoodData FoodData;

		private UISprite m_Sprite;

		private void Awake()
		{
			m_Sprite = GetComponentInChildren<UISprite>();
			if (!(m_Sprite == null))
			{
				Vector3 zero = Vector3.zero;
				zero.y = -64f;
				m_Sprite.transform.localPosition += zero;
			}
		}

		private void OnClick()
		{
			GameEventRouter.SendEvent(PantryEvent.FoodItemClicked, base.gameObject, this);
		}

		private void OnDrag(Vector2 delta)
		{
			GameObject gameObject = GameObject.Find("PantryFlow");
			gameObject.GetComponent<PantryFlow>().HaveScrolledThisSession = true;
			gameObject.GetComponent<PantryFlow>().EnableAppropriateHints();
		}

		public void Fade(float duration, bool becomeVisible)
		{
			if (!(m_Sprite == null))
			{
				float alpha = ((!becomeVisible) ? 0f : 1f);
				TweenAlpha.Begin(m_Sprite.gameObject, duration, alpha);
			}
		}
	}
}
