using System;
using System.Collections;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class BabyEndMinigameFlow : MonoBehaviour
	{
		[Serializable]
		public class ScoreSection
		{
			public GameObject RootGameObject;

			public GameObject ScoreRoot;

			public GameObject RatingRoot;

			public GameObject FurbucksRoot;

			public UILabel ScoreLabel;

			public UILabel FurbucksLabel;

			public UISprite[] RatingStars;

			public Animation[] RatingStarsEffectsAnimation;

			public float PauseAfterShowingLabel = 0.5f;

			public float PauseBeforeUpdatingScore = 0.5f;

			public float TimeToSpendUpdatingScore = 3f;

			public float PauseBeforeUpdatingStars = 0.15f;

			public float PauseBetweenUpdatingStars = 3f;

			public float PauseBeforeUpdatingFurbucks = 0.25f;

			public float TimeToSpendUpdatingFurbucks = 1f;
		}

		[Serializable]
		public class ProgressSection
		{
			public GameObject RootGameObject;

			public GameObject Sparkle;

			public float PauseBeforeUpdatingXp = 1f;

			public float TimeToSpendUpdatingXp = 1f;
		}

		[Serializable]
		public class GraduationSection
		{
			public GameObject RootGameObject;

			public GameObject Congratulations;

			public float CongratsPause = 2f;

			public GameObject Buttons;
		}

		[Serializable]
		public class VerdictSection
		{
			public GameObject RootGameObject;
		}

		public delegate void SectionShowHandler(GameObject section);

		[SerializeField]
		private EndGameScoring m_scoringData;

		[SerializeField]
		public bool m_showScoring = true;

		[SerializeField]
		private ScoreSection m_scoreSection;

		[SerializeField]
		private ProgressSection m_progressSection;

		[SerializeField]
		private XpSlider m_xpSlider;

		[SerializeField]
		private ParticleSystem m_furbucksParticles;

		[SerializeField]
		public int m_furbucksParticlesEmitRate = 12;

		[SerializeField]
		private GameObject m_playAgainRoot;

		[SerializeField]
		private GraduationSection m_graduationSection;

		[SerializeField]
		private VerdictSection m_verdictSection;

		private GameEventSubscription m_eventSubs;

		private int m_score;

		private int m_rating;

		private int m_furbucks;

		private int m_xp;

		private int m_startingFurbucks;

		public GameObject PlayAgainRoot
		{
			get
			{
				return m_playAgainRoot;
			}
		}

		public event SectionShowHandler ShowingPlayAgain;

		private void Awake()
		{
			m_eventSubs = new GameEventSubscription(typeof(BabyEndMinigameEvent), OnEvents);
			InitialiseSections();
		}

		private void OnDisable()
		{
			m_eventSubs.Dispose();
		}

		private void Start()
		{
			m_startingFurbucks = FurbyGlobals.Wallet.Balance;
			m_xpSlider.gameObject.SetActive(true);
			m_xpSlider.ShowGraphics(false);
		}

		private void OnEvents(Enum evtType, GameObject originator, params object[] parameters)
		{
			switch ((BabyEndMinigameEvent)(object)evtType)
			{
			case BabyEndMinigameEvent.ShowDialog:
				StartCoroutine(DoEndGame());
				break;
			case BabyEndMinigameEvent.SetScore:
				m_score = (int)parameters[0];
				break;
			case BabyEndMinigameEvent.HideDialog:
			{
				UIPanel component = GetComponent<UIPanel>();
				component.enabled = false;
				break;
			}
			case BabyEndMinigameEvent.ReCheckStartingFurbucks:
				m_startingFurbucks = FurbyGlobals.Wallet.Balance;
				break;
			}
		}

		private void InitialiseSections()
		{
			m_scoreSection.RootGameObject.SetActive(false);
			m_playAgainRoot.SetActive(false);
			m_graduationSection.RootGameObject.SetActive(false);
			m_progressSection.Sparkle.SetActive(false);
			if (m_verdictSection.RootGameObject != null)
			{
				m_verdictSection.RootGameObject.SetActive(true);
			}
		}

		private IEnumerator DoEndGame()
		{
			UIPanel panel = GetComponent<UIPanel>();
			panel.enabled = true;
			DisableOtherLayers disabler = GetComponent<DisableOtherLayers>();
			disabler.enabled = true;
			bool firstFurby = FurbyGlobals.BabyRepositoryHelpers.Neighbourhood.Count() == 0;
			for (int i = 0; i < m_scoringData.ScoreStarThresholds.Length && m_scoringData.ScoreStarThresholds[i] <= m_score; i++)
			{
				m_rating = i + 1;
				if (firstFurby)
				{
					m_xp = m_scoringData.XpPerThresholdFirstFurby[i];
				}
				else
				{
					m_xp = m_scoringData.XpPerThreshold[i];
				}
			}
			yield return null;
			m_furbucks = FurbyGlobals.Wallet.Balance - m_startingFurbucks;
			yield return StartCoroutine(DisplayScoreSection());
			FurbyGlobals.Player.SelectedFurbyBaby.EarnedXP = m_xp;
			if (FurbyGlobals.Player.SelectedFurbyBaby.UpdateXP())
			{
				yield return StartCoroutine(DisplayGraduationSection());
			}
			else
			{
				yield return StartCoroutine(DisplayPlayAgainSection());
			}
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(BabyEndMinigameEvent.PlayAgain, BabyEndMinigameEvent.ReturnToPlayroom));
			panel.enabled = false;
			disabler.enabled = false;
			m_xpSlider.ShowGraphics(false);
			if (waiter.ReturnedEvent.Equals(BabyEndMinigameEvent.ReturnToPlayroom))
			{
				yield return new WaitForSeconds(0.5f);
			}
			GameEventRouter.SendEvent(BabyEndMinigameEvent.DialogFinished);
		}

		private void SetScoreAmounts(float ratio)
		{
			int num = (int)((float)m_score * ratio);
			m_scoreSection.ScoreLabel.text = num.ToString();
		}

		private void SetRatingAmount(int rating)
		{
			m_scoreSection.RatingStarsEffectsAnimation[rating].Play();
			m_scoreSection.RatingStars[rating].enabled = true;
			if (rating == 0)
			{
				GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowStar1);
			}
			if (rating == 1)
			{
				GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowStar2);
			}
			if (rating == 2)
			{
				GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowStar3);
			}
		}

		private void SetXpAmount(float ratio)
		{
			int num = FurbyGlobals.Player.SelectedFurbyBaby.XP + (int)(ratio * (float)m_xp);
			m_xpSlider.UpdateSlider((float)num / (float)FurbyGlobals.BabyLibrary.GetBabyFurby(FurbyGlobals.Player.SelectedFurbyBaby.Type).XpToLevelUp);
		}

		private void SetFurbucksAmount(float ratio)
		{
			int num = (int)(ratio * (float)m_furbucks) + m_startingFurbucks;
			m_scoreSection.FurbucksLabel.text = num.ToString();
		}

		private IEnumerator DisplayScoreSection()
		{
			m_scoreSection.RootGameObject.SetActive(true);
			m_scoreSection.ScoreRoot.SetActive(false);
			m_scoreSection.RatingRoot.SetActive(false);
			m_scoreSection.FurbucksRoot.SetActive(false);
			UISprite[] ratingStars = m_scoreSection.RatingStars;
			foreach (UISprite sprite in ratingStars)
			{
				sprite.enabled = false;
			}
			if (m_showScoring)
			{
				yield return new WaitForSeconds(m_scoreSection.PauseBeforeUpdatingScore);
				m_scoreSection.ScoreRoot.SetActive(true);
				SetScoreAmounts(0f);
				GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowScoreLabel);
				yield return new WaitForSeconds(m_scoreSection.PauseAfterShowingLabel);
				if (m_score == 0)
				{
					GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowScoreNoIncrease);
				}
				else
				{
					GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowScoreIncrease);
				}
				for (float timePassed = 0f; timePassed <= m_scoreSection.TimeToSpendUpdatingScore; timePassed += Time.deltaTime)
				{
					float ratio = timePassed / m_scoreSection.TimeToSpendUpdatingScore;
					SetScoreAmounts(ratio);
					yield return null;
				}
				SetScoreAmounts(1f);
				yield return new WaitForSeconds(m_scoreSection.PauseBeforeUpdatingStars);
				m_scoreSection.RatingRoot.SetActive(true);
				GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowStarLabel);
				yield return new WaitForSeconds(m_scoreSection.PauseAfterShowingLabel);
				for (int rating = 0; rating < m_rating; rating++)
				{
					SetRatingAmount(rating);
					yield return new WaitForSeconds(m_scoreSection.PauseBetweenUpdatingStars);
					yield return null;
				}
			}
			yield return new WaitForSeconds(m_scoreSection.PauseBeforeUpdatingFurbucks);
			m_scoreSection.FurbucksRoot.SetActive(true);
			SetFurbucksAmount(0f);
			GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowMoneyLabel);
			yield return new WaitForSeconds(m_scoreSection.PauseAfterShowingLabel);
			GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowMoneyIncrease);
			m_furbucksParticles.emissionRate = m_furbucksParticlesEmitRate;
			for (float timePassed2 = 0f; timePassed2 <= m_scoreSection.TimeToSpendUpdatingFurbucks; timePassed2 += Time.deltaTime)
			{
				float ratio2 = timePassed2 / m_scoreSection.TimeToSpendUpdatingFurbucks;
				SetFurbucksAmount(ratio2);
				yield return null;
			}
			SetFurbucksAmount(1f);
			m_furbucksParticles.emissionRate = 0f;
			yield return new WaitForSeconds(m_progressSection.PauseBeforeUpdatingXp);
			m_xpSlider.ShowGraphics(true);
			SetXpAmount(0f);
			GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowXpLabel);
			yield return new WaitForSeconds(m_scoreSection.PauseAfterShowingLabel);
			if (FurbyGlobals.Player.SelectedFurbyBaby.Progress != FurbyBabyProgresss.N)
			{
				GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowXpIncrease);
				m_xpSlider.UpdateSlider((float)FurbyGlobals.Player.SelectedFurbyBaby.XP / (float)FurbyGlobals.BabyLibrary.GetBabyFurby(FurbyGlobals.Player.SelectedFurbyBaby.Type).XpToLevelUp);
				for (float timePassed3 = 0f; timePassed3 <= m_progressSection.TimeToSpendUpdatingXp; timePassed3 += Time.deltaTime)
				{
					float ratio3 = timePassed3 / m_progressSection.TimeToSpendUpdatingXp;
					SetXpAmount(ratio3);
					yield return null;
				}
			}
			SetXpAmount(1f);
		}

		private IEnumerator DisplayPlayAgainSection()
		{
			if (this.ShowingPlayAgain != null)
			{
				this.ShowingPlayAgain(m_playAgainRoot);
			}
			m_playAgainRoot.SetActive(true);
			yield return null;
		}

		private IEnumerator DisplayGraduationSection()
		{
			m_graduationSection.RootGameObject.SetActive(true);
			base.animation.Rewind();
			m_graduationSection.Buttons.SetActive(false);
			m_progressSection.Sparkle.SetActive(true);
			m_progressSection.Sparkle.animation.Play();
			GameEventRouter.SendEvent(BabyEndMinigameEvent.ShowXpComplete);
			yield return new WaitForSeconds(m_graduationSection.CongratsPause);
			m_scoreSection.RootGameObject.SetActive(false);
			if (m_verdictSection.RootGameObject != null)
			{
				m_verdictSection.RootGameObject.SetActive(false);
			}
			m_graduationSection.Buttons.SetActive(true);
			base.animation.Play();
		}
	}
}
