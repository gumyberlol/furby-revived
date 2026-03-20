using Relentless;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class ChangeTextureForGlobalCrystal : MonoBehaviour
{
	public Texture m_texture;

	private void Awake()
	{
		if (Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked)
		{
			UITexture component = GetComponent<UITexture>();
			component.mainTexture = m_texture;
		}
	}
}
