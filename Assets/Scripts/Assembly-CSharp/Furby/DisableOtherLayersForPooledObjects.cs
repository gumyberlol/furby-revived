using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Furby
{
	public class DisableOtherLayersForPooledObjects : MonoBehaviour
	{
		private Dictionary<UICamera, bool> m_previousEnabledStates;

		public void DisableOtherLayers()
		{
			m_previousEnabledStates = new Dictionary<UICamera, bool>();
			UICamera[] array = (from camera in Camera.allCameras
				let uicamera = camera.GetComponent<UICamera>()
				where uicamera != null
				select uicamera).ToArray();
			int layer = base.gameObject.layer;
			UICamera[] array2 = array;
			foreach (UICamera uICamera in array2)
			{
				m_previousEnabledStates[uICamera] = uICamera.enabled;
				int layer2 = uICamera.gameObject.layer;
				bool flag = layer2 == layer;
				if (uICamera.gameObject.layer != 31)
				{
					uICamera.enabled = flag;
				}
			}
		}

		public void EnableOtherLayers()
		{
			if (m_previousEnabledStates == null)
			{
				return;
			}
			Dictionary<UICamera, bool>.KeyCollection keys = m_previousEnabledStates.Keys;
			foreach (UICamera item in keys)
			{
				if (item.gameObject.layer != 31)
				{
					item.enabled = m_previousEnabledStates[item];
				}
			}
			m_previousEnabledStates = null;
		}
	}
}
