using System;

namespace Furby.Data
{
	[Serializable]
	public class FurbyTribe
	{
		public string Name;

		public FurbyType Mother;

		public FurbyType Father;
	}
}
