using Relentless;
using UnityEngine;

public class EyeFocalPoint : RelentlessMonoBehaviour
{
	public float m_ScaleFactor = 1f;

	private bool m_AmActive = true;

	private Vector3 m_DefaultScale;

	private void Start()
	{
		m_DefaultScale = base.transform.localScale;
	}

	private void LateUpdate()
	{
		if (m_AmActive)
		{
			if (Input.GetMouseButton(0))
			{
				int width = Screen.width;
				float num = (int)Input.mousePosition.x;
				int num2 = width / 2;
				float num3 = 0f;
				num3 = ((!(num > (float)num2)) ? ((float)num2 - num) : ((float)num2 - ((float)num2 - (num - (float)num2))));
				float num4 = num3 / m_ScaleFactor * 2f;
				base.transform.localScale *= num4;
			}
			else
			{
				base.transform.localScale = m_DefaultScale;
			}
		}
	}

	public void SetActiveTrue()
	{
		m_AmActive = true;
	}

	public void SetActiveFalse()
	{
		m_AmActive = false;
	}
}
