using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class ScrubPoint : MonoBehaviour
	{
		public delegate void ScrubHandler();

		public ScrubHandler Scrubbed;

		public RubbyPoint RubbyPoint
		{
			get
			{
				return base.gameObject.GetComponent<RubbyPoint>();
			}
		}

		public void Start()
		{
			RubbyPoint.LevelChanged += delegate(float f)
			{
				if (f >= 1f)
				{
					if (Scrubbed != null)
					{
						Scrubbed();
					}
					Object.Destroy(base.gameObject);
				}
			};
		}
	}
}
