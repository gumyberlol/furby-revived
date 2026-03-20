using System;
using UnityEngine;

namespace Furby
{
	public class GiftDialog : MonoBehaviour
	{
		public event Action Destroyed;

		public void OnDestroy()
		{
			if (this.Destroyed != null)
			{
				this.Destroyed();
			}
		}
	}
}
