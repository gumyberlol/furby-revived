using Fabric;
using UnityEngine;

namespace Relentless
{
	[AddComponentMenu("NGUI/Interaction/PlayAudioWhenTriggered")]
	public class PlayAudioWhenTriggered : RelentlessMonoBehaviour
	{
		public string AudioEventName;

		public void OnTriggerEnter(Collider other)
		{
			EventManager.Instance.PostEvent(AudioEventName);
		}
	}
}
