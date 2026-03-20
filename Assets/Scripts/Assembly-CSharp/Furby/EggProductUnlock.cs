using System;

namespace Furby
{
	[Serializable]
	public class EggProductUnlock
	{
		public string m_ScannableQRCode = string.Empty;

		public string m_VariantCode = string.Empty;

		public UnlockType m_UnlockType = UnlockType.PantryItem;

		public string m_SpriteName = string.Empty;

		public UIAtlas m_UiAtlas;

		public FurblingSpecific m_FurblingSpecific;

		public string GetQRCodeAndVariant()
		{
			return m_ScannableQRCode + m_VariantCode;
		}
	}
}
