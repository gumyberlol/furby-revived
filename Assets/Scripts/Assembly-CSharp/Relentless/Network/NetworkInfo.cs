using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Relentless.Network
{
	public static class NetworkInfo
	{
		private static string m_MACAddress;

		private static string m_IPAddress;

		public static string FirstEthernetMACAddress
		{
			get
			{
				if (string.IsNullOrEmpty(m_MACAddress))
				{
					m_MACAddress = GetNetworkCardMacAddress();
				}
				return m_MACAddress;
			}
		}

		public static string FirstEthernetIPAddress
		{
			get
			{
				if (string.IsNullOrEmpty(m_IPAddress))
				{
					m_IPAddress = GetNetworkCardIPAddress();
				}
				return m_IPAddress;
			}
		}

		public static string GetNetworkCardMacAddress()
		{
			RuntimePlatform platform = Application.platform;
			if (platform == RuntimePlatform.Android)
			{
				return SingletonInstance<AndroidSpecificHelperJNI>.Instance.GetMacAddressHex();
			}
			return GetMacAddressUsingMono();
		}

		public static string GetNetworkCardIPAddress()
		{
			RuntimePlatform platform = Application.platform;
			if (platform == RuntimePlatform.Android)
			{
				return SingletonInstance<AndroidSpecificHelperJNI>.Instance.GetIpAddress();
			}
			return GetIPAddressUsingMono();
		}

		private static string GetIPAddressUsingMono()
		{
			IEnumerable<NetworkInterface> allNetworkCardDetails = GetAllNetworkCardDetails();
			if (allNetworkCardDetails.Any())
			{
				NetworkInterface[] allNetworkCardDetails2 = GetAllNetworkCardDetails(NetworkInterfaceType.Ethernet);
				if (allNetworkCardDetails2.Any())
				{
					IPInterfaceProperties iPProperties = allNetworkCardDetails2.ToList()[0].GetIPProperties();
					string empty = string.Empty;
					if (iPProperties != null)
					{
					}
					return empty;
				}
			}
			throw new InvalidOperationException("Cannot get IP Address");
		}

		private static string GetMacAddressUsingMono()
		{
			IEnumerable<NetworkInterface> allNetworkCardDetails = GetAllNetworkCardDetails();
			if (allNetworkCardDetails.Any())
			{
				NetworkInterface[] allNetworkCardDetails2 = GetAllNetworkCardDetails(NetworkInterfaceType.Ethernet);
				if (allNetworkCardDetails2.Any())
				{
					return allNetworkCardDetails2.ToList()[0].GetPhysicalAddress().ToString();
				}
			}
			throw new InvalidOperationException("Cannot get MAC Address");
		}

		private static NetworkInterface[] GetAllNetworkCardDetails(NetworkInterfaceType networkInterfaceType)
		{
			return (from nic in GetAllNetworkCardDetails()
				where nic.NetworkInterfaceType == networkInterfaceType
				select nic).ToArray();
		}

		private static IEnumerable<NetworkInterface> GetAllNetworkCardDetails()
		{
			return NetworkInterface.GetAllNetworkInterfaces();
		}

		private static void PrintNetworkCardDebugInfo()
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			Logging.Log(allNetworkInterfaces.Length + " nics");
			NetworkInterface[] array = allNetworkInterfaces;
			foreach (NetworkInterface networkInterface in array)
			{
				Logging.Log(networkInterface.Description);
				Logging.Log(string.Empty.PadLeft(networkInterface.Description.Length, '='));
				Logging.Log("  Interface type .......................... : " + networkInterface.NetworkInterfaceType);
				Logging.Log("  Physical Address ........................ : " + networkInterface.GetPhysicalAddress().ToString());
				Logging.Log("  Is receive only.......................... : " + networkInterface.IsReceiveOnly);
				Logging.Log("  Multicast................................ : " + networkInterface.SupportsMulticast);
			}
		}
	}
}
