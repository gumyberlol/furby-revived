using System.Collections.Generic;
using System.Runtime.InteropServices;
using Prime31;
using UnityEngine;

public class LocalyticsBinding
{
	[DllImport("__Internal")]
	private static extern void _localyticsInit(string apiKey);

	public static void init(string apiKey)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsInit(apiKey);
		}
	}

	[DllImport("__Internal")]
	private static extern void _localyticsSetCustomDimension(int dimension, string value);

	public static void setCustomDimension(int dimension, string value)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsSetCustomDimension(dimension, value);
		}
	}

	[DllImport("__Internal")]
	private static extern void _localyticsStartSession();

	public static void startSession()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsStartSession();
		}
	}

	[DllImport("__Internal")]
	private static extern void _localyticsSetCustomerName(string name);

	public static void setCustomerName(string name)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsSetCustomerName(name);
		}
	}

	[DllImport("__Internal")]
	private static extern void _localyticsSetCustomerId(string customerId);

	public static void setCustomerId(string customerId)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsSetCustomerId(customerId);
		}
	}

	[DllImport("__Internal")]
	private static extern void _localyticsSetCustomerEmail(string email);

	public static void setCustomerEmail(string email)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsSetCustomerEmail(email);
		}
	}

	[DllImport("__Internal")]
	private static extern void _localyticsSetValueForIdentifier(string identifier, string value);

	public static void setValueForIdentifier(string identifier, string value)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsSetValueForIdentifier(identifier, value);
		}
	}

	[DllImport("__Internal")]
	private static extern void _localyticsTagEvent(string eventName);

	public static void tagEvent(string eventName)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsTagEvent(eventName);
		}
	}

	[DllImport("__Internal")]
	private static extern void _localyticsTagEventWithAttributes(string eventName, string attributes);

	public static void tagEventWithAttributes(string eventName, Dictionary<string, object> attributes)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsTagEventWithAttributes(eventName, attributes.toJson());
		}
	}

	[DllImport("__Internal")]
	private static extern void _localyticsTagEventWithAttributesAndReportAttributes(string eventName, string attributes, string reportAttributes, double customerValueIncrease);

	public static void tagEventWithAttributesAndReportAttributes(string eventName, Dictionary<string, object> attributes, Dictionary<string, object> reportAttributes, double customerValueIncrease)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsTagEventWithAttributesAndReportAttributes(eventName, attributes.toJson(), reportAttributes.toJson(), customerValueIncrease);
		}
	}

	[DllImport("__Internal")]
	private static extern void _localyticsTagScreen(string screenName);

	public static void tagScreen(string screenName)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_localyticsTagScreen(screenName);
		}
	}
}
