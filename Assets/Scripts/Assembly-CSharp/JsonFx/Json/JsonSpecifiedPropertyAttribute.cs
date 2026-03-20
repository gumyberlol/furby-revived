using System;

namespace JsonFx.Json
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class JsonSpecifiedPropertyAttribute : Attribute
	{
		private string specifiedProperty;

		public string SpecifiedProperty
		{
			get
			{
				return specifiedProperty;
			}
			set
			{
				specifiedProperty = value;
			}
		}

		public JsonSpecifiedPropertyAttribute()
		{
		}

		public JsonSpecifiedPropertyAttribute(string propertyName)
		{
			specifiedProperty = propertyName;
		}
	}
}
