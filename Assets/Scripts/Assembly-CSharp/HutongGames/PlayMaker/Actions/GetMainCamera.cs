using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Gets the camera tagged MainCamera from the scene")]
	[ActionCategory(ActionCategory.Camera)]
	public class GetMainCamera : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[RequiredField]
		public FsmGameObject storeGameObject;

		public override void Reset()
		{
			storeGameObject = null;
		}

		public override void OnEnter()
		{
			storeGameObject.Value = ((!(Camera.main != null)) ? null : Camera.main.gameObject);
			Finish();
		}
	}
}
