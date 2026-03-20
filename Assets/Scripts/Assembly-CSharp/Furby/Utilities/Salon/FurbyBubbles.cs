using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon
{
	public class FurbyBubbles : MonoBehaviour
	{
		public float touchLevel;

		public float touchLevelFadeTime = 3f;

		public float touchThreshold;

		public PlayMakerFSM m_GameStateMachine;

		private Vector2 lastPos;

		public float distThreshold = 18f;

		public Ray[] fingerRays;

		public RaycastHit[] fingerRaysHitInfo;

		private RaycastHit hit;

		private Ray ray;

		public Collider touchArea;

		private bool m_active;

		private bool addParticles = true;

		public GameObject tool;

		public GameObject ShowerControl;

		private bool finalStage;

		public GameObject tipShower;

		public UILabel tip;

		private GameObject instantiated;

		private GameObject instantiatedSparkle;

		private string temp;

		public bool first = true;

		private GameObject m_particlePrefab;

		private GameObject m_particlePrefabSparkle;

		public CarouselItem currentItem;

		public GameObject usableTool;

		public int currentState = 1;

		public ParticleManager showerPm;

		public BubbleContainer bubbles;

		private void Start()
		{
			fingerRays = new Ray[5];
			fingerRaysHitInfo = new RaycastHit[5];
			touchThreshold = bubbles.sphereDifficulty;
		}

		private void Update()
		{
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			touchLevel = Mathf.Clamp(touchLevel -= Time.deltaTime, 0f, touchLevelFadeTime);
			if (touchArea.Raycast(ray, out hit, 2000f) && Input.GetMouseButton(0) && hit.collider == touchArea && Vector2.Distance(Input.mousePosition, lastPos) > distThreshold)
			{
				touchLevel = Mathf.Clamp(touchLevel += Time.deltaTime * 10f, 0f, touchLevelFadeTime);
			}
			if (Input.GetMouseButtonUp(0))
			{
				touchLevel = 0f;
			}
			if (touchLevel >= touchThreshold && usableTool.activeInHierarchy && ShowerSetting())
			{
				if (addParticles)
				{
					if (first)
					{
						base.gameObject.SendGameEvent(SalonGameEvent.SalonLotion);
						m_particlePrefab = currentItem.m_SalonItem.EffectPrefab;
						instantiated = Object.Instantiate(m_particlePrefab) as GameObject;
						instantiated.transform.position = base.transform.position;
						instantiated.SetLayerInChildren(base.gameObject.layer);
						instantiated.transform.parent = base.transform;
						first = false;
					}
				}
				else if (!finalStage && first)
				{
					base.gameObject.SendGameEvent(SalonGameEvent.SalonScrub);
					first = false;
					instantiated.SetActive(false);
				}
				if (finalStage && first)
				{
					first = false;
				}
				m_active = true;
			}
			lastPos = Input.mousePosition;
		}

		public bool isActive()
		{
			return m_active;
		}

		public void setFalse()
		{
			m_active = false;
			first = true;
			if (addParticles)
			{
				addParticles = false;
				currentState = 2;
			}
			else
			{
				currentState = 3;
				finalStage = true;
			}
		}

		private bool ShowerSetting()
		{
			if ((addParticles && !showerPm.CurrentValue()) || (!addParticles && showerPm.CurrentValue()) || finalStage)
			{
				return true;
			}
			if (addParticles)
			{
				tip.text = "Turn Shower Off";
			}
			else
			{
				tip.text = "Turn Shower On";
			}
			return false;
		}

		public void setTrue()
		{
			showerPm.ChangeEmission(0f);
			finalStage = false;
			addParticles = true;
			first = true;
			m_active = false;
			currentState = 1;
			ShowerControl.SetActive(true);
		}
	}
}
