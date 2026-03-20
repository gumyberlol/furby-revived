using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.MiniGames.HideAndSeek
{
	public class Generate3DHideObjects : RelentlessMonoBehaviour
	{
		[EasyEditArray]
		public GameObject[] m_PrefabList;

		public GameObject m_ParentObject;

		[EasyEditArray]
		public HideObjectParams3D[] m_HideObjectParams;

		[SerializeField]
		private string m_risingAnimationName;

		[SerializeField]
		private string m_bouncingAnimationName;

		[SerializeField]
		private float m_wibbleScale = 0.25f;

		public float m_InitialSequenceTimeMin = 6f;

		public float m_InitialSequenceTimeMax = 6f;

		[SerializeField]
		private float m_columnDelay = 0.1f;

		private float m_rowDelay = 0.25f;

		private int m_currentLevel;

		public Camera m_3DCamera;

		private List<GameObject> m_createdBalloons = new List<GameObject>();

		public int NumLevels
		{
			get
			{
				return m_HideObjectParams.Length;
			}
		}

		private HideObjectParams3D CurrentParams
		{
			get
			{
				return m_HideObjectParams[m_currentLevel];
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		private float GetColumnInitialPositionY(bool isEvenColumn, float objectHeight)
		{
			if (isEvenColumn)
			{
				return objectHeight + (float)Screen.height * m_3DCamera.rect.yMin;
			}
			return objectHeight * 0.5f + (float)Screen.height * m_3DCamera.rect.yMin;
		}

		public void GenerateObjects(int level)
		{
			m_currentLevel = level;
			int width = Screen.width;
			int height = Screen.height;
			width = (int)((float)width * m_3DCamera.rect.width);
			height = (int)((float)height * m_3DCamera.rect.height);
			float num = width / CurrentParams.m_ObjectsAlongWidth;
			float num2 = height / CurrentParams.m_ObjectsAlongHeight;
			Vector3 position = new Vector3(num * 0.5f + (float)Screen.width * m_3DCamera.rect.xMin, GetColumnInitialPositionY(false, num2), base.transform.position.z);
			int max = m_PrefabList.Length;
			int min = 0;
			for (int i = 0; i < CurrentParams.m_ObjectsAlongWidth; i++)
			{
				bool flag = i % 2 == 0;
				int num3 = ((!flag) ? CurrentParams.m_TopEvenRowsToAvoid : CurrentParams.m_TopOddRowsToAvoid);
				for (int j = 0; j < CurrentParams.m_ObjectsAlongHeight - num3; j++)
				{
					GameObject gameObject = SingletonInstance<PrefabPool>.Instance.InstantiatePrefab(m_PrefabList[Random.Range(min, max)], Vector3.zero, Quaternion.identity);
					m_createdBalloons.Add(gameObject);
					HexGridPosition hexGridPosition = gameObject.AddComponent<HexGridPosition>();
					hexGridPosition.HexPosition.x = i;
					hexGridPosition.HexPosition.y = j - i / 2;
					float num4 = Random.Range(CurrentParams.m_ObjectScaleMin, CurrentParams.m_ObjectScaleMax);
					gameObject.transform.parent = m_ParentObject.transform;
					Vector3 positionRandomizer = CurrentParams.m_PositionRandomizer;
					Vector3 position2 = m_3DCamera.ScreenToWorldPoint(position);
					position2.x = Random.Range(position2.x - positionRandomizer.x, position2.x + positionRandomizer.x);
					position2.y = Random.Range(position2.y - positionRandomizer.y, position2.y + positionRandomizer.y);
					position2.z = Random.Range(position2.z - positionRandomizer.z, position2.z + positionRandomizer.z);
					gameObject.transform.position = position2;
					gameObject.transform.localScale = new Vector3(num4, num4, num4);
					float angle = Random.Range(0f - CurrentParams.m_ObjectRotation, CurrentParams.m_ObjectRotation);
					gameObject.transform.Rotate(Vector3.forward, angle);
					position.y += num2;
					if (gameObject.animation != null)
					{
						float num5 = (float)(CurrentParams.m_ObjectsAlongHeight - num3 - j) * m_rowDelay + (float)i * m_columnDelay;
						gameObject.animation.Play(m_risingAnimationName);
						gameObject.animation[m_risingAnimationName].time = 0f - num5;
					}
				}
				position.y = GetColumnInitialPositionY(flag, num2);
				position.x += num;
			}
		}

		public void RemoveObjects()
		{
			foreach (Transform item in m_ParentObject.transform)
			{
				SingletonInstance<PrefabPool>.Instance.ReturnToPool(item.gameObject);
			}
			m_createdBalloons = new List<GameObject>();
		}

		public void WibbleObjects()
		{
			foreach (Transform item in m_ParentObject.transform)
			{
				Vector3 position = item.position;
				position.z = 0f;
				float sqrMagnitude = position.sqrMagnitude;
				item.animation.Play(m_bouncingAnimationName);
				item.animation[m_bouncingAnimationName].time = (0f - sqrMagnitude) * m_wibbleScale;
			}
		}
	}
}
