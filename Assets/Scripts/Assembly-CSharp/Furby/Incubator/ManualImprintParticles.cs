using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Incubator
{
	public class ManualImprintParticles : MonoBehaviour
	{
		[SerializeField]
		private IncubatorGameFlow m_gameFlow;

		[SerializeField]
		private GameObject m_VfxPrefab;

		[SerializeField]
		private Transform m_spawnPoint;

		[SerializeField]
		private Camera m_camera;

		public void Start()
		{
			m_gameFlow.ManualImprintStart += delegate
			{
				GameObject o = UnityEngine.Object.Instantiate(m_VfxPrefab, m_spawnPoint.position, m_spawnPoint.rotation) as GameObject;
				o.transform.parent = m_spawnPoint;
				o.transform.localScale = Vector3.one;
				o.transform.localPosition = Vector3.zero;
				StartCoroutine(DragObject(o.transform));
				Action stop = null;
				stop = delegate
				{
					ParticleSystem[] componentsInChildren = o.GetComponentsInChildren<ParticleSystem>();
					ParticleSystem[] array = componentsInChildren;
					foreach (ParticleSystem particleSystem in array)
					{
						particleSystem.Stop();
					}
					m_gameFlow.ManualImprintEnd -= stop;
				};
				m_gameFlow.ManualImprintEnd += stop;
			};
		}

		private IEnumerator DragObject(Transform t)
		{
			while (t != null)
			{
				Vector3 touchPos = Input.mousePosition;
				if (Input.touchCount > 0)
				{
					touchPos = Input.GetTouch(0).position;
				}
				Logging.Log("Touch at " + touchPos.ToString());
				Plane p = new Plane(new Vector3(0f, 0f, 1f), t.position);
				Ray ray = m_camera.ScreenPointToRay(touchPos);
				float dist = 0f;
				p.Raycast(ray, out dist);
				Logging.Log("Dist = " + dist);
				t.position = ray.GetPoint(dist);
				yield return null;
			}
		}
	}
}
