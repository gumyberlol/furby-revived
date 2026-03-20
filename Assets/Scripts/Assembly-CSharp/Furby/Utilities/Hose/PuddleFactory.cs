using UnityEngine;

namespace Furby.Utilities.Hose
{
	public class PuddleFactory : MonoBehaviour
	{
		public delegate void NewPuddleHandler(Puddle p);

		[SerializeField]
		public Puddle m_prefab;

		public event NewPuddleHandler PuddleCreated;

		public Puddle CreatePuddle(FurbyHoseReactions furby)
		{
			Puddle puddle = Object.Instantiate(m_prefab) as Puddle;
			puddle.gameObject.layer = base.gameObject.layer;
			if (this.PuddleCreated != null)
			{
				this.PuddleCreated(puddle);
			}
			return puddle;
		}
	}
}
