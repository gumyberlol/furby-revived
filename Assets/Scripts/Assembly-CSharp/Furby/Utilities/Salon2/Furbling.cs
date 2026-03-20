using System;
using System.Collections;
using System.Collections.Generic;
using Furby.Utilities.Blender;
using Furby.Utilities.Salon;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class Furbling : MonoBehaviour
	{
		public delegate void ScoreHandler(int score);

		private Dictionary<SalonStage, SalonItem> m_usedItems = new Dictionary<SalonStage, SalonItem>();

		[SerializeField]
		private BabyInstance m_babyInstance;

		[SerializeField]
		private Transform m_Points;

		private Dictionary<int, SalonGameEvent> m_eventsByScore;

		[SerializeField]
		private List<SalonGameEvent> m_preVerdictEvents;

		[SerializeField]
		private float m_pauseBeforePreVerdictEvent = 1f;

		public BabyInstance BabyInstance
		{
			get
			{
				return m_babyInstance;
			}
		}

		public Transform Points
		{
			get
			{
				return m_Points;
			}
		}

		public FurbyBabyReanimator Animator
		{
			get
			{
				GameObject gameObject = BabyInstance.gameObject;
				return gameObject.GetComponent<FurbyBabyReanimator>();
			}
		}

		public void SetItemForStage(SalonItem item, SalonStage stage)
		{
			m_usedItems[stage] = item;
		}

		public void Start()
		{
			m_eventsByScore = new Dictionary<int, SalonGameEvent>();
			m_eventsByScore[0] = SalonGameEvent.StyleReactionDisklike;
			m_eventsByScore[1] = SalonGameEvent.StyleReactionNeutral;
			m_eventsByScore[2] = SalonGameEvent.StyleReactionLike;
			m_eventsByScore[3] = SalonGameEvent.StyleReactionLove;
			m_eventsByScore[4] = SalonGameEvent.StyleReactionLove;
			m_eventsByScore[5] = SalonGameEvent.StyleReactionPerfect;
		}

		public IEnumerator ReactToStageCompletion(SalonStage stage)
		{
			while (!Animator.IsIdling())
			{
				yield return null;
			}
		}

		public IEnumerator ReactToGameCompletion(ScoreHandler scoreHandler)
		{
			yield return StartCoroutine(FinalInspection());
			yield return StartCoroutine(GiveVerdict(scoreHandler));
		}

		private IEnumerator FinalInspection()
		{
			yield return new WaitForSeconds(m_pauseBeforePreVerdictEvent);
			if (m_preVerdictEvents.Count != 0)
			{
				int index = UnityEngine.Random.Range(0, m_preVerdictEvents.Count);
				GameObjectExtensions.SendGameEvent(eventType: m_preVerdictEvents[index], gameObject: base.gameObject, parameters: new object[0]);
				while (!Animator.IsIdling())
				{
					yield return null;
				}
			}
		}

		private IEnumerator GiveVerdict(ScoreHandler scoreHandler)
		{
			int score = GetScore();
			if (scoreHandler != null)
			{
				scoreHandler(score);
			}
			UpdateStyleLikeOrDislike();
			base.gameObject.SendGameEvent(m_eventsByScore[score]);
			while (!Animator.IsIdling())
			{
				yield return null;
			}
		}

		private void UpdateStyleLikeOrDislike()
		{
			FurbyBaby selectedFurbyBaby = FurbyGlobals.Player.SelectedFurbyBaby;
			FurbyBabyPersonality personality = selectedFurbyBaby.Personality;
			foreach (SalonItem value in m_usedItems.Values)
			{
				int score = value.Score[personality];
				selectedFurbyBaby.OnStyleFeedback(value.Name, score);
			}
		}

		private int GetScore()
		{
			FurbyBabyPersonality personality = FurbyGlobals.Player.SelectedFurbyBaby.Personality;
			float num = 0f;
			foreach (KeyValuePair<SalonStage, SalonItem> usedItem in m_usedItems)
			{
				SalonStage key = usedItem.Key;
				SalonItem value = usedItem.Value;
				int num2 = value.Score[personality];
				Logging.Log(string.Format("{0} for stage {1} scores {2}", value.Name, key.ToString(), num2));
				num += (float)value.Score[personality];
			}
			num /= (float)m_usedItems.Count;
			int num3 = Mathf.RoundToInt(num);
			Logging.Log(string.Format("Furbling reaction is score: {0}", num3));
			return Math.Max(0, Math.Min(num3, 5));
		}
	}
}
