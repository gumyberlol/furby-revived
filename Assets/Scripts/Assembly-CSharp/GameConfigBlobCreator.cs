using Furby;
using UnityEngine;

public class GameConfigBlobCreator : ScriptableObject
{
	[SerializeField]
	public GameConfigBlob m_GameConfigBlob;

	[HideInInspector]
	[SerializeField]
	public string m_DestinationFile = "C:\\GameConfig.txt";
}
