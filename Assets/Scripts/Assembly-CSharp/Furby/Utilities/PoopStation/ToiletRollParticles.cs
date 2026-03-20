using System;
using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class ToiletRollParticles : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_sheetPrefab;

		[SerializeField]
		private ToiletRoll m_spawnPoint;

		[SerializeField]
		private GameObject m_HangingSheet;

		private void Start()
		{
			if (m_spawnPoint == null)
			{
				throw new ApplicationException("m_spawnPoint has not been set");
			}
			m_spawnPoint.Clicked += CreateSheet;
		}

		[ContextMenu("Spawn")]
		private void CreateSheet()
		{
			base.gameObject.SendGameEvent(PoopStationEvent.RollPulled);
			GameObject gameObject = UnityEngine.Object.Instantiate(m_sheetPrefab, m_spawnPoint.transform.position, m_spawnPoint.transform.rotation) as GameObject;
			m_HangingSheet.SetActive(false);
			PlayRandomAnim(gameObject);
			PlayRandomAnim(base.gameObject);
			StartCoroutine(DestroySheetWhenDone(gameObject));
			StartCoroutine(ReinstateHangingSheetWhenDone(base.gameObject));
		}

		private void PlayRandomAnim(GameObject o)
		{
			List<AnimationState> list = new List<AnimationState>();
			foreach (AnimationState item in o.animation)
			{
				list.Add(item);
			}
			float f = UnityEngine.Random.Range(0f, list.Count - 1);
			int index = (int)Mathf.Round(f);
			AnimationState animationState = list[index];
			o.animation.Play(animationState.name);
		}

		private IEnumerator DestroySheetWhenDone(GameObject sheet)
		{
			while (sheet.animation.isPlaying)
			{
				yield return null;
			}
			UnityEngine.Object.Destroy(sheet);
		}

		private IEnumerator ReinstateHangingSheetWhenDone(GameObject gameObject)
		{
			while (base.gameObject.animation.isPlaying)
			{
				yield return null;
			}
			m_HangingSheet.SetActive(true);
		}
	}
}
