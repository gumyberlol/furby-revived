using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class ActivateObjectReaction : GameEventReaction
	{
		public GameObject TargetObject;

		public bool Activation = true;

		public override void React(GameObject gameObject, params object[] paramlist)
		{
			TargetObject.SetActive(Activation);
		}
	}
}
