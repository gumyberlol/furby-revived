using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon
{
	public class Carousel : Singleton<Carousel>
	{
		public GameObject m_ItemPrefab;

		public UISprite target;

		public PlayMakerFSM fsm;

		public CarouselItem store;

		public ParticleSystem ShineEffect;

		[SerializeField]
		private SalonItemList m_salonItemList;

		public SalonItemList GetSalonItemList()
		{
			return m_salonItemList;
		}

		public void Start()
		{
			UIGrid componentInChildren = GetComponentInChildren<UIGrid>();
			UIDraggablePanel component = GetComponent<UIDraggablePanel>();
			foreach (SalonItem item in m_salonItemList.Items)
			{
				if (WholeGameShopHelpers.IsItemUnlocked(item))
				{
					GameObject gameObject = Object.Instantiate(m_ItemPrefab) as GameObject;
					gameObject.layer = base.gameObject.layer;
					CarouselItem component2 = gameObject.GetComponent<CarouselItem>();
					UIDragPanelContents component3 = gameObject.GetComponent<UIDragPanelContents>();
					UISprite component4 = gameObject.GetComponent<UISprite>();
					ToolSelect component5 = gameObject.GetComponent<ToolSelect>();
					component5.selected = target;
					component5.m_GameStateMachine = fsm;
					component5.selection = store;
					component5.ps = ShineEffect.gameObject;
					component4.spriteName = item.Graphic;
					component4.MakePixelPerfect();
					component3.draggablePanel = component;
					component2.m_SalonItem = item;
					gameObject.transform.parent = componentInChildren.transform;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localScale /= 500f;
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.Rotate(0f, 0f, -30f);
					component4.depth = 1;
				}
			}
			componentInChildren.Reposition();
			component.relativePositionOnReset = new Vector3(0.5f, 0.5f);
			component.ResetPosition();
		}
	}
}
