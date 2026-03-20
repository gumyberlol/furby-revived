using System;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class FlushButton : MonoBehaviour
	{
		[SerializeField]
		private Toilet m_toilet;

		public void Start()
		{
			if (m_toilet == null)
			{
				throw new ApplicationException(string.Format("{0} does not have its toilet set.", base.gameObject.name));
			}
		}

		public void OnClick()
		{
			m_toilet.GetBowl().Flush();
		}
	}
}
