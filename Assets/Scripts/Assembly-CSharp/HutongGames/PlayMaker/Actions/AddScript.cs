using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Adds a Script to a Game Object. Use this to change the behaviour of objects on the fly. Optionally remove the Script on exiting the state.")]
	[ActionCategory(ActionCategory.ScriptControl)]
	public class AddScript : FsmStateAction
	{
		[Tooltip("The Game Object to add the script to.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		[Tooltip("The Script to add to the Game Object.")]
		[UIHint(UIHint.ScriptComponent)]
		public FsmString script;

		[Tooltip("Remove the script from the Game Object when this State is exited.")]
		public FsmBool removeOnExit;

		private Component addedComponent;

		public override void Reset()
		{
			gameObject = null;
			script = null;
		}

		public override void OnEnter()
		{
			DoAddComponent((gameObject.OwnerOption != OwnerDefaultOption.UseOwner) ? gameObject.GameObject.Value : base.Owner);
			Finish();
		}

		public override void OnExit()
		{
			if (removeOnExit.Value && addedComponent != null)
			{
				Object.Destroy(addedComponent);
			}
		}

		private void DoAddComponent(GameObject go)
		{
			addedComponent = go.AddComponent(script.Value);
			if (addedComponent == null)
			{
				LogError("Can't add script: " + script.Value);
			}
		}
	}
}
