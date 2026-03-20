using UnityEngine;

namespace Furby.Utilities.Help
{
	public class HelpPrevNextButton : MonoBehaviour
	{
		[SerializeField]
		private HelpPanel m_panel;

		[SerializeField]
		private int m_delta = 1;

		public void Start()
		{
			m_panel.PageMoved += delegate(int page)
			{
				int num = page + m_delta;
				bool flag = num < 0 || num >= m_panel.NumPages;
				Collider component = GetComponent<Collider>();
				if ((bool)component)
				{
					component.enabled = !flag;
				}
			};
		}

		public void OnClick()
		{
			int currentPage = m_panel.CurrentPage;
			currentPage += m_delta;
			m_panel.GoToPage(currentPage);
		}
	}
}
