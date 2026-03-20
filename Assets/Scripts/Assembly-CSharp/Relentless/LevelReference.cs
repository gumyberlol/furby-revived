using System;
using System.IO;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class LevelReference
	{
		[SerializeField]
		private string m_path;

		public string Path
		{
			get
			{
				return m_path;
			}
			set
			{
				m_path = value;
			}
		}

		public string SceneName
		{
			get
			{
				return System.IO.Path.GetFileNameWithoutExtension(Path);
			}
		}
	}
}
