using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomStartup_FromHood : RelentlessMonoBehaviour
	{
		public FurbyBaby m_furbyBaby;

		public FurbyBaby FurbyBaby
		{
			get
			{
				return m_furbyBaby;
			}
			set
			{
				m_furbyBaby = value;
			}
		}

		private void OnLevelWasLoaded()
		{
			if (!Application.loadedLevelName.ToLower().Contains("empty"))
			{
				DefineTheFurbyBabyInstance();
				Object.Destroy(base.transform.gameObject);
			}
		}

		private void DefineTheFurbyBabyInstance()
		{
			GameObject gameObject = GameObject.Find("FurbyBaby");
			if ((bool)gameObject)
			{
				BabyInstance component = gameObject.GetComponent<BabyInstance>();
				if ((bool)component)
				{
					component.SetTargetFurbyBaby(m_furbyBaby);
				}
			}
		}
	}
}
