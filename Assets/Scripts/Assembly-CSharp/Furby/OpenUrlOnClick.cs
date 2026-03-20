using Relentless;
using UnityEngine;

namespace Furby
{
	public class OpenUrlOnClick : MonoBehaviour
	{
		[SerializeField]
		[NamedText]
		private string m_url;

		private void OnClick()
		{
			Application.OpenURL(Singleton<Localisation>.Instance.GetText(m_url));
		}
	}
}
