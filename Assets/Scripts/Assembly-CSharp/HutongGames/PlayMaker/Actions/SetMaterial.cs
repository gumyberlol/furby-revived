using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[Tooltip("Sets the material on a game object.")]
	[ActionCategory(ActionCategory.Material)]
	public class SetMaterial : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		public FsmInt materialIndex;

		[RequiredField]
		public FsmMaterial material;

		public override void Reset()
		{
			gameObject = null;
			material = null;
			materialIndex = 0;
		}

		public override void OnEnter()
		{
			DoSetMaterial();
			Finish();
		}

		private void DoSetMaterial()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				if (ownerDefaultTarget.renderer == null)
				{
					LogError("Missing Renderer!");
				}
				else if (materialIndex.Value == 0)
				{
					ownerDefaultTarget.renderer.material = material.Value;
				}
				else if (ownerDefaultTarget.renderer.materials.Length > materialIndex.Value)
				{
					Material[] materials = ownerDefaultTarget.renderer.materials;
					materials[materialIndex.Value] = material.Value;
					ownerDefaultTarget.renderer.materials = materials;
				}
			}
		}
	}
}
