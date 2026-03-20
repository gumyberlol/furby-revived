using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorTimer : MonoBehaviour
	{
		[SerializeField]
		private UISlider m_UISlider;

		[SerializeField]
		private Transform m_DialXform;

		[SerializeField]
		private GameObject m_DialPrefab;

		[SerializeField]
		private Transform m_IconXform;

		[SerializeField]
		private GameObject m_IconPrefab;

		[SerializeField]
		private float m_SliderMin;

		[SerializeField]
		private float m_SliderMax;

		[SerializeField]
		private float m_MinAngle = -90f;

		[SerializeField]
		private float m_MaxAngle = 145f;

		public void Update()
		{
			FurbyBaby inProgressFurbyBaby = FurbyGlobals.Player.InProgressFurbyBaby;
			FurbyPersonality[] incubationPersonalities = inProgressFurbyBaby.IncubationPersonalities;
			for (int i = 0; i < incubationPersonalities.Length; i++)
			{
				SetProgressIcon(i + 1, incubationPersonalities[i]);
			}
			SetProgressTime(inProgressFurbyBaby.IncubationProgress / inProgressFurbyBaby.IncubationDuration);
		}

		public void SetProgressMarkers(IncubatorLevel timings)
		{
			foreach (IncubatorLevel.Interaction item in timings.GenerateImprintPoints())
			{
				SetProgressMark(timings, item.ImprintIndex);
			}
		}

		public void ClearProgressMarkers()
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < m_DialXform.childCount; i++)
			{
				GameObject item = m_DialXform.GetChild(i).gameObject;
				list.Add(item);
			}
			for (int j = 0; j < m_IconXform.childCount; j++)
			{
				GameObject item2 = m_IconXform.GetChild(j).gameObject;
				list.Add(item2);
			}
			Logging.Log("Deleted " + list.Count + " objects.");
			foreach (GameObject item3 in list)
			{
				Object.DestroyImmediate(item3);
			}
			list.Clear();
		}

		public float SetProgressMark(IncubatorLevel gameLevel, int indexValue)
		{
			GameObject gameObject = Object.Instantiate(m_DialPrefab) as GameObject;
			float result = CalculateProgressRatio(gameLevel, indexValue);
			float z = CalculateProgressAngle(gameLevel, indexValue);
			if (indexValue < gameLevel.GetImprintCount())
			{
				gameObject.name = "Marker" + indexValue;
				gameObject.transform.parent = m_DialXform;
				gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, z);
				gameObject.transform.localScale = new Vector3(166f, 166f, 1f);
				gameObject.transform.localPosition = Vector3.zero;
			}
			SetProgressIcon(gameLevel, indexValue);
			return result;
		}

		public float SetProgressIcon(IncubatorLevel gameLevel, int indexValue)
		{
			GameObject gameObject = Object.Instantiate(m_IconPrefab) as GameObject;
			GameObject gameObject2 = gameObject.transform.GetChild(0).gameObject;
			float result = CalculateProgressRatio(gameLevel, indexValue);
			float num = CalculateProgressAngle(gameLevel, indexValue);
			gameObject.name = "Icon" + indexValue;
			gameObject.transform.parent = m_IconXform;
			gameObject.transform.localPosition = gameObject.transform.position;
			gameObject2.transform.localRotation = Quaternion.Euler(10f, 0f - num, 0f);
			gameObject.transform.localRotation = Quaternion.Euler(0f, num, 0f);
			gameObject.transform.localScale = m_IconPrefab.transform.localScale;
			gameObject.SetActive(true);
			return result;
		}

		public void SetProgressIcon(int indexValue, FurbyPersonality personality)
		{
			Transform transform = m_IconXform.Find("Icon" + indexValue);
			if (transform != null)
			{
				Transform child = transform.GetChild(0);
				Transform transform2 = child.Find(personality.ToString());
				Transform transform3 = child.Find("Unassigned");
				transform2.gameObject.SetActive(true);
				transform3.gameObject.SetActive(false);
			}
		}

		public void SetProgressTime(float timeValue)
		{
			m_UISlider.sliderValue = Mathf.Lerp(m_SliderMin, m_SliderMax, timeValue);
		}

		private static float CalculateProgressRatio(IncubatorLevel gameLevel, float imprintIndex)
		{
			return imprintIndex / (float)gameLevel.GetImprintCount() * (float)gameLevel.IncubationTime;
		}

		private float CalculateProgressAngle(IncubatorLevel gameLevel, float imprintIndex)
		{
			return Mathf.Lerp(m_MinAngle, m_MaxAngle, imprintIndex / (float)gameLevel.GetImprintCount());
		}
	}
}
