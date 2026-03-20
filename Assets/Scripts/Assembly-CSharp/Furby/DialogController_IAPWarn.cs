using System.Collections.Generic;
using Relentless;
using Relentless.Core.DesignPatterns.ProviderManager;
using UnityEngine;

namespace Furby
{
	public class DialogController_IAPWarn : GameEventConsumer<SharedGuiEvents>
	{
		[SerializeField]
		protected GameObject m_DialogPanel;

		[SerializeField]
		public List<PlatformIAPWarning> m_PlatformSpecificIAPWarning = new List<PlatformIAPWarning>();

		[SerializeField]
		public UILabel m_TargetLabel;

		public void Awake()
		{
			SetLocalizedText();
		}

		private void SetLocalizedText()
		{
			PlatformIAPWarning platformSpecificWarning = GetPlatformSpecificWarning();
			string text = Singleton<Localisation>.Instance.GetText(platformSpecificWarning.m_LocalisedStringKey);
			m_TargetLabel.text = text;
		}

		private PlatformIAPWarning GetPlatformSpecificWarning()
		{
			BillingPlatform billingPlatform = GetBillingPlatform();
			return m_PlatformSpecificIAPWarning.Find((PlatformIAPWarning iapWarn) => iapWarn.m_BillingPlatform == billingPlatform);
		}

		private BillingPlatform GetBillingPlatform()
		{
			BillingPlatform result = BillingPlatform.GooglePlay;
			switch (Application.platform)
			{
			case RuntimePlatform.Android:
			{
				ProviderDevices device = ProviderDevicesHelper.GetDevice();
				result = ((device == ProviderDevices.Kindle) ? BillingPlatform.AmazonAppstore : BillingPlatform.GooglePlay);
				break;
			}
			case RuntimePlatform.IPhonePlayer:
				result = BillingPlatform.AppleAppStore;
				break;
			}
			return result;
		}
	}
}
