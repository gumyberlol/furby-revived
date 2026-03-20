using Relentless;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class ChangeTransformForGlobalCrystal : MonoBehaviour
{
	public Vector3 m_localPosition;

	public Vector3 m_localScale;

	private void Awake()
	{
		if (Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked)
		{
			base.transform.localPosition = m_localPosition;
			base.transform.localScale = m_localScale;
		}
	}
}
