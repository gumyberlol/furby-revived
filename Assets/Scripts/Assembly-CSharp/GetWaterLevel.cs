using Furby.Utilities.Bath;
using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory("Relentless.Bath")]
[HutongGames.PlayMaker.Tooltip("Get Water to Variable")]
public class GetWaterLevel : FsmStateAction
{
	[UIHint(UIHint.Variable)]
	[RequiredField]
	public FsmFloat floatVariable;

	public FsmOwnerDefault gameObject;

	private bool everyFrame;

	public override void Reset()
	{
		everyFrame = true;
	}

	public override void OnEnter()
	{
		DoWaterUpdate();
		if (!everyFrame)
		{
			Finish();
		}
	}

	public override void OnUpdate()
	{
		DoWaterUpdate();
	}

	private void DoWaterUpdate()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		BathContents component = ownerDefaultTarget.GetComponent<BathContents>();
		if ((bool)component)
		{
			float waterAmount = component.waterAmount;
			if (waterAmount == 0f)
			{
				floatVariable.Value = 0f;
				return;
			}
			float num = 0.2f;
			float num2 = 1f - num;
			floatVariable.Value = num + num2 * component.waterAmount;
		}
	}
}
