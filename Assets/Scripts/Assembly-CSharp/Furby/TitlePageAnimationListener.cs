using Relentless;
using UnityEngine;

namespace Furby
{
	public class TitlePageAnimationListener : MonoBehaviour
	{
		private void OnStartButtonAnimFinish()
		{
			Singleton<FurbyDataChannel>.Instance.AutoConnect = true;
		}
	}
}
