using System;
using Furby.Utilities.Salon;
using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class SalonStageTitle : MonoBehaviour
	{
		[SerializeField]
		private Salon2Flow m_gameFlow;

		private UILabel m_label;

		public void Start()
		{
			m_label = GetComponent<UILabel>();
			if (m_label == null)
			{
				throw new ApplicationException(string.Format("Failed to find a UILabel on {0}", base.gameObject.name));
			}
			m_gameFlow.StageStarted += delegate(SalonStage s)
			{
				m_label.text = s.Name;
			};
		}
	}
}
