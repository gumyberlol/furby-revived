using Relentless;
using UnityEngine;

namespace Furby
{
	public class DashboardSideSwitchButton : MonoBehaviour
	{
		[SerializeField]
		private UICenterOnChild m_centering;

		[SerializeField]
		private Transform m_objectToCenterOn;

		private void OnClick()
		{
			Logging.Log("Switching side");
			m_centering.Recenter(m_objectToCenterOn);
		}
	}
}
