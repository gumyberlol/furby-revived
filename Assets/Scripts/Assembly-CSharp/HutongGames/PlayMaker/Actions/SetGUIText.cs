using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUIElement)]
	[Tooltip("Sets the Text used by the GUIText Component attached to a Game Object.")]
	public class SetGUIText : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(GUIText))]
		public FsmOwnerDefault gameObject;

		public FsmString text;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			text = string.Empty;
		}

		public override void OnEnter()
		{
			DoSetGUIText();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetGUIText();
		}

		private void DoSetGUIText()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null && ownerDefaultTarget.guiText != null)
			{
				ownerDefaultTarget.guiText.text = text.Value;
			}
		}
	}
}
