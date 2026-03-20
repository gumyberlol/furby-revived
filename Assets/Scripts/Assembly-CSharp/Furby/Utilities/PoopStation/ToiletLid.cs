using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class ToiletLid : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_asset;

		private bool down = true;

		public GameObject m_LidButtonObject;

		public IEnumerator LiftUp()
		{
			Logging.Log("Toilet lid LiftUp");
			if (down)
			{
				down = false;
				base.gameObject.SendGameEvent(PoopStationEvent.LidActivated);
				Animation anim = m_asset.animation;
				anim.Play("poopStation_lidOpen");
				while (anim.isPlaying)
				{
					yield return null;
				}
				Logging.Log("LidLift Coroutine completes");
				base.gameObject.SendGameEvent(PoopStationEvent.LidLiftCompleted, null);
			}
		}

		public IEnumerator DropDown()
		{
			if (!down)
			{
				base.gameObject.SendGameEvent(PoopStationEvent.LidDownStarted, null);
				Animation anim = m_asset.animation;
				anim.Rewind();
				anim.Play("poopStation_LidShut");
				while (anim.isPlaying)
				{
					yield return null;
				}
				down = true;
				base.gameObject.SendGameEvent(PoopStationEvent.LidDownCompleted, null);
				if (m_LidButtonObject != null)
				{
					ToiletLidButton lidButton = m_LidButtonObject.GetComponent<ToiletLidButton>();
					lidButton.EnableHints = true;
				}
			}
		}

		public bool IsDown()
		{
			return down;
		}
	}
}
