using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class UpsellMessageDefinition
	{
		public GameObject m_PrefabToInstance;

		public string m_SpeechBubbleNamedTextKey;

		public string m_DialogHeaderNamedTextKey;
	}
}
