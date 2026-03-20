using UnityEngine;

namespace Relentless
{
	public class TmpClone : RelentlessMonoBehaviour
	{
		private void OnDestroy()
		{
			base.gameObject.transform.parent = null;
			Object.Destroy(base.gameObject);
		}
	}
}
