using UnityEngine;

namespace Relentless
{
	public class BuildTag : RelentlessMonoBehaviour
	{
		private string m_displayString;

		public NamedTextTable m_buildInformation;

		public string m_tagFormat;

		public string[] m_textValues;

		public Rect m_labelPosition;

		public GUIStyle TextStyle;

		private GameEventSubscription m_debugSubs;

		[SerializeField]
		private bool m_showOnlyInDebug;
	}
}
