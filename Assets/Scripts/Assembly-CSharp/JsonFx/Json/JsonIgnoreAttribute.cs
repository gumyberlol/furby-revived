using System;

namespace JsonFx.Json
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public sealed class JsonIgnoreAttribute : Attribute
	{
	}
}
