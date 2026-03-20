using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Get the maximum amount of connections/players allowed.")]
	[ActionCategory(ActionCategory.Network)]
	public class NetworkGetMaximumConnections : FsmStateAction
	{
		[Tooltip("Get the maximum amount of connections/players allowed.")]
		[UIHint(UIHint.Variable)]
		public FsmInt result;

		public override void Reset()
		{
			result = null;
		}

		public override void OnEnter()
		{
			result.Value = Network.maxConnections;
			Finish();
		}
	}
}
