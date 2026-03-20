using Relentless;
using UnityEngine;

namespace Furby
{
	public class LanguageSelectPopulator : MonoBehaviour
	{
		[SerializeField]
		public UIGrid m_ScreenGrid;

		[SerializeField]
		public UICenterOnChild m_CenterOnChild;

		[EasyEditArray]
		[SerializeField]
		public LanguageInstance[] m_SupportedLanguages;

		[SerializeField]
		public UIAtlas m_UIAtlas;

		public GameObject m_CarouselItemPrefab;

		[HideInInspector]
		private GameObject[] m_CarouselObjects;

		public UICamera m_UICamera;

		public GameObject[] m_GameObjectsToHideWhilstInputDisabled;

		[HideInInspector]
		private GameObject m_CurrentLocaleObject;

		private void Start()
		{
			InitializeCarousel();
		}

		private void InitializeCarousel()
		{
			Locale currentLocale = Singleton<Localisation>.Instance.CurrentLocale;
			m_CarouselObjects = new GameObject[m_SupportedLanguages.Length];
			for (int i = 0; i < m_SupportedLanguages.Length; i++)
			{
				LanguageInstance languageInstance = m_SupportedLanguages[i];
				m_CarouselObjects[i] = (GameObject)Object.Instantiate(m_CarouselItemPrefab);
				m_CarouselObjects[i].SetActive(true);
				m_CarouselObjects[i].name = i.ToString("D2") + "_" + m_SupportedLanguages[i].m_Locale;
				m_CarouselObjects[i].transform.parent = m_ScreenGrid.gameObject.transform;
				m_CarouselObjects[i].transform.localScale = new Vector3(1f, 1f, 1f);
				m_CarouselObjects[i].transform.localPosition = new Vector3(0f, 0f, -0.5f);
				if (m_UIAtlas != null)
				{
					GameObject gameObject = m_CarouselObjects[i].transform.Find("Texture").gameObject;
					UISprite uISprite = (UISprite)gameObject.GetComponent(typeof(UISprite));
					uISprite.atlas = m_UIAtlas;
					uISprite.spriteName = languageInstance.m_SpriteName;
					uISprite.MakePixelPerfect();
				}
				ApplyLanguage component = m_CarouselObjects[i].GetComponent<ApplyLanguage>();
				component.Initialise(languageInstance.m_Locale, this);
				if (languageInstance.m_Locale == currentLocale)
				{
					m_CurrentLocaleObject = m_CarouselObjects[i];
				}
			}
			RecenterAndFocusOnActive();
		}

		public void OnEnable()
		{
			RecenterAndFocusOnActive();
		}

		private void RecenterAndFocusOnActive()
		{
			m_ScreenGrid.Reposition();
			m_CenterOnChild.enabled = true;
			if (m_CurrentLocaleObject != null)
			{
				m_CenterOnChild.Recenter(m_CurrentLocaleObject.transform);
			}
			Invoke("SwitchOffFocusSnapping", 0.5f);
		}

		public void AllowInput()
		{
			m_UICamera.enabled = true;
			GameObject[] gameObjectsToHideWhilstInputDisabled = m_GameObjectsToHideWhilstInputDisabled;
			foreach (GameObject gameObject in gameObjectsToHideWhilstInputDisabled)
			{
				gameObject.SetActive(true);
			}
		}

		public void ProhibitInput()
		{
			m_UICamera.enabled = false;
			GameObject[] gameObjectsToHideWhilstInputDisabled = m_GameObjectsToHideWhilstInputDisabled;
			foreach (GameObject gameObject in gameObjectsToHideWhilstInputDisabled)
			{
				gameObject.SetActive(false);
			}
		}

		public void Refresh()
		{
			GameObject[] carouselObjects = m_CarouselObjects;
			foreach (GameObject gameObject in carouselObjects)
			{
				ApplyLanguage component = gameObject.GetComponent<ApplyLanguage>();
				component.Refresh();
			}
		}

		private void SwitchOffFocusSnapping()
		{
			m_CenterOnChild.enabled = false;
		}
	}
}
