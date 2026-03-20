using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Sets the Texture used by the GUITexture attached to a Game Object.")]
	[ActionCategory(ActionCategory.GUIElement)]
	public class SetGUITexture : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(GUITexture))]
		public FsmOwnerDefault gameObject;

		public FsmTexture texture;

		public override void Reset()
		{
			gameObject = null;
			texture = null;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (ownerDefaultTarget != null && ownerDefaultTarget.guiTexture != null)
			{
				ownerDefaultTarget.guiTexture.texture = texture.Value;
			}
			Finish();
		}
	}
}
