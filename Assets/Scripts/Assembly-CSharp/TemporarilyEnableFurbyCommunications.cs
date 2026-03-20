using Furby;
using Relentless;
using UnityEngine;

public class TemporarilyEnableFurbyCommunications : MonoBehaviour
{
	private void Start()
	{
		Singleton<FurbyDataChannel>.Instance.DisableCommunications = false;
	}

	private void OnDestroy()
	{
		Singleton<FurbyDataChannel>.Instance.DisableCommunications = FurbyGlobals.Player.NoFurbyOnSaveGame();
	}
}
