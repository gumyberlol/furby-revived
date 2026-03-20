using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets a named texture in a game object's material.")]
	public class SetMaterialTexture : FsmStateAction
	{
		[Tooltip("The GameObject that the material is applied to.")]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		[Tooltip("A named parameter in the shader.")]
		[UIHint(UIHint.NamedTexture)]
		public FsmString namedTexture;

		public FsmTexture texture;

		public override void Reset()
		{
			gameObject = null;
			materialIndex = 0;
			material = null;
			namedTexture = "_MainTex";
			texture = null;
		}

		public override void OnEnter()
		{
			DoSetMaterialTexture();
			Finish();
		}

		private void DoSetMaterialTexture()
		{
			string text = namedTexture.Value;
			if (text == string.Empty)
			{
				text = "_MainTex";
			}
			if (material.Value != null)
			{
				material.Value.SetTexture(text, texture.Value);
				return;
			}
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
					ownerDefaultTarget.renderer.material.SetTexture(text, texture.Value);
				}
				else if (ownerDefaultTarget.renderer.materials.Length > materialIndex.Value)
				{
					Material[] materials = ownerDefaultTarget.renderer.materials;
					materials[materialIndex.Value].SetTexture(text, texture.Value);
					ownerDefaultTarget.renderer.materials = materials;
				}
			}
		}
	}
}
