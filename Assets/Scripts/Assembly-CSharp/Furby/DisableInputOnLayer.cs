using Relentless;
using UnityEngine;

namespace Furby
{
	public class DisableInputOnLayer : RelentlessMonoBehaviour
	{
		[SerializeField]
		private LayerMask m_layersToDisable;

		private Object[] m_uiCameraList;

		public void ExcludeLayer(int layer)
		{
			m_layersToDisable = (int)m_layersToDisable & ~(1 << layer);
		}

		private void OnEnable()
		{
			m_uiCameraList = Object.FindObjectsOfType(typeof(UICamera));
			DisableInputLayers();
		}

		public void ForceDisableInput()
		{
			DisableInputLayers();
		}

		private void DisableInputLayers()
		{
			Object[] uiCameraList = m_uiCameraList;
			for (int i = 0; i < uiCameraList.Length; i++)
			{
				UICamera uICamera = (UICamera)uiCameraList[i];
				if (((int)m_layersToDisable & (1 << uICamera.gameObject.layer)) != 0 && uICamera.gameObject.layer != 31)
				{
					uICamera.enabled = false;
				}
			}
		}

		private void OnDisable()
		{
			Object[] uiCameraList = m_uiCameraList;
			for (int i = 0; i < uiCameraList.Length; i++)
			{
				UICamera uICamera = (UICamera)uiCameraList[i];
				if (((int)m_layersToDisable & (1 << uICamera.gameObject.layer)) != 0 && uICamera.gameObject.layer != 31)
				{
					uICamera.enabled = true;
				}
			}
		}
	}
}
