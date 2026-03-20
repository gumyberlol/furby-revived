using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyProfileSync : MonoBehaviour
	{
		public UILabel m_StatusLabel;

		public void Start()
		{
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Status);
		}

		public void Update()
		{
			m_StatusLabel.text = Singleton<FurbyDataChannel>.Instance.FurbyStatus.Personality.ToString();
		}

		public void OnDestroy()
		{
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
		}
	}
}
