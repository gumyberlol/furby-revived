using Relentless;
using UnityEngine;

namespace Furby.MiniGames.HideAndSeek
{
	internal class ObjectHitReactions : Singleton<SpecialObjectReactions>
	{
		public ObjectHitVFX[] m_ObjectHitVFX;

		private int m_CurrentLevel;

		private void Start()
		{
			m_CurrentLevel = Singleton<GameDataStoreObject>.Instance.Data.HideAndSeekLevel % Singleton<HideAndSeekState>.Instance.TotalLevels;
		}

		private void TriggerVFX()
		{
			GameObject lastHitObject = Singleton<HideAndSeekUtlity>.Instance.LastHitObject;
			if ((bool)lastHitObject)
			{
				Object.Instantiate(m_ObjectHitVFX[m_CurrentLevel].m_VFXPrefab, lastHitObject.transform.position, Quaternion.identity);
			}
		}
	}
}
