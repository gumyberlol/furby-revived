using Relentless;
using UnityEngine;

namespace Furby.Utilities.Help
{
	public class HelpMenuGroupTitleItem : MonoBehaviour
	{
		public NGUILocaliser m_localiser;

		public void SetupMenuItem(string localisedStringKey)
		{
			m_localiser.LocalisedStringKey = localisedStringKey;
		}
	}
}
