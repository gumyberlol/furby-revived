using Furby;
using HutongGames.PlayMaker;
using Relentless;

[ActionCategory("Furby")]
[Tooltip("Awards Xp to the player's Furby")]
public class FurbyAwardXp : FsmStateAction
{
	public XpAwardEvent xpEvent;

	public override void OnEnter()
	{
		GameEventRouter.SendEvent(xpEvent);
		Finish();
	}
}
