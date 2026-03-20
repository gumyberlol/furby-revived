using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class ApplicationSettings : ScriptableObject
	{
		[Serializable]
		public class iOSSettings
		{
			[SerializeField]
			public string applicationDisplayName;

			[SerializeField]
			public string applicationURL;

			[SerializeField]
			public string iOsSKU = string.Empty;
		}

		[SerializeField]
		public string productName;

		[SerializeField]
		public string companyName;

		[SerializeField]
		public ProfileFriendlyName friendlyProfileName;

		[SerializeField]
		public ProfileType profileType;

		[SerializeField]
		public Resolution resolution;

		[SerializeField]
		public string bundleVersion;

		[SerializeField]
		public string bundleIdentifier;

		[SerializeField]
		public iOSSettings iOS;

		[SerializeField]
		public bool isCommandlineBuild;

		[SerializeField]
		public string GooglePlayPublicKey;

		[SerializeField]
		public AndroidBillingPlatform AndroidBillingPlatform;

		[SerializeField]
		public bool StoreSandboxEnabled;

		public void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(this);
		}
	}
}
