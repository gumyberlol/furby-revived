using System;

namespace JsonFx.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class JsonNameAttribute : Attribute
	{
		private string name;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public JsonNameAttribute()
		{
		}

		public JsonNameAttribute(string name)
		{
			Name = name;
		}
	}
}
