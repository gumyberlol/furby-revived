using UnityEngine;

namespace Relentless
{
	public class DontDestroyOnLoad : RelentlessMonoBehaviour
	{
		private void OnEnable()
		{
			Object.DontDestroyOnLoad(this);
		}
	}
}
