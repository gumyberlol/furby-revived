using UnityEngine;

namespace Furby
{
	public class FurballDisablePauseWhileActive : MonoBehaviour
	{
		private void OnEnable()
		{
			GameObject gameObject = GameObject.Find("PauseButtonPanel");
			if (gameObject != null)
			{
				PauseButtonPanel component = gameObject.GetComponent<PauseButtonPanel>();
				component.SetEnabled(false);
			}
		}

		private void OnDisable()
		{
			GameObject gameObject = GameObject.Find("PauseButtonPanel");
			if (gameObject != null)
			{
				PauseButtonPanel component = gameObject.GetComponent<PauseButtonPanel>();
				component.SetEnabled(true);
			}
		}
	}
}
