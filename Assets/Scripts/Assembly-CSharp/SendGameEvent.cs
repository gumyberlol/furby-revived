using System;
using HutongGames.PlayMaker;
using Relentless;

[ActionCategory("Relentless")]
[Tooltip("Posts an event through the Game Event Router")]
public class SendGameEvent : FsmStateAction
{
	public FsmString m_eventType;

	public FsmString m_eventName;

	public FsmGameObject m_gameObject;

	public override void OnEnter()
	{
		Type type = Type.GetType(m_eventType.Value);
		Enum eventType = (Enum)Enum.Parse(type, m_eventName.Value);
		GameEventRouter.SendEvent(eventType, m_gameObject.Value);
		Finish();
	}
}
