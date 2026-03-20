using UnityEngine;

public class ScrollUVs : MonoBehaviour
{
	public float m_scrollSpeedX = 1f;

	public float m_scrollSpeedY = 1f;

	public Renderer m_renderer;

	private Material m_material;

	private Vector2 m_offset = Vector2.one;

	public bool m_enableTexture01Scroll = true;

	public string m_textureName01 = "_TextureA";

	public bool m_enableTexture02Scroll = true;

	public string m_textureName02 = "_TextureB";

	private void Start()
	{
		m_material = m_renderer.material;
	}

	private void Update()
	{
		m_offset.x -= m_scrollSpeedX * Time.deltaTime;
		m_offset.y -= m_scrollSpeedY * Time.deltaTime;
		if (m_offset.x < -10f)
		{
			m_offset.x += 10f;
		}
		if (m_offset.y < -10f)
		{
			m_offset.y += 10f;
		}
		if (m_enableTexture01Scroll)
		{
			m_material.SetTextureOffset(m_textureName01, m_offset);
		}
		if (m_enableTexture02Scroll)
		{
			m_material.SetTextureOffset(m_textureName02, m_offset);
		}
	}
}
