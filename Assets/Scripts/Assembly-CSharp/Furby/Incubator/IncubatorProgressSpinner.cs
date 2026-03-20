using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorProgressSpinner : MonoBehaviour
	{
		public void Update()
		{
			base.transform.Rotate(0f, 0f, Time.deltaTime * -100f);
		}
	}
}
