using Furby;
using HutongGames.PlayMaker;
using Relentless;

[Tooltip("Toggles the enforcement of 'App-Mode' on a Furby")]
[ActionCategory(ActionCategory.Audio)]
public class FurbyApplicationMode : FsmStateAction
{
	[Tooltip("Enable App-Mode")]
	public bool m_Active;

	public override void Reset()
	{
		m_Active = false;
	}

	public override void OnEnter()
	{
		Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(m_Active);
		Finish();
	}
}
