using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon
{
	public class BubbleContainer : MonoBehaviour
	{
		public PlayMakerFSM m_GameStateMachine;

		public FurbyBubbles[] Bubbles;

		private bool currentAttempt;

		public TipTimer Tip;

		public GameObject tool;

		private float time;

		private GameObject m_particlePrefabSparkle;

		private GameObject instantiatedSparkle;

		private ParticleManager pm;

		public float sphereDifficulty;

		public float styleTime;

		private bool styleComplete;

		private bool tempBool;

		private void Start()
		{
			Bubbles = GetComponentsInChildren<FurbyBubbles>();
			base.gameObject.SendGameEvent(SalonGameEvent.Enter);
		}

		private void Update()
		{
			currentAttempt = true;
			FurbyBubbles[] bubbles = Bubbles;
			foreach (FurbyBubbles furbyBubbles in bubbles)
			{
				if (furbyBubbles.first)
				{
					currentAttempt = false;
				}
			}
			int num = state();
			if ((currentAttempt && num == 1) || (currentAttempt && num == 2) || styleComplete)
			{
				FurbyBubbles[] bubbles2 = Bubbles;
				foreach (FurbyBubbles furbyBubbles2 in bubbles2)
				{
					furbyBubbles2.setFalse();
				}
				m_GameStateMachine.SendEvent("ToolUsed");
				Tip.NextCarousel();
				time = 0f;
				styleComplete = false;
			}
			if (state() == 3 && time > styleTime)
			{
				base.gameObject.SendGameEvent(SalonGameEvent.StyleOff);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			base.gameObject.SendGameEvent(SalonGameEvent.SalonRubOn);
			if (state() == 3)
			{
				base.gameObject.SendGameEvent(SalonGameEvent.StyleOn);
				if (!tempBool)
				{
					Logging.Log("yah");
					m_particlePrefabSparkle = Bubbles[1].currentItem.m_SalonItem.EffectPrefab;
					instantiatedSparkle = Object.Instantiate(m_particlePrefabSparkle) as GameObject;
					instantiatedSparkle.SetLayerInChildren(base.gameObject.layer);
					instantiatedSparkle.SetActive(true);
					instantiatedSparkle.transform.parent = base.transform;
					instantiatedSparkle.transform.position = new Vector3(0f, -0.75f, -1f);
					pm = instantiatedSparkle.GetComponent<ParticleManager>();
					tempBool = true;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			base.gameObject.SendGameEvent(SalonGameEvent.SalonRubOff);
			if (state() == 3)
			{
				base.gameObject.SendGameEvent(SalonGameEvent.StyleOff);
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (state() == 3 && tool.GetComponent<DraggableTool>().IsDragActive)
			{
				time += Time.deltaTime;
				pm.ChangeEmission(time / styleTime * 100f);
				if (time > styleTime)
				{
					styleComplete = true;
					pm.Activator(false);
				}
			}
		}

		private int state()
		{
			return Bubbles[1].currentState;
		}

		public void reset()
		{
			FurbyBubbles[] bubbles = Bubbles;
			foreach (FurbyBubbles furbyBubbles in bubbles)
			{
				furbyBubbles.setTrue();
			}
		}
	}
}
