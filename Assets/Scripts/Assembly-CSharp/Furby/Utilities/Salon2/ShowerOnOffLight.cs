using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class ShowerOnOffLight : MonoBehaviour
	{
		private Shower m_shower;

		[SerializeField]
		private GameObject m_onLight;

		[SerializeField]
		private GameObject m_offLight;

		private void Start()
		{
			m_shower = base.gameObject.GetComponent<Shower>();
			m_shower.Switched += delegate
			{
				m_onLight.SetActive(m_shower.IsOn);
				m_offLight.SetActive(!m_shower.IsOn);
			};
		}
	}
}
