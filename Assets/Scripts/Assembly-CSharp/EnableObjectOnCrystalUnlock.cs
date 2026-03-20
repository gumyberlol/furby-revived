using Relentless;
using UnityEngine;

public class EnableObjectOnCrystalUnlock : MonoBehaviour
{
	public bool m_enable;

	private void Awake()
	{
		if (Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked)
		{
			base.gameObject.SetActive(m_enable);
		}
	}
}
