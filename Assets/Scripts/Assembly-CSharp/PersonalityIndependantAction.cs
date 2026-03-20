using Furby;
using HutongGames.PlayMaker;
using Relentless;

[Tooltip("Instructs Furby to perform a personality-independent action")]
[ActionCategory(ActionCategory.Audio)]
public class PersonalityIndependantAction : FsmStateAction
{
	[Tooltip("Base specific action")]
	public FurbyAction m_Action;

	public override void Reset()
	{
		m_Action = FurbyAction.Purr;
	}

	public override void OnEnter()
	{
		Singleton<FurbyDataChannel>.Instance.PostAction(m_Action, null);
		Finish();
	}
}
