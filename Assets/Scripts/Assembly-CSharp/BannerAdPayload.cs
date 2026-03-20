using System;
using UnityEngine;

[Serializable]
public class BannerAdPayload
{
	[SerializeField]
	public int m_Width;

	[SerializeField]
	public int m_Height;

	[SerializeField]
	public Texture2D m_Texture;
}
