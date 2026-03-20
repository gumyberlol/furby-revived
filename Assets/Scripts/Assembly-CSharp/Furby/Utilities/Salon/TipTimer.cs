using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon
{
	public class TipTimer : MonoBehaviour
	{
		public GameObject TipScreen;

		private float TipTime;

		public UILabel TipText;

		public float TipWait = 30f;

		public int tip = 1;

		private void Update()
		{
			if (!TipScreen.activeInHierarchy)
			{
				TipTime += Time.deltaTime;
			}
			if (TipTime > TipWait)
			{
				if (tip == 1)
				{
					TipText.text = "Tap to select an item from the carousel, tap again to confirm";
				}
				else if (tip == 2)
				{
					TipText.text = "Drag item over Furby until he is fully covered";
				}
				TipTime = 0f;
			}
		}

		private void ResetTimer()
		{
			TipTime = 0f;
		}

		private void ToolConfirmed()
		{
			tip = 2;
			Logging.Log("Tip Changed to 2");
		}

		public void NextCarousel()
		{
			tip = 1;
			Logging.Log("Tip Changed to 1");
		}
	}
}
