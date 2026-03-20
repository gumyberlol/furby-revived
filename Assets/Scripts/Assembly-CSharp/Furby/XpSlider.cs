using UnityEngine;

namespace Furby
{
	public class XpSlider : MonoBehaviour
	{
		public AnimationCurve m_fillCurve;

		public Renderer m_fillMeshRenderer;

		private Material m_fillMaterial;

		private void Awake()
		{
			m_fillMaterial = m_fillMeshRenderer.sharedMaterial;
		}

		public void UpdateSlider(float fillValue)
		{
			m_fillMaterial.mainTextureOffset = new Vector2(m_fillCurve.Evaluate(fillValue), 0f);
		}

		public void ShowGraphics(bool show)
		{
			foreach (Transform item in base.transform)
			{
				if ((bool)item.gameObject.GetComponent<Renderer>())
				{
					item.gameObject.GetComponent<Renderer>().enabled = show;
				}
				if ((bool)item.gameObject.GetComponent<UISprite>())
				{
					item.gameObject.GetComponent<UISprite>().enabled = show;
				}
			}
		}
	}
}
