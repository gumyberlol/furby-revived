using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon
{
	public class SalonStage : ScriptableObject
	{
		[SerializeField]
		private string m_name;

		[SerializeField]
		private SalonItemList m_items;

		[SerializeField]
		private List<SalonGameEvent> m_initialContactEvents = new List<SalonGameEvent>();

		[SerializeField]
		private List<SalonGameEvent> m_rubStartEvents = new List<SalonGameEvent>();

		[SerializeField]
		private List<SalonGameEvent> m_rubStopEvents = new List<SalonGameEvent>();

		[SerializeField]
		private List<SalonGameEvent> m_rubEvents = new List<SalonGameEvent>();

		[SerializeField]
		private List<SalonGameEvent> m_progressionEvents = new List<SalonGameEvent>();

		[SerializeField]
		private List<SalonGameEvent> m_rubCompleteEvents = new List<SalonGameEvent>();

		public string Name
		{
			get
			{
				string text = Singleton<Localisation>.Instance.GetText(m_name);
				if (string.IsNullOrEmpty(text))
				{
					throw new ApplicationException(string.Format("Failed to localise key \"{0}\" for Salon Stage \"{1}\"", m_name, base.name));
				}
				return text;
			}
		}

		public SalonItemList Items
		{
			get
			{
				return m_items;
			}
		}

		public void OnInitialContact(GameObject caller)
		{
			DoList(caller, m_initialContactEvents);
		}

		public void OnRubStart(GameObject caller)
		{
			DoList(caller, m_rubStartEvents);
		}

		public void OnRubStop(GameObject caller)
		{
			DoList(caller, m_rubStopEvents);
		}

		public void OnPointRubbed(GameObject caller)
		{
			DoList(caller, m_rubEvents);
		}

		public void OnProgression(GameObject caller, float progression)
		{
			DoList(caller, m_progressionEvents, progression);
		}

		public void OnCompletion(GameObject caller)
		{
			DoList(caller, m_rubCompleteEvents);
		}

		private static void DoList(GameObject caller, List<SalonGameEvent> list, params object[] parameters)
		{
			foreach (SalonGameEvent item in list)
			{
				caller.SendGameEvent(item, parameters);
			}
		}
	}
}
