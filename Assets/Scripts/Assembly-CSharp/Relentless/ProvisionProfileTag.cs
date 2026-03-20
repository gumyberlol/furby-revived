using UnityEngine;

namespace Relentless
{
	public class ProvisionProfileTag : RelentlessMonoBehaviour
	{
		public Rect LabelPosition;

		public GUIStyle TextStyle;

		public bool RelativeToLeftOfScreen;

		public bool RelativeToTopOfScreen;

		private string m_text = string.Empty;

		private void Start()
		{
			if (TextStyle == null)
			{
				TextStyle = new GUIStyle();
			}
		}

		private void OnGUI()
		{
			if (!(SingletonInstance<ApplicationSettingsBehaviour>.Instance == null) && !(SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings == null))
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
				if (string.IsNullOrEmpty(m_text))
				{
					m_text = string.Format("Profile {0} {1} {2}", SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.friendlyProfileName, SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.profileType, SingletonInstance<ApplicationSettingsBehaviour>.Instance.ApplicationSettings.resolution);
				}
				GUI.Label(position, m_text, TextStyle);
			}
		}
	}
}
