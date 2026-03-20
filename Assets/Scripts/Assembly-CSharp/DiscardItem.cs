using Furby.Utilities.Bath;
using HutongGames.PlayMaker;
using UnityEngine;

[HutongGames.PlayMaker.Tooltip("Discard Bath Item")]
[ActionCategory("Relentless.Bath")]
public class DiscardItem : FsmStateAction
{
	public FsmOwnerDefault gameObject;

	public override void Reset()
	{
	}

	public override void OnEnter()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		BathContents component = ownerDefaultTarget.GetComponent<BathContents>();
		component.DiscardItem();
		Finish();
	}
}
