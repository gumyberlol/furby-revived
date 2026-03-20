using System;

namespace Furby
{
	[Serializable]
	public class Flair
	{
		public string Name;

		public string Path;

		public string AttachNode;

		public bool IsCompressed = true;

		public string VocalSwitch;
	}
}
