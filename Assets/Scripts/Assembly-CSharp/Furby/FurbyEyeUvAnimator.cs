using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyEyeUvAnimator : RelentlessMonoBehaviour
	{
		public Transform m_controllerTransformEyeL;

		public Transform m_controllerTransformEyeR;

		public Renderer m_rendererEyeL;

		public Renderer m_rendererEyeR;

		private bool m_ready;

		private void Start()
		{
			InitialiseEyeUvs();
		}

		private void Update()
		{
			if (m_ready)
			{
				Vector2 offset = new Vector2((0f - m_controllerTransformEyeL.localPosition.x) * 100f, m_controllerTransformEyeL.localPosition.z * 100f);
				Vector2 offset2 = new Vector2((0f - m_controllerTransformEyeR.localPosition.x) * 100f, m_controllerTransformEyeR.localPosition.z * 100f);
				m_rendererEyeL.sharedMaterial.SetTextureOffset("_Texture", offset);
				m_rendererEyeR.sharedMaterial.SetTextureOffset("_Texture", offset2);
			}
		}

		public void AssignEyeControllers(Transform controllerTransformEyeL, GameObject modelEyeL, Transform controllerTransformEyeR, GameObject modelEyeR)
		{
			m_controllerTransformEyeL = controllerTransformEyeL;
			m_controllerTransformEyeR = controllerTransformEyeR;
			m_rendererEyeL = modelEyeL.renderer;
			m_rendererEyeR = modelEyeR.renderer;
			InitialiseEyeUvs();
		}

		private void InitialiseEyeUvs()
		{
			if (m_controllerTransformEyeL != null && m_rendererEyeL != null && m_controllerTransformEyeR != null && m_rendererEyeR != null)
			{
				m_rendererEyeL.sharedMaterial.SetTextureOffset("_Texture", Vector2.zero);
				m_rendererEyeR.sharedMaterial.SetTextureOffset("_Texture", Vector2.zero);
				m_ready = true;
			}
			else
			{
				m_ready = false;
				Logging.LogWarning("Not all Eye UV objects are assigned!");
			}
		}
	}
}
