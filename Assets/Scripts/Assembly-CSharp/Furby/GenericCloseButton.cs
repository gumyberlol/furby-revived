using System;
using UnityEngine;

namespace Furby
{
	public class GenericCloseButton : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_target;

		public event Action Clicked;

		public void OnClick()
		{
			if (this.Clicked != null)
			{
				this.Clicked();
			}
			if (m_target != null)
			{
				UnityEngine.Object.Destroy(m_target);
			}
		}
	}
}
