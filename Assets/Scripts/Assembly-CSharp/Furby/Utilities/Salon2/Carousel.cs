using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Furby.Utilities.Salon;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class Carousel : MonoBehaviour
	{
		public delegate void ItemSelectedHandler(SalonItem item);

		public delegate void ScrollHandler();

		private static string s_animIn = "CarouselComeIn";

		private static string s_animOut = "CarouselGoAway";

		[SerializeField]
		private CarouselItem m_itemPrefab;

		[SerializeField]
		private float m_itemScale = 1f;

		[SerializeField]
		private UILabel m_nameLabel;

		private UILabel NameLabel
		{
			get
			{
				return m_nameLabel;
			}
		}

		[SerializeField]
		public UIGrid Grid
		{
			get
			{
				UIGrid componentInChildren = base.gameObject.GetComponentInChildren<UIGrid>();
				if (componentInChildren == null)
				{
					throw new ApplicationException(string.Format("Carousel \"{0}\" doesn't have a grid to place items on.", base.gameObject.name));
				}
				return componentInChildren;
			}
		}

		public event ItemSelectedHandler ItemSelected;

		public event ScrollHandler Scrolled;

		public IEnumerator Start()
		{
			UIDraggablePanel draggable = GetComponentInChildren<UIDraggablePanel>();
			draggable.onDragFinished = delegate
			{
				if (this.Scrolled != null)
				{
					this.Scrolled();
				}
			};
			Collider[] colliders = base.gameObject.GetComponentsInChildren<Collider>();
			Collider[] array = colliders;
			foreach (Collider c in array)
			{
				c.enabled = false;
			}
			base.animation.Play(s_animIn);
			while (base.animation.isPlaying)
			{
				yield return null;
			}
			Collider[] array2 = colliders;
			foreach (Collider c2 in array2)
			{
				c2.enabled = true;
			}
		}

		public void SetupForStage(SalonStage stage)
		{
			List<CarouselItem> list = new List<CarouselItem>();
			foreach (SalonItem item2 in stage.Items.Items)
			{
				CarouselItem item = CreateUIFor(item2);
				list.Add(item);
			}
			int num = 0;
			foreach (CarouselItem item3 in list)
			{
				StringWriter stringWriter = new StringWriter();
				stringWriter.Write((!item3.SalonItem.HasFinalArtwork) ? "B" : "A");
				stringWriter.Write(" - ");
				stringWriter.Write(num.ToString("D2"));
				stringWriter.Write(" - ");
				stringWriter.Write(item3.name);
				item3.name = stringWriter.ToString();
				num++;
			}
			Grid.Reposition();
			IEnumerable<SalonItem> enumerable = stage.Items.Items.Where((SalonItem salonItem) => !salonItem.HasFinalArtwork);
			if (!enumerable.Any())
			{
				return;
			}
			StringWriter stringWriter2 = new StringWriter();
			stringWriter2.Write(string.Format("The following items in list \"{0}\" have unfinished artwork:\n", stage.name));
			foreach (SalonItem item4 in enumerable)
			{
				stringWriter2.Write("{0} ", item4.Name);
			}
			Logging.LogWarning(stringWriter2);
		}

		private UILabel GetName()
		{
			return m_nameLabel;
		}

		private CarouselItem CreateUIFor(SalonItem item)
		{
			CarouselItem carouselItem = UnityEngine.Object.Instantiate(m_itemPrefab) as CarouselItem;
			carouselItem.transform.parent = Grid.transform;
			carouselItem.transform.localScale = new Vector3(m_itemScale, m_itemScale, 1f);
			carouselItem.SetupFrom(item);
			carouselItem.Clicked += delegate
			{
				Logging.Log(string.Format("You selected {0}", item.Name));
				if (this.ItemSelected != null)
				{
					this.ItemSelected(item);
				}
			};
			UIDraggablePanel componentInChildren = base.gameObject.GetComponentInChildren<UIDraggablePanel>();
			carouselItem.gameObject.GetComponentInChildren<UIDragPanelContents>().draggablePanel = componentInChildren;
			return carouselItem;
		}

		public void GoAway()
		{
			StartCoroutine(GoAwayFlow());
		}

		private IEnumerator GoAwayFlow()
		{
			Collider[] colliders = base.gameObject.GetComponentsInChildren<Collider>();
			Collider[] array = colliders;
			foreach (Collider c in array)
			{
				UnityEngine.Object.Destroy(c);
			}
			Animation anim = base.animation;
			anim.Play(s_animOut);
			while (anim.isPlaying)
			{
				yield return null;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
