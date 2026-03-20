using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.MiniGames.HideAndSeek
{
	public class GenerateHideObjects : RelentlessMonoBehaviour
	{
		public GameObject[] m_PrefabList;

		public GameObject m_ParentObject;

		[SerializeField]
		private HideObjectParams[] m_HideObjectParams;

		public float m_InitialSequenceTimeMin = 6f;

		public float m_InitialSequenceTimeMax = 6f;

		private int m_CurrentLevel;

		private List<GameObject> m_createdBalloons = new List<GameObject>();

		public int NumLevels
		{
			get
			{
				return m_HideObjectParams.Length;
			}
		}

		private HideObjectParams CurrentParams
		{
			get
			{
				return m_HideObjectParams[m_CurrentLevel];
			}
		}

		private void Start()
		{
		}

		public void GenerateObjects(int level)
		{
			RemoveObjects();
			m_CurrentLevel = level;
			int width = Screen.width;
			int height = Screen.height;
			float num = width / CurrentParams.m_ObjectsAlongWidth;
			float num2 = height / CurrentParams.m_ObjectsAlongHeight;
			Vector3 vector = new Vector3((0f - num) * (float)(CurrentParams.m_ObjectsAlongWidth - 1) * 0.5f, (0f - num2) * (float)CurrentParams.m_ObjectsAlongHeight * 0.5f, 0f);
			Vector3 to = new Vector3((0f - num) * (float)(CurrentParams.m_ObjectsAlongWidth - 1) * 0.5f, num2 * (float)(CurrentParams.m_ObjectsAlongHeight - 1) * 0.5f, 0f);
			int max = m_PrefabList.Length;
			int min = 0;
			for (int i = 0; i < CurrentParams.m_ObjectsAlongWidth; i++)
			{
				for (int j = 0; j < CurrentParams.m_ObjectsAlongHeight; j++)
				{
					GameObject gameObject = SingletonInstance<PrefabPool>.Instance.InstantiatePrefab(m_PrefabList[Random.Range(min, max)], Vector3.zero, Quaternion.identity);
					float num3 = Random.Range(1f, CurrentParams.m_ObjectScale);
					gameObject.transform.parent = m_ParentObject.transform;
					gameObject.transform.localScale = new Vector3(num * num3, num2 * num3, 1f);
					gameObject.transform.localPosition = vector;
					float num4 = Random.Range(0f - CurrentParams.m_ObjectRotation, CurrentParams.m_ObjectRotation);
					gameObject.transform.Rotate(Vector3.forward, num4);
					TweenPosition component = gameObject.GetComponent<TweenPosition>();
					if ((bool)component)
					{
						component.duration = Random.Range(m_InitialSequenceTimeMin, m_InitialSequenceTimeMax);
						Vector3 vector2 = vector;
						component.from = vector2;
						component.to = to;
					}
					TweenRotation component2 = gameObject.GetComponent<TweenRotation>();
					if ((bool)component2)
					{
						component2.duration = Random.Range(m_InitialSequenceTimeMin, m_InitialSequenceTimeMax);
						component2.from = new Vector3(0f, 0f, 0f - num4);
						component2.to = new Vector3(0f, 0f, num4);
					}
					vector.y -= num2;
					to.y -= num2;
				}
				vector.y = (0f - num2) * (float)CurrentParams.m_ObjectsAlongHeight * 0.5f;
				vector.x += num;
				to.y = num2 * (float)(CurrentParams.m_ObjectsAlongHeight - 1) * 0.5f;
				to.x += num;
			}
		}

		public void RemoveObjects()
		{
			foreach (GameObject createdBalloon in m_createdBalloons)
			{
				SingletonInstance<PrefabPool>.Instance.ReturnToPool(createdBalloon);
			}
			m_createdBalloons = new List<GameObject>();
		}
	}
}
