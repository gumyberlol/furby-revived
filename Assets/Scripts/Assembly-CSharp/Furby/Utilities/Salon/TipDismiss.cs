using UnityEngine;

namespace Furby.Utilities.Salon
{
	public class TipDismiss : MonoBehaviour
	{
		public GameObject TipScreen;

		private void OnClick()
		{
			TipScreen.SetActive(false);
		}
	}
}
