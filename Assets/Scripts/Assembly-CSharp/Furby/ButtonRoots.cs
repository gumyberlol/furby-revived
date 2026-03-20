using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class ButtonRoots
	{
		[SerializeField]
		public GameObject m_IAPsAvailableNewSave;

		[SerializeField]
		public GameObject m_IAPsAvailableExistingSave;

		[SerializeField]
		public GameObject m_IAPsUnavailableNewSave;

		[SerializeField]
		public GameObject m_IAPsUnavailableExistingSave;
	}
}
