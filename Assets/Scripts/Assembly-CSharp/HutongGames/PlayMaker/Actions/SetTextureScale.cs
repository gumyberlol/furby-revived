using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Sets the Scale of a named texture in a Game Object's Material. Useful for special effects.")]
	[ActionCategory(ActionCategory.Material)]
	public class SetTextureScale : FsmStateAction
	{
		[CheckForComponent(typeof(Renderer))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		public FsmInt materialIndex;

		[UIHint(UIHint.NamedColor)]
		public FsmString namedTexture;

		[RequiredField]
		public FsmFloat scaleX;

		[RequiredField]
		public FsmFloat scaleY;

		public bool everyFrame;

		public override void Reset()
		{
			gameObject = null;
			materialIndex = 0;
			namedTexture = "_MainTex";
			scaleX = 1f;
			scaleY = 1f;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoSetTextureScale();
			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoSetTextureScale();
		}

		private void DoSetTextureScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				if (ownerDefaultTarget.renderer == null)
				{
					LogError("Missing Renderer!");
				}
				else if (ownerDefaultTarget.renderer.material == null)
				{
					LogError("Missing Material!");
				}
				else if (materialIndex.Value == 0)
				{
					ownerDefaultTarget.renderer.material.SetTextureScale(namedTexture.Value, new Vector2(scaleX.Value, scaleY.Value));
				}
				else if (ownerDefaultTarget.renderer.materials.Length > materialIndex.Value)
				{
					Material[] materials = ownerDefaultTarget.renderer.materials;
					materials[materialIndex.Value].SetTextureScale(namedTexture.Value, new Vector2(scaleX.Value, scaleY.Value));
					ownerDefaultTarget.renderer.materials = materials;
				}
			}
		}
	}
}
