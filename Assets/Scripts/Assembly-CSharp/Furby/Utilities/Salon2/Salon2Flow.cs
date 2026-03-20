using System;
using System.Collections;
using System.Collections.Generic;
using Furby.Utilities.Salon;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class Salon2Flow : MonoBehaviour
	{
		public delegate void StageEvent(SalonStage stage);

		private delegate void ToolConfirmedHandler(Tool t);

		private delegate void ScrubHandler(GameObject obj);

		private delegate void AllScrubbedHandler();

		private delegate void ScrubSystemCustomizer(ScrubSystem scrubs);

		private delegate void ToolCustomizer(Tool tool);

		[SerializeField]
		private ScrubSystem m_scrubsPrefab;

		[SerializeField]
		private Transform m_carouselSpawnPoint;

		[SerializeField]
		private Transform m_toolSpawnPoint;

		[SerializeField]
		private Tool m_toolPrefab;

		[SerializeField]
		private Carousel m_carouselPrefab;

		[SerializeField]
		private SalonStage m_washStage;

		[SerializeField]
		private SalonStage m_scrubStage;

		[SerializeField]
		private SalonStage m_styleStage;

		[SerializeField]
		private Furbling m_furbling;

		[SerializeField]
		private Shower m_shower;

		[SerializeField]
		private StoolAnimEventReceiver m_stool;

		[SerializeField]
		private ShowerCurtain m_showerCurtain;

		[SerializeField]
		private Salon2Hints m_hints;

		public event StageEvent StageStarted;

		private Carousel MakeCarousel(SalonStage stage)
		{
			Carousel carousel = UnityEngine.Object.Instantiate(m_carouselPrefab) as Carousel;
			carousel.transform.parent = m_carouselSpawnPoint;
			carousel.transform.localScale = Vector3.one;
			carousel.transform.localPosition = Vector3.zero;
			carousel.transform.localRotation = Quaternion.identity;
			carousel.SetupForStage(stage);
			m_hints.ConnectCarouselListeners(carousel);
			return carousel;
		}

		private Tool MakeTool(SalonItem item, SalonStage stage)
		{
			Tool tool = UnityEngine.Object.Instantiate(m_toolPrefab) as Tool;
			tool.transform.parent = m_toolSpawnPoint;
			tool.transform.localPosition = Vector3.zero;
			tool.transform.localScale = Vector3.one;
			tool.SetupFrom(item, stage);
			return tool;
		}

		private IEnumerator ChooseTool(SalonStage stage, ToolConfirmedHandler onConfirm)
		{
			Carousel c = MakeCarousel(stage);
			bool confirmed = false;
			Tool tool = null;
			SalonStage stage2 = default(SalonStage);
			c.ItemSelected += delegate(SalonItem item)
			{
				c.gameObject.SendGameEvent(SalonGameEvent.ToolSelection);
				if (tool != null)
				{
					UnityEngine.Object.Destroy(tool.gameObject);
				}
				tool = MakeTool(item, stage2);
				Tool.Handler clickHandler = null;
				clickHandler = delegate
				{
					confirmed = true;
					tool.Clicked -= clickHandler;
				};
				tool.Clicked += clickHandler;
			};
			while (!confirmed)
			{
				yield return null;
			}
			tool.gameObject.SendGameEvent(SalonGameEvent.ToolConfirmation);
			c.GoAway();
			m_furbling.SetItemForStage(tool.SalonItem, tool.Stage);
			onConfirm(tool);
		}

		private ScrubSystem MakeScrubPoints(Tool tool, ScrubHandler onScrub)
		{
			ScrubSystem scrubSystem = UnityEngine.Object.Instantiate(m_scrubsPrefab) as ScrubSystem;
			if (scrubSystem == null)
			{
				throw new ApplicationException("Failed to instantiate ScrubSystem");
			}
			BabyInstance babyInstance = m_furbling.BabyInstance;
			GameObject gameObject = babyInstance.gameObject;
			scrubSystem.transform.position = gameObject.transform.position;
			ScrubSystem.PointGeneratedHandler callerFunc = delegate(GameObject babyPoint, ScrubPoint sp)
			{
				sp.Scrubbed = (ScrubPoint.ScrubHandler)Delegate.Combine(sp.Scrubbed, (ScrubPoint.ScrubHandler)delegate
				{
					onScrub(babyPoint);
				});
			};
			foreach (Transform point in m_furbling.Points)
			{
				GameObject o = point.gameObject;
				scrubSystem.GenerateScrubPoint(o, callerFunc);
			}
			scrubSystem.Scrubber = tool.GetCollider();
			scrubSystem.Background.OnMovementStart += delegate
			{
				tool.OnScrubbingStart();
			};
			scrubSystem.Background.OnMovementStop += delegate
			{
				tool.OnScrubbingStop();
			};
			m_hints.ConnectScrubSystemListeners(scrubSystem);
			return scrubSystem;
		}

		private IEnumerator ScrubFurby(Tool tool, ScrubSystemCustomizer scrubCustomizer)
		{
			Tool tool2 = default(Tool);
			ScrubHandler passScrubMessageToTool = delegate(GameObject babyPoint)
			{
				tool2.OnPointScrubbed(babyPoint.transform);
			};
			ScrubSystem scrubs = MakeScrubPoints(tool, passScrubMessageToTool);
			scrubCustomizer(scrubs);
			scrubs.OnProgression += delegate(float f)
			{
				tool2.Progress(m_furbling.Points.transform, f);
			};
			while (scrubs != null)
			{
				yield return null;
			}
			SalonStage stage = tool.Stage;
			UnityEngine.Object.Destroy(tool.gameObject);
			stage.OnCompletion(base.gameObject);
			yield return StartCoroutine(m_furbling.ReactToStageCompletion(stage));
		}

		private IEnumerator DoStage(SalonStage stage, ToolCustomizer toolCustomizer, ScrubSystemCustomizer scrubCustomizer)
		{
			if (this.StageStarted != null)
			{
				this.StageStarted(stage);
			}
			Tool tool = null;
			yield return StartCoroutine(ChooseTool(stage, delegate(Tool t)
			{
				tool = t;
			}));
			if (toolCustomizer != null)
			{
				toolCustomizer(tool);
			}
			yield return StartCoroutine(ScrubFurby(tool, scrubCustomizer));
		}

		private IEnumerator Start()
		{
			m_hints.ConnectShowerListeners(m_shower);
			if (m_scrubsPrefab == null)
			{
				throw new ApplicationException("Scrubs prefab has not been set.");
			}
			m_stool.Boing += delegate
			{
				base.gameObject.SendGameEvent(SalonGameEvent.StoolBoing);
			};
			while (true)
			{
				yield return StartCoroutine(GameFlow());
			}
		}

		private IEnumerator GameFlow()
		{
			base.gameObject.SendGameEvent(SalonGameEvent.ShowerRoom);
			using (m_shower.GetEnabledPeriod())
			{
				float niceSpeed = 4f;
				yield return StartCoroutine(LotionStage());
				yield return StartCoroutine(m_shower.Dial.SetValueOverTime(1f, niceSpeed));
				yield return StartCoroutine(ScrubbingStage());
				yield return StartCoroutine(m_shower.Dial.SetValueOverTime(0f, niceSpeed));
			}
			while (!m_furbling.Animator.IsIdling())
			{
				yield return null;
			}
			yield return StartCoroutine(MoveToStylingRoom());
			yield return StartCoroutine(StylingStage());
			yield return StartCoroutine(PostStylingStage());
			m_showerCurtain.ResetCurtain();
		}

		private IEnumerator LotionStage()
		{
			ScrubSystemCustomizer lotionCustomizer = delegate(ScrubSystem scrubs)
			{
				scrubs.m_envTester = () => !m_shower.IsOn;
			};
			m_hints.HintShowerOff();
			yield return StartCoroutine(DoStage(m_washStage, null, lotionCustomizer));
		}

		private IEnumerator ScrubbingStage()
		{
			ScrubSystemCustomizer scrubCustomizer = delegate(ScrubSystem scrubs)
			{
				scrubs.m_envTester = () => m_shower.IsOn;
			};
			m_hints.HintShowerOn();
			yield return StartCoroutine(DoStage(m_scrubStage, null, scrubCustomizer));
		}

		private IEnumerator MoveToStylingRoom()
		{
			m_shower.EnableDialCollider(false);
			m_shower.SwitchOff();
			base.gameObject.SendGameEvent(SalonGameEvent.TransitionToStylingRoomStart);
			m_showerCurtain.Recede();
			while (m_showerCurtain.IsReceding())
			{
				yield return null;
			}
			base.gameObject.SendGameEvent(SalonGameEvent.StylingRoom);
			m_shower.SwitchOff();
		}

		private IEnumerator StylingStage()
{
    HashSet<ToolEffect> effects = new HashSet<ToolEffect>();
    ToolCustomizer toolCustomizer = delegate(Tool t)
    {
        t.ProgressApplied += delegate(ToolEffect effect, float f)
        {
            effects.Add(effect);
        };
    };
    ScrubSystemCustomizer scrubCustomizer = delegate(ScrubSystem scrubs)
    {
        scrubs.Points.AllScrubbed += delegate
        {
            foreach (ToolEffect item in effects)
            {
                UnityEngine.Object.Destroy(item.gameObject);
            }
        };
    };
    yield return StartCoroutine(DoStage(m_styleStage, toolCustomizer, scrubCustomizer));
}

		private IEnumerator PostStylingStage()
		{
			GameEventRouter.SendEvent(BabyEndMinigameEvent.ReCheckStartingFurbucks);
			int score = 0;
			yield return StartCoroutine(m_furbling.ReactToGameCompletion(delegate(int s)
			{
				score = s;
			}));
			base.gameObject.SendGameEvent(BabyEndMinigameEvent.SetScore, score);
			base.gameObject.SendGameEvent(BabyEndMinigameEvent.ShowDialog);
			yield return StartCoroutine(new WaitForGameEvent().WaitForEvent(BabyEndMinigameEvent.DialogFinished));
		}
	}
}
