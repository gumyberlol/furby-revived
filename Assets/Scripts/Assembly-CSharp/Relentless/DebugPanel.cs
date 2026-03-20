using UnityEngine;

namespace Relentless
{
	public class DebugPanel : MonoBehaviour
	{
		[SerializeField]
		private GUISkin m_skin;

		public static bool StartSection(string sectionName)
		{
			return false;
		}

		public static void EndSection()
		{
		}
	}
}
