using Relentless;
using UnityEngine;

[ExecuteInEditMode]
public class SetHueFromColour : MonoBehaviour
{
	[SerializeField]
	private Color m_colour;

	private Color m_setColour;

	public Color Colour
	{
		get
		{
			return m_colour;
		}
		set
		{
			m_colour = value;
		}
	}

	private void SetMaterialHue()
	{
		Vector3 vector = HSV.RGBtoHSV(m_colour);
		base.gameObject.renderer.sharedMaterial.SetFloat("_Hue", vector.x);
		m_setColour = m_colour;
	}

	private void Start()
	{
		SetMaterialHue();
	}

	private void Update()
	{
		if (m_colour != m_setColour)
		{
			SetMaterialHue();
		}
	}
}
