using System;
using System.Text.RegularExpressions;
using JsonFx.Json;
using JsonFx.Serialization;

namespace Relentless
{
	public static class JSONSerialiser
	{
		public static U Parse<U>(string content) where U : class
		{
			DataReaderSettingsRS dataReaderSettingsRS = new DataReaderSettingsRS();
			dataReaderSettingsRS.AllowNullValueTypes = true;
			DataReaderSettingsRS settings = dataReaderSettingsRS;
			JsonReaderRS jsonReaderRS = new JsonReaderRS(settings);
			return jsonReaderRS.Read<U>(content);
		}

		public static string AsString<U>(U obj) where U : class
		{
			return new JsonWriterRS().Write(obj);
		}

		private static string ConvertJsonDateToDateString(Match m)
		{
			return new DateTime(1970, 1, 1).AddMilliseconds(long.Parse(m.Groups[1].Value)).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
		}

		private static string ConvertDateStringToJsonDate(Match m)
		{
			DateTime dateTime = DateTime.Parse(m.Groups[0].Value).ToUniversalTime();
			return string.Format("\\/Date({0}+0800)\\/", (dateTime - DateTime.Parse("1970-01-01")).TotalMilliseconds);
		}
	}
}
