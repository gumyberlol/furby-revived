using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class SprayButton : MonoBehaviour
	{
		[SerializeField]
		private Spray m_spray;

		public void OnPress(bool down)
		{
			if (down)
			{
				m_spray.On();
			}
			else
			{
				m_spray.Off();
			}
		}
	}
}
