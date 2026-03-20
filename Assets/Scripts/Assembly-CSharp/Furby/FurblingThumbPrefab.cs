using System.Collections;
using UnityEngine;

namespace Furby
{
	public class FurblingThumbPrefab : MonoBehaviour
	{
		[SerializeField]
		private UISprite m_furblingSprite;

		[SerializeField]
		private GameObject m_eyesSpriteObject;

		[SerializeField]
		private float m_minBlinkTime = 0.4f;

		[SerializeField]
		private float m_maxBlinkTime = 1.8f;

		[SerializeField]
		private float m_minBlinkDuration = 0.05f;

		[SerializeField]
		private float m_maxBlinkDuration = 0.2f;

		private IEnumerator Start()
		{
			foreach (AnimationState animState in base.animation)
			{
				animState.normalizedTime = Random.Range(0f, 1f);
			}
			while (true)
			{
				yield return new WaitForSeconds(Random.Range(m_minBlinkTime, m_maxBlinkTime));
				m_eyesSpriteObject.SetActive(false);
				yield return new WaitForSeconds(Random.Range(m_minBlinkDuration, m_maxBlinkDuration));
				m_eyesSpriteObject.SetActive(true);
				foreach (AnimationState animState2 in base.animation)
				{
					animState2.speed = Random.Range(0.9f, 1.1f);
				}
			}
		}

		public void SetFurbySprite(string spriteName)
		{
			m_furblingSprite.spriteName = spriteName;
		}

		public Vector3 GetPixelSize()
		{
			UIAtlas.Sprite sprite = m_furblingSprite.sprite;
			Vector3 vector = new Vector3(sprite.inner.width, sprite.inner.height);
			float num = vector.x / m_furblingSprite.transform.localScale.x;
			Vector3 vector2 = new Vector3(vector.x * (1f + sprite.paddingLeft + sprite.paddingRight), vector.y * (1f + sprite.paddingTop + sprite.paddingBottom));
			return vector2 / num;
		}

		public Vector3 GetOffset()
		{
			return m_furblingSprite.transform.localPosition;
		}
	}
}
