using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets a Game Object's material randomly from an array of Materials.")]
	public class SetRandomMaterial : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		public FsmInt materialIndex;

		public FsmMaterial[] materials;

		public override void Reset()
		{
			gameObject = null;
			materialIndex = 0;
			materials = new FsmMaterial[3];
		}

		public override void OnEnter()
		{
			DoSetRandomMaterial();
			Finish();
		}

		private void DoSetRandomMaterial()
		{
			if (materials == null || materials.Length == 0)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
			if (!(ownerDefaultTarget == null))
			{
				if (ownerDefaultTarget.renderer == null)
				{
					LogError("SetMaterial: Missing Renderer!");
				}
				else if (materialIndex.Value == 0)
				{
					ownerDefaultTarget.renderer.material = materials[Random.Range(0, materials.Length)].Value;
				}
				else if (ownerDefaultTarget.renderer.materials.Length > materialIndex.Value)
				{
					Material[] array = ownerDefaultTarget.renderer.materials;
					array[materialIndex.Value] = materials[Random.Range(0, materials.Length)].Value;
					ownerDefaultTarget.renderer.materials = array;
				}
			}
		}
	}
}
