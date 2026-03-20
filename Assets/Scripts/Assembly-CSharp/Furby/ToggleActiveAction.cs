using UnityEngine;

namespace Furby
{
	public class ToggleActiveAction : MonoBehaviour
	{
		[SerializeField]
		protected GameObject[] m_targets;

		private void OnClick()
		{
			if (base.enabled)
			{
				GameObject[] targets = m_targets;
				foreach (GameObject gameObject in targets)
				{
					gameObject.SetActive(!gameObject.activeSelf);
				}
			}
		}
	}
}
