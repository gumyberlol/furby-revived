using UnityEngine;

namespace Relentless
{
	public class TextureModificationSettings : ScriptableObject
	{
		public enum OperationType
		{
			Modify = 0,
			Copy = 1
		}

		public OperationType Operation;

		public string NameMatch = "*";

		public float Scale = 0.5f;

		public string Pattern = "{path}/SD/{filename}";
	}
}
