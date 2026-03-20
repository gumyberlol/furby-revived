using System;
using System.Collections.Generic;
using System.Text;
using Relentless.Network.Security;
using UnityEngine;

namespace Relentless.Network.Analytics
{
	public class TelemetryParams
	{
		private Dictionary<string, string> m_parameters;

		public Dictionary<string, string> Params
		{
			get
			{
				return m_parameters;
			}
		}

		public TelemetryParams()
		{
			m_parameters = new Dictionary<string, string>();
			AddIdAndTimeStampToDictionary();
		}

		public TelemetryParams(Dictionary<string, string> dict)
		{
			m_parameters = new Dictionary<string, string>(dict);
			AddIdAndTimeStampToDictionary();
		}

		public TelemetryParams(string key, string value)
		{
			m_parameters = new Dictionary<string, string>();
			Add(key, value);
			AddIdAndTimeStampToDictionary();
		}

		public void Add(string key, string value)
		{
			m_parameters.Add(key, value);
		}

		private void AddIdAndTimeStampToDictionary()
		{
			if (!m_parameters.ContainsKey("DeviceId"))
			{
				Add("DeviceId", DeviceIdManager.DeviceId);
			}
			if (!m_parameters.ContainsKey("SessionId"))
			{
				Add("SessionId", TelemetryManager.SessionId);
			}
			if (!m_parameters.ContainsKey("TimeStamp"))
			{
				Add("TimeStamp", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff zzz"));
			}
			if (!m_parameters.ContainsKey("GameTime"))
			{
				Add("GameTime", default(DateTime).AddSeconds(Time.time).ToString("HH:mm:ss.fff"));
			}
		}

		public string DictionaryToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string key in m_parameters.Keys)
			{
				stringBuilder.Append(string.Format("{0} = {1}, ", key, m_parameters[key]));
			}
			return stringBuilder.ToString();
		}
	}
}
