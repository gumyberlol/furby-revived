using HutongGames.PlayMaker;
using UnityEngine;

[ActionCategory(ActionCategory.Material)]
[HutongGames.PlayMaker.Tooltip("Get a material at index on a gameObject and store it in a variable")]
public class GetMaterial : FsmStateAction
{
	[CheckForComponent(typeof(Renderer))]
	[RequiredField]
	public FsmOwnerDefault gameObject;

	public FsmInt materialIndex;

	[UIHint(UIHint.Variable)]
	[RequiredField]
	public FsmMaterial material;

	[HutongGames.PlayMaker.Tooltip("Get the shared material of this object. NOTE: Modifying the shared material will change the appearance of all objects using this material, and change material settings that are stored in the project too.")]
	public bool getSharedMaterial;

	public override void Reset()
	{
		gameObject = null;
		material = null;
		materialIndex = 0;
		getSharedMaterial = false;
	}

	public override void OnEnter()
	{
		DoGetMaterial();
		Finish();
	}

	private void DoGetMaterial()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (!(ownerDefaultTarget == null))
		{
			if (ownerDefaultTarget.renderer == null)
			{
				LogError("Missing Renderer!");
			}
			else if (materialIndex.Value == 0 && !getSharedMaterial)
			{
				material.Value = ownerDefaultTarget.renderer.material;
			}
			else if (materialIndex.Value == 0 && getSharedMaterial)
			{
				material.Value = ownerDefaultTarget.renderer.sharedMaterial;
			}
			else if (ownerDefaultTarget.renderer.materials.Length > materialIndex.Value && !getSharedMaterial)
			{
				Material[] materials = ownerDefaultTarget.renderer.materials;
				material.Value = materials[materialIndex.Value];
				ownerDefaultTarget.renderer.materials = materials;
			}
			else if (ownerDefaultTarget.renderer.materials.Length > materialIndex.Value && getSharedMaterial)
			{
				Material[] sharedMaterials = ownerDefaultTarget.renderer.sharedMaterials;
				material.Value = sharedMaterials[materialIndex.Value];
				ownerDefaultTarget.renderer.sharedMaterials = sharedMaterials;
			}
		}
	}
}
