using Furby.Utilities.Bath;
using HutongGames.PlayMaker;
using UnityEngine;

[HutongGames.PlayMaker.Tooltip("Evaluate Bath Contents")]
[ActionCategory("Relentless.Bath")]
public class EvaluateBath : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	[RequiredField]
	public FsmFloat bathRating;

	public FsmOwnerDefault gameObject;

	public override void Reset()
	{
	}

	public override void OnEnter()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		BathContents component = ownerDefaultTarget.GetComponent<BathContents>();
		bathRating.Value = component.EvaluateBath();
		Finish();
	}
}
