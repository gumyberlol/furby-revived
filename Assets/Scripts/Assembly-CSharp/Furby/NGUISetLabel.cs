using HutongGames.PlayMaker;
using UnityEngine;

namespace Furby
{
	[ActionCategory(ActionCategory.GameObject)]
	[HutongGames.PlayMaker.Tooltip("Sets the label on an NGUI Label object")]
	public class NGUISetLabel : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("The NGUI object that has label")]
		public UILabel m_LabelObject;

		[HutongGames.PlayMaker.Tooltip("The time to set the label for")]
		public float m_LabelTime = 1f;

		[HutongGames.PlayMaker.Tooltip("The Label to set during and after time")]
		public string m_LabelDuringTime;

		[HutongGames.PlayMaker.Tooltip("The Label to set during and after time")]
		public string m_LabelAfterTime;

		private float m_CurrentTime;

		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			m_LabelObject.text = m_LabelDuringTime;
			m_CurrentTime = 0f;
		}

		public override void OnUpdate()
		{
			if (InternalUpdate(Time.deltaTime))
			{
				Finish();
			}
		}

		private bool InternalUpdate(float deltaTime)
		{
			bool result = false;
			m_CurrentTime += deltaTime;
			if (m_CurrentTime >= m_LabelTime)
			{
				m_LabelObject.text = m_LabelAfterTime;
				result = true;
			}
			return result;
		}
	}
}
