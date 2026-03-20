using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FriendsBookTutorialBoxConnector : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_gridRoot;

		private IEnumerator Start()
		{
			yield return null;
			GameObject secondFriend = m_gridRoot.transform.GetChild(1).gameObject;
			TutorialInstructionBox tutorial = GetComponent<TutorialInstructionBox>();
			tutorial.TargetCollider = secondFriend.collider;
			StartCoroutine(tutorial.WaitAndEnable());
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogShow));
			yield return null;
			GameEventRouter.SendEvent(SharedGuiEvents.DialogAccept);
		}
	}
}
