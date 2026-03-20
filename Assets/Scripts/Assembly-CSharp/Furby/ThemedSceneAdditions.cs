using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby
{
	public class ThemedSceneAdditions : MonoBehaviour
	{
		[Serializable]
		public class Addition
		{
			public string m_comments;

			public Transform m_locator;

			public string m_item;
		}

		[SerializeField]
		private ThemePeriod m_period;

		[SerializeField]
		private bool m_forceOnForDebugging;

		[SerializeField]
		private List<Addition> m_additions = new List<Addition>();

		public void Start()
		{
			ThemePeriod period = FurbyGlobals.ThemePeriodChooser.GetPeriod();
			bool flag = period == m_period;
			if (flag | m_forceOnForDebugging)
			{
				DoAdditions();
			}
		}

		private void DoAdditions()
		{
			foreach (Addition addition in m_additions)
			{
				string path = "Seasonal/" + addition.m_item;
				GameObject original = Resources.Load(path) as GameObject;
				GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
				gameObject.transform.parent = addition.m_locator;
				gameObject.transform.localScale = Vector3.one;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				RecursiveSetLayer(gameObject.transform, addition.m_locator.gameObject.layer);
			}
		}

		private void RecursiveSetLayer(Transform t, int layer)
		{
			t.gameObject.layer = layer;
			foreach (Transform item in t)
			{
				item.gameObject.layer = layer;
				RecursiveSetLayer(item, layer);
			}
		}
	}
}
