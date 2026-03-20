using HutongGames.PlayMaker;
using UnityEngine;

[HutongGames.PlayMaker.Tooltip("Get a texture from a material on a GameObject")]
[ActionCategory(ActionCategory.Material)]
public class GetMaterialTexture : FsmStateAction
{
	[CheckForComponent(typeof(Renderer))]
	[RequiredField]
	public FsmOwnerDefault gameObject;

	public FsmInt materialIndex;

	[UIHint(UIHint.NamedTexture)]
	public FsmString namedTexture;

	[UIHint(UIHint.Variable)]
	[RequiredField]
	public FsmTexture storedTexture;

	public bool getFromSharedMaterial;

	public override void Reset()
	{
		gameObject = null;
		materialIndex = 0;
		namedTexture = "_MainTex";
		storedTexture = null;
		getFromSharedMaterial = false;
	}

	public override void OnEnter()
	{
		DoGetMaterialTexture();
		Finish();
	}

	private void DoGetMaterialTexture()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(gameObject);
		if (ownerDefaultTarget == null)
		{
			return;
		}
		if (ownerDefaultTarget.renderer == null)
		{
			LogError("Missing Renderer!");
			return;
		}
		string text = namedTexture.Value;
		if (text == string.Empty)
		{
			text = "_MainTex";
		}
		if (materialIndex.Value == 0 && !getFromSharedMaterial)
		{
			storedTexture.Value = ownerDefaultTarget.renderer.material.GetTexture(text);
		}
		else if (materialIndex.Value == 0 && getFromSharedMaterial)
		{
			storedTexture.Value = ownerDefaultTarget.renderer.sharedMaterial.GetTexture(text);
		}
		else if (ownerDefaultTarget.renderer.materials.Length > materialIndex.Value && !getFromSharedMaterial)
		{
			Material[] materials = ownerDefaultTarget.renderer.materials;
			storedTexture.Value = ownerDefaultTarget.renderer.materials[materialIndex.Value].GetTexture(text);
			ownerDefaultTarget.renderer.materials = materials;
		}
		else if (ownerDefaultTarget.renderer.materials.Length > materialIndex.Value && getFromSharedMaterial)
		{
			Material[] sharedMaterials = ownerDefaultTarget.renderer.sharedMaterials;
			storedTexture.Value = ownerDefaultTarget.renderer.sharedMaterials[materialIndex.Value].GetTexture(text);
			ownerDefaultTarget.renderer.materials = sharedMaterials;
		}
	}
}
