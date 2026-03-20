using UnityEngine;

namespace Furby
{
	public class SlotHandler : MonoBehaviour
	{
		public string m_QRCode;

		[SerializeField]
		private string m_VariantCode = string.Empty;

		public SlotPopulator m_Pop;

		public string QRCode
		{
			get
			{
				return m_QRCode;
			}
			set
			{
				m_QRCode = value;
			}
		}

		public string VariantCode
		{
			get
			{
				return m_VariantCode;
			}
			set
			{
				m_VariantCode = value;
			}
		}

		public void OnClick()
		{
			StartCoroutine(m_Pop.PresentationFeedback(this));
		}
	}
}
