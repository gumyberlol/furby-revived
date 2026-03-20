using UnityEngine;

namespace Relentless
{
	public class CommandlineBuildTag : RelentlessMonoBehaviour
	{
		public Rect LabelPosition;

		public GUIStyle TextStyle;

		public bool RelativeToLeftOfScreen;

		public bool RelativeToTopOfScreen;

		private void Start()
		{
			if (TextStyle == null)
			{
				TextStyle = new GUIStyle();
			}
		}

		private void OnGUI()
		{
			if (!(SingletonInstance<ApplicationSettingsBehaviour>.Instance == null) && !(SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings == null) && !SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.isCommandlineBuild)
			{
				float height = LabelPosition.height;
				float width = LabelPosition.width;
				Rect position = new Rect
				{
					width = width,
					height = height
				};
				if (RelativeToLeftOfScreen)
				{
					position.x = LabelPosition.x;
				}
				else
				{
					position.x = (float)Mathf.Max(2, Screen.width) - LabelPosition.x;
				}
				if (RelativeToTopOfScreen)
				{
					position.y = LabelPosition.y;
				}
				else
				{
					position.y = (float)Mathf.Max(2, Screen.height) - LabelPosition.y;
				}
				GUI.Label(position, "Not a commandline build", TextStyle);
			}
		}
	}
}
