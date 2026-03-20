using HutongGames.PlayMaker;

namespace Relentless
{
	[ActionCategory("Fabric")]
	[Tooltip("Unloads any loaded fabric prefabs")]
	public class UnloadFabricPrefabs : FsmStateAction
	{
		public override void OnEnter()
		{
			FabricPrefabManager.Instance.UnloadAll();
			Finish();
		}
	}
}
