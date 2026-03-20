using UnityEngine;

namespace Furby
{
	public class AutoClickFurblingInNoFurbyMode : MonoBehaviour
	{
		[SerializeField]
		private Collider m_furblingButton;

		private void Start()
		{
			if (FurbyGlobals.Player.NoFurbyOnSaveGame())
			{
				m_furblingButton.gameObject.SendMessage("OnClick");
			}
		}
	}
}
