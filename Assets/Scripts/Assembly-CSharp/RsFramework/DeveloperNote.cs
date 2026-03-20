using System;
using UnityEngine;

namespace RsFramework
{
	[Serializable]
	public class DeveloperNote : MonoBehaviour
	{
		[SerializeField]
		public NoteSequence m_NoteSequence = new NoteSequence();
	}
}
