using System;
using UnityEngine;

namespace RsFramework
{
	[Serializable]
	public class NoteInstance
	{
		[SerializeField]
		public string m_CreationTime;

		[SerializeField]
		public string m_LastUpdateTime;

		[SerializeField]
		public string m_Body;

		[SerializeField]
		public string m_LastEditUsername;
	}
}
