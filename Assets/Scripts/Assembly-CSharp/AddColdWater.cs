using Furby.Utilities.Bath;
using HutongGames.PlayMaker;
using UnityEngine;

[HutongGames.PlayMaker.Tooltip("Add Cold Water")]
[ActionCategory("Relentless.Bath")]
public class AddColdWater : FsmStateAction
{
	[HutongGames.PlayMaker.Tooltip("Cold Water Amount")]
	public float m_amount;

	public FsmOwnerDefault gameObject;

	private bool everyFrame;

	private bool perSecond;

	public override void Reset()
	{
		m_amount = 0f;
		everyFrame = true;
		perSecond = true;
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
			if (!perSecond)
			{
				component.AddColdWater(m_amount);
			}
			else
			{
				component.AddColdWater(m_amount * Time.deltaTime);
			}
		}
	}
}
