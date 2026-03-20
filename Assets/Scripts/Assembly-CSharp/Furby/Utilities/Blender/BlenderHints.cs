using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Blender
{
	public class BlenderHints : MonoBehaviour
	{
		private delegate void NextAction();

		[SerializeField]
		private Blender m_blender;

		[SerializeField]
		private Carousel m_carousel;

		[SerializeField]
		private HintState m_scrollHint = new HintState();

		[SerializeField]
		private HintState m_selectionHint = new HintState();

		[SerializeField]
		private HintState m_blendHint = new HintState();

		[SerializeField]
		private HintState m_bottleHint = new HintState();

		private bool m_seenScrolling;

		public void Start()
		{
			m_carousel.Scrolled += delegate
			{
				m_seenScrolling = true;
			};
			StartCoroutine(HintAtCarousel(m_carousel, m_blender));
		}

		private IEnumerator HintAtCarousel(Carousel carousel, Blender blender)
		{
			HintState hint = ((!m_seenScrolling) ? m_scrollHint : m_selectionHint);
			Logging.Log("HintAtCarousel with m_seenScrolling = " + m_seenScrolling);
			NextAction nextAction = null;
			Carousel carousel2 = default(Carousel);
			Blender blender2 = default(Blender);
			Carousel.ScrollHandler onScroll = delegate
			{
				if (nextAction == null)
				{
					nextAction = delegate
					{
						StartCoroutine(HintAtCarousel(carousel2, blender2));
					};
				}
			};
			Blender.ItemHandler onAdd = delegate
			{
				nextAction = delegate
				{
					if (m_blender.IsFull)
					{
						StartCoroutine(HintAtBlender(blender2, carousel2));
					}
					else
					{
						StartCoroutine(HintAtCarousel(carousel2, blender2));
					}
				};
			};
			blender.Added += onAdd;
			carousel.Scrolled += onScroll;
			using (hint.GetEnabledPeriod())
			{
				while (nextAction == null)
				{
					yield return null;
				}
			}
			carousel.Scrolled -= onScroll;
			blender.Added -= onAdd;
			nextAction();
		}

		private IEnumerator HintAtBlender(Blender blender, Carousel carousel)
		{
			bool waiting = true;
			Blender blender2 = default(Blender);
			Blender.Handler onActivation = delegate
			{
				waiting = false;
				StartCoroutine(WaitForBottle(blender2));
			};
			Carousel carousel2 = default(Carousel);
			Blender.ItemHandler onRemoval = delegate
			{
				waiting = false;
				StartCoroutine(HintAtCarousel(carousel2, blender2));
			};
			blender.Activated += onActivation;
			blender.Removed += onRemoval;
			using (m_blendHint.GetEnabledPeriod())
			{
				while (waiting)
				{
					yield return null;
				}
			}
			blender.Removed -= onRemoval;
			blender.Activated -= onActivation;
		}

		private IEnumerator WaitForBottle(Blender blender)
		{
			bool waiting = true;
			Blender blender2 = default(Blender);
			Blender.Handler onCompletion = delegate
			{
				waiting = false;
				StartCoroutine(HintAtBottle(blender2));
			};
			blender.BlendCompleted += onCompletion;
			while (waiting)
			{
				yield return null;
			}
			blender.BlendCompleted -= onCompletion;
		}

		private IEnumerator HintAtBottle(Blender blender)
		{
			bool waiting = true;
			Blender.Handler onPour = delegate
			{
				waiting = false;
			};
			blender.Drunk += onPour;
			using (m_bottleHint.GetEnabledPeriod())
			{
				while (waiting)
				{
					yield return null;
				}
			}
			blender.Drunk -= onPour;
		}

		public void Update()
		{
			m_blendHint.TestAndBroadcastState();
			m_scrollHint.TestAndBroadcastState();
			m_selectionHint.TestAndBroadcastState();
			m_bottleHint.TestAndBroadcastState();
		}
	}
}
