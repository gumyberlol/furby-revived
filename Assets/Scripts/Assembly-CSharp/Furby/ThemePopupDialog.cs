using UnityEngine;

namespace Furby
{
	public class ThemePopupDialog : MonoBehaviour
	{
		[SerializeField]
		private ThemeButton m_buttonPrefab;

		[SerializeField]
		private UIGrid m_grid;

		public void Start()
		{
			AddButtonForPeriod(null);
			ThemePeriodChooser themePeriodChooser = FurbyGlobals.ThemePeriodChooser;
			foreach (ThemePeriod period in themePeriodChooser.Periods)
			{
				AddButtonForPeriod(period);
			}
			m_grid.Reposition();
		}

		private void AddButtonForPeriod(ThemePeriod p)
		{
			ThemeButton themeButton = Object.Instantiate(m_buttonPrefab) as ThemeButton;
			themeButton.transform.parent = m_grid.transform;
			themeButton.transform.localScale = Vector3.one;
			themeButton.transform.localPosition = Vector3.zero;
			themeButton.SetupFrom(p);
			themeButton.Clicked += delegate
			{
				Object.Destroy(base.gameObject);
			};
		}
	}
}
