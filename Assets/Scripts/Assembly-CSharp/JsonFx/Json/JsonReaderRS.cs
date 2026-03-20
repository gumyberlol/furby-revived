using System;
using System.Collections.Generic;
using JsonFx.Model;
using JsonFx.Serialization;

namespace JsonFx.Json
{
	public class JsonReaderRS : ModelReaderRS
	{
		private readonly string[] ContentTypes;

		public override IEnumerable<string> ContentType
		{
			get
			{
				if (ContentTypes != null)
				{
					string[] contentTypes = ContentTypes;
					for (int i = 0; i < contentTypes.Length; i++)
					{
						yield return contentTypes[i];
					}
				}
				else
				{
					yield return "application/json";
					yield return "text/json";
					yield return "text/x-json";
				}
			}
		}

		public JsonReaderRS()
			: this(new DataReaderSettingsRS())
		{
		}

		public JsonReaderRS(DataReaderSettingsRS settings)
			: base((settings == null) ? new DataReaderSettingsRS() : settings)
		{
		}

		public JsonReaderRS(DataReaderSettingsRS settings, params string[] contentTypes)
			: base((settings == null) ? new DataReaderSettingsRS() : settings)
		{
			if (contentTypes == null)
			{
				throw new NullReferenceException("contentTypes");
			}
			ContentTypes = new string[contentTypes.Length];
			Array.Copy(contentTypes, ContentTypes, contentTypes.Length);
		}

		protected override ITextTokenizer<ModelTokenType> GetTokenizer()
		{
			return new JsonReader.JsonTokenizer();
		}
	}
}
