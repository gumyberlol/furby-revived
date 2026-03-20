using UnityEngine;

namespace Relentless
{
	public class GhostPrefab
	{
		public static void RenderGhostPrefab(GameObject prefab, Matrix4x4 matrix)
		{
			MeshFilter[] componentsInChildren = prefab.GetComponentsInChildren<MeshFilter>(true);
			MeshRenderer[] componentsInChildren2 = prefab.GetComponentsInChildren<MeshRenderer>(true);
			Matrix4x4 matrix4x = matrix * prefab.transform.worldToLocalMatrix;
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				for (int j = 0; j < componentsInChildren2[i].sharedMaterial.passCount; j++)
				{
					Graphics.DrawMesh(componentsInChildren[i].sharedMesh, matrix4x * componentsInChildren[i].transform.localToWorldMatrix, componentsInChildren2[i].sharedMaterial, 0, null, i % componentsInChildren[i].sharedMesh.subMeshCount);
				}
			}
			SkinnedMeshRenderer[] componentsInChildren3 = prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			SkinnedMeshRenderer[] array = componentsInChildren3;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				int num = 0;
				Material[] sharedMaterials = skinnedMeshRenderer.sharedMaterials;
				foreach (Material material in sharedMaterials)
				{
					for (int m = 0; m < material.passCount; m++)
					{
						Graphics.DrawMesh(skinnedMeshRenderer.sharedMesh, matrix4x * skinnedMeshRenderer.transform.localToWorldMatrix, material, 0, null, num);
					}
					num++;
				}
			}
		}
	}
}
