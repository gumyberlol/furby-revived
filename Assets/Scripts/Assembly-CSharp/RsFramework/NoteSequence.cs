using System;
using UnityEngine;

namespace RsFramework
{
	[Serializable]
	public class NoteSequence
	{
		[SerializeField]
		public NoteInstance[] m_Notes;
	}
}
