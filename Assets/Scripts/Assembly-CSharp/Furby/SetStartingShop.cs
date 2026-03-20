using UnityEngine;

namespace Furby
{
	public class SetStartingShop : MonoBehaviour
	{
		public string m_startingShop;

		public static string s_startingShop;

		private void OnClick()
		{
			s_startingShop = m_startingShop;
		}
	}
}
