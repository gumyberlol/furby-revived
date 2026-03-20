using UnityEngine;

namespace Relentless
{
	public class DoDestroyOnLoad : RelentlessMonoBehaviour
	{
		private void OnLevelWasLoaded()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
