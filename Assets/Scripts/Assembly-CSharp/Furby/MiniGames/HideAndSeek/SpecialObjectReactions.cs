using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.MiniGames.HideAndSeek
{
	public class SpecialObjectReactions : Singleton<SpecialObjectReactions>
	{
		public SpecialReactionsData[] m_SpecialReactionsData;

		public SpecialObjectHitVFX[] m_ObjectHitVFX;

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

		private IEnumerator ChainReaction()
		{
			int chainPops = m_SpecialReactionsData[m_CurrentLevel].m_ChainReactionData.m_ChainReactionPops;
			GameObject chainVFX = m_SpecialReactionsData[m_CurrentLevel].m_ChainReactionData.m_ChainReactionVFX;
			while (chainPops-- > 0)
			{
				GameObject objectToPop = Singleton<HideAndSeekUtlity>.Instance.GetRandomHideObject(Singleton<SpecialObjectManager>.Instance.CurrentSpecialObject);
				if ((bool)objectToPop)
				{
					if ((bool)chainVFX)
					{
						Object.Instantiate(chainVFX, objectToPop.transform.position, Quaternion.identity);
					}
					SingletonInstance<PrefabPool>.Instance.ReturnToPool(objectToPop);
				}
				yield return new WaitForSeconds(m_SpecialReactionsData[m_CurrentLevel].m_ChainReactionData.m_ChainReactionDelay);
			}
		}

		public void AddTries()
		{
			Singleton<HideAndSeekState>.Instance.IncrementTries(m_SpecialReactionsData[m_CurrentLevel].m_SpecialTriesAdd);
		}

		public void SubtractTries()
		{
			Singleton<HideAndSeekState>.Instance.IncrementTries(-m_SpecialReactionsData[m_CurrentLevel].m_SpecialTriesSubtract);
		}
	}
}
