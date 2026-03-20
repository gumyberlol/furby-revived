using UnityEngine;

namespace Furby
{
	public class DisableIfNotInEndSequence : MonoBehaviour
	{
		private void Awake()
		{
			if (GameObject.Find("SENTINEL_ShowRevealSequence") == null)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
