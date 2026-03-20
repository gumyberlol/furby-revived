using System;
using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class Poop : MonoBehaviour
	{
		public enum PoopType
		{
			None = 0,
			Small = 1,
			Medium = 2,
			Large = 3,
			Generic_Rare = 4,
			Personality_Rare = 5
		}

		public delegate void SmellHandler(Smell s);

		[SerializeField]
		private PoopType m_type;

		[SerializeField]
		private List<FurbyPersonality> m_limitPersonalities;

		[SerializeField]
		private GameObject m_asset;

		[SerializeField]
		private Smell m_smellPrefab;

		[SerializeField]
		private float m_emissionMultiplier;

		private bool m_hasLeft;

		private bool m_selfActivated;

		[SerializeField]
		private float m_likelihood = 1f;

		public float Likelihood
		{
			get
			{
				return m_likelihood;
			}
		}

		public event SmellHandler m_smellCreated;

		public PoopType GetPoopType()
		{
			return m_type;
		}

		public bool IsAppropriateFor(PoopStationFurby furby)
		{
			FurbyPersonality personality = Singleton<FurbyDataChannel>.Instance.FurbyStatus.Personality;
			return m_limitPersonalities.Count == 0 || m_limitPersonalities.Contains(personality);
		}

		public void Start()
		{
			m_hasLeft = false;
			if (m_emissionMultiplier > 0f && m_smellPrefab == null)
			{
				throw new ApplicationException(string.Format("Poop {0} has emission, but no smell prefab.", base.gameObject.name));
			}
			if (m_asset.gameObject.activeInHierarchy && !m_selfActivated)
			{
				throw new ApplicationException(string.Format("Asset \"{0}\" for Poop \"{1}\" is already active.  Wrong item dragged in?", m_asset.name, base.name));
			}
		}

		public void Activate()
		{
			m_asset.SetActive(true);
			m_selfActivated = true;
		}

		public void Leave(Animation flushAnim)
		{
			StartCoroutine(LeaveFlow(flushAnim));
		}

		private IEnumerator LeaveFlow(Animation anim)
		{
			Vector3 startPos = m_asset.transform.position;
			while (anim.isPlaying)
			{
				yield return null;
			}
			Logging.Log("Poop has finished anim.  Setting asset to inactive.");
			m_asset.SetActive(false);
			m_selfActivated = false;
			yield return StartCoroutine(LeaveSmell(startPos));
			m_hasLeft = true;
		}

		public bool HasLeft()
		{
			return m_hasLeft;
		}

		private IEnumerator LeaveSmell(Vector3 pos)
		{
			if (m_emissionMultiplier > 0f)
			{
				float waitTime = 4f;
				Logging.Log(string.Format("{0} waiting {1} before leaving smell.", base.gameObject.name, waitTime));
				yield return new WaitForSeconds(waitTime);
				Logging.Log(string.Format("{0} leaving smell.", base.gameObject.name));
				Smell smell = UnityEngine.Object.Instantiate(m_smellPrefab) as Smell;
				smell.MultiplyEmissionRate(m_emissionMultiplier);
				smell.transform.position = pos;
				base.gameObject.SendGameEvent(PoopStationEvent.SmellSpreading, smell);
				if (this.m_smellCreated != null)
				{
					this.m_smellCreated(smell);
				}
			}
		}
	}
}
