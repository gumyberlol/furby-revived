using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Store the current send rate for all NetworkViews")]
	[ActionCategory(ActionCategory.Network)]
	public class NetworkGetSendRate : FsmStateAction
	{
		[Tooltip("Store the current send rate for NetworkViews")]
		[UIHint(UIHint.Variable)]
		[RequiredField]
		public FsmFloat sendRate;

		public override void Reset()
		{
			sendRate = null;
		}

		public override void OnEnter()
		{
			DoGetSendRate();
			Finish();
		}

		private void DoGetSendRate()
		{
			sendRate.Value = Network.sendRate;
		}
	}
}
