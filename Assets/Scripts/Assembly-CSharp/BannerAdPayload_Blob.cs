using UnityEngine;

public class BannerAdPayload_Blob : ScriptableObject
{
	[SerializeField]
	public BannerAdPayload m_BannerAdPayload;

	[SerializeField]
	[HideInInspector]
	public string m_DestinationFile = "C:\\BannerAdPayload.txt";
}
