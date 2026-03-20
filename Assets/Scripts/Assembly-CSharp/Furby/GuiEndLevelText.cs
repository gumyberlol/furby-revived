using UnityEngine;

namespace Furby
{
	public class GuiEndLevelText : MonoBehaviour
	{
		public UILabel headerLabel;

		private void Start()
		{
			headerLabel.text = "Level Complete";
		}

		public void setText(string headerText)
		{
			headerLabel.text = headerText;
		}
	}
}
