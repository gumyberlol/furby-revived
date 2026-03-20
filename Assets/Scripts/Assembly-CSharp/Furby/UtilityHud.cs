using UnityEngine;

namespace Furby
{
	public class UtilityHud : MonoBehaviour
	{
		public int Furbucks;

		public UILabel FurbuckCount;

		private void Start()
		{
			FurbuckCount.text = string.Empty + 50;
		}

		private void Update()
		{
		}
	}
}
