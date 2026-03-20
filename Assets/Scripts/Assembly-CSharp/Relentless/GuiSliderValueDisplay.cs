using UnityEngine;

namespace Relentless
{
	public class GuiSliderValueDisplay : MonoBehaviour
	{
		public UISlider m_SourceSlider;

		public UILabel m_DestinationLabel;

		public float m_ValueScale = 100f;

		public bool m_IntegralDisplay = true;

		public void OnSliderChange(float sliderValue)
		{
			float f = sliderValue * m_ValueScale;
			if (m_IntegralDisplay)
			{
				f = Mathf.RoundToInt(f);
			}
			m_DestinationLabel.text = f.ToString();
		}
	}
}
