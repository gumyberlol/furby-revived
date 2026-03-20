using Fabric;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class SetInitialVolume : MonoBehaviour
	{
		private void Start()
		{
			EventManager.Instance.PostEvent("all", EventAction.SetVolume, Singleton<GameDataStoreObject>.Instance.GlobalData.GetPreSaveGameLoadAudioVolume());
		}
	}
}
