using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Help
{
	public class HelpPanel : MonoBehaviour
	{
		public delegate void PageMoveHandler(int page);

		[SerializeField]
		private HelpPage m_pagePrefab;

		[SerializeField]
		private UILabel m_helpText;

		[SerializeField]
		private UIButton m_closeButton;

		[SerializeField]
		private GameObject m_previousButton;

		[SerializeField]
		private GameObject m_nextButton;

		public List<GameObject> m_GeneratedObjects = new List<GameObject>();

		public static int PageWidth = 480;

		public static int PageHeight = 640;

		private UIGrid m_pageGrid;

		private TweenPosition m_tweener;

		private HelpData m_helpData;

		private int m_page;

		public int NumPages
		{
			get
			{
				return m_helpData.m_pages.Count;
			}
		}

		public int CurrentPage
		{
			get
			{
				return m_page;
			}
		}

		public event PageMoveHandler PageMoved;

		public void Start()
		{
			if (m_helpData == null)
			{
				throw new ApplicationException(string.Format("No data for {0}.  Maybe you left the prefab in the scene?", base.gameObject.name));
			}
			HelpPage helpPage = GatherPages()[0];
			helpPage.ActivatePage();
			m_closeButton.GetComponent<HelpPanelCloseButton>().m_helpPanel = this;
		}

		public void OnEnable()
		{
			base.gameObject.SendGameEvent(SharedGuiEvents.DialogWasShown);
			m_page = 0;
			if ((bool)m_pageGrid)
			{
				m_pageGrid.transform.localPosition = new Vector3(0f, 0f, m_pageGrid.transform.localPosition.z);
			}
		}

		public void OnDisable()
		{
			foreach (GameObject generatedObject in m_GeneratedObjects)
			{
				UnityEngine.Object.Destroy(generatedObject);
			}
			m_GeneratedObjects.Clear();
			if ((bool)m_pageGrid)
			{
				m_pageGrid.Reposition();
			}
			m_page = 0;
		}

		public void SetupFrom(HelpData data)
		{
			m_helpData = data;
			m_pageGrid = base.gameObject.GetComponentInChildren<UIGrid>();
			m_tweener = m_pageGrid.GetComponent<TweenPosition>();
			foreach (HelpPageData page in data.m_pages)
			{
				InstantiatePageFrom(data, page);
			}
			SetTextFromPageData(data.m_pages[0]);
			m_previousButton.SetActive(false);
			m_nextButton.SetActive(true);
			m_nextButton.collider.enabled = true;
			if (NumPages == 1)
			{
				m_nextButton.SetActive(false);
			}
		}

		private void InstantiatePageFrom(HelpData data, HelpPageData pageData)
		{
			HelpPage helpPage = UnityEngine.Object.Instantiate(m_pagePrefab) as HelpPage;
			helpPage.SetupFrom(data, pageData);
			helpPage.transform.parent = m_pageGrid.transform;
			helpPage.transform.localPosition = Vector3.zero;
			helpPage.transform.localScale = Vector3.one;
			m_pageGrid.Reposition();
			m_GeneratedObjects.Add(helpPage.gameObject);
		}

		public void GoToPage(int page)
		{
			HelpPage[] array = GatherPages();
			page = ((page >= 0) ? ((page < array.Length) ? page : (array.Length - 1)) : 0);
			if (page != m_page)
			{
				HelpPage current = array[m_page];
				HelpPage helpPage = array[page];
				m_closeButton.collider.enabled = false;
				current.DeactivatePage();
				helpPage.ActivatePage();
				m_tweener.from = m_tweener.transform.localPosition;
				m_tweener.to = new Vector3(page * -600, 0f, 0f);
				m_tweener.Reset();
				m_tweener.Play(true);
				m_tweener.onFinished = delegate
				{
					m_closeButton.collider.enabled = true;
					current.UnloadTexture();
					m_tweener.onFinished = null;
				};
				m_page = page;
				if (m_page == 0)
				{
					m_previousButton.SetActive(false);
				}
				else if (!m_previousButton.activeInHierarchy)
				{
					m_previousButton.SetActive(true);
				}
				if (m_page == NumPages - 1)
				{
					m_nextButton.SetActive(false);
				}
				else if (!m_nextButton.activeInHierarchy)
				{
					m_nextButton.SetActive(true);
				}
				SetTextFromPageData(m_helpData.m_pages[page]);
				this.PageMoved(page);
			}
		}

		private HelpPage[] GatherPages()
		{
			return GetComponentsInChildren<HelpPage>();
		}

		private void SetTextFromPageData(HelpPageData data)
		{
			string text = data.m_text;
			string text2 = Singleton<Localisation>.Instance.GetText(text);
			m_helpText.text = text2;
		}
	}
}
