using UnityEngine;

namespace Relentless
{
	public class PrefabGroupLayoutAsset : ScriptableObject
	{
		public PrefabData[] m_prefabData = new PrefabData[0];

		public GameObject m_bakedPrefab;

		public void Unbuild(Transform root)
		{
			Transform transform = root.FindChild("ScreenRoot");
			if (transform != null)
			{
				Object.DestroyImmediate(transform.gameObject);
			}
		}

		public void Build(Transform root)
		{
			Unbuild(root);
			Transform transform = root.FindChild("ScreenRoot");
			if (transform == null)
			{
				GameObject gameObject = new GameObject("ScreenRoot");
				gameObject.transform.parent = root;
				transform = gameObject.transform;
				gameObject.layer = root.gameObject.layer;
				transform.transform.localPosition = Vector3.zero;
				transform.transform.localRotation = Quaternion.identity;
				transform.transform.localScale = Vector3.one;
			}
			if (Application.isPlaying && m_bakedPrefab != null)
			{
				GameObject gameObject2 = Object.Instantiate(m_bakedPrefab) as GameObject;
				if (gameObject2 != null)
				{
					Vector3 localPosition = gameObject2.transform.localPosition;
					Quaternion localRotation = gameObject2.transform.localRotation;
					Vector3 localScale = gameObject2.transform.localScale;
					gameObject2.transform.parent = transform;
					gameObject2.transform.localPosition = localPosition;
					gameObject2.transform.localRotation = localRotation;
					gameObject2.transform.localScale = localScale;
				}
			}
		}
	}
}
