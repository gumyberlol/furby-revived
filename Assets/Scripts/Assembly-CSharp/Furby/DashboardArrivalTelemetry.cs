using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class DashboardArrivalTelemetry : MonoBehaviour
	{
		private static long s_saveGameCreation;

		private void Start()
		{
			GameData data = Singleton<GameDataStoreObject>.Instance.Data;
			if (s_saveGameCreation != data.TimeOfCreation)
			{
				data.CountSession();
				SendMessages(data);
				s_saveGameCreation = data.TimeOfCreation;
			}
			else
			{
				Logging.Log(string.Format("Already sent dashboard-arrival telemetry for save game starting at {0}", new DateTime(s_saveGameCreation).ToString()));
			}
		}

		private void SendMessages(GameData gd)
		{
			SendModeChoice(gd);
			SendConversion(gd);
		}

		private void SendModeChoice(GameData gd)
		{
			FurbyModeChoice furbyModeChoice = ((!gd.IsFurbyMode) ? FurbyModeChoice.DashboardArrival_NoFurby : FurbyModeChoice.DashboardArrival_Furby);
			base.gameObject.SendGameEvent(furbyModeChoice);
		}

		private void SendConversion(GameData gd)
		{
			if (gd.m_numFurbySessions == 1)
			{
				base.gameObject.SendGameEvent(FurbyModeChoice.DashboardArrival_Conversion, gd);
			}
		}
	}
}
