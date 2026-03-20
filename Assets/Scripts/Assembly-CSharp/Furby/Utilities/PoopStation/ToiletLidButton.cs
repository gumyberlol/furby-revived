using Relentless;
using UnityEngine;

namespace Furby.Utilities.PoopStation
{
	public class ToiletLidButton : MonoBehaviour
	{
		[SerializeField]
		private HintState m_hint;

		public ToiletLid lid;

		public bool EnableHints
		{
			set
			{
				m_hint.SetEnabled(value);
			}
		}

		public void OnClick()
		{
			Logging.Log("Toilet lid button click.");
			Lift();
		}

		public void OnDrag(Vector2 delta)
		{
			if (Vector2.Dot(delta.normalized, new Vector2(0f, 1f)) > 0.5f)
			{
				Logging.Log("Toilet lid button drag.");
				Lift();
			}
		}

		private void Lift()
		{
			StartCoroutine(lid.LiftUp());
			m_hint.Disable();
		}

		private void Start()
		{
			m_hint.Enable();
		}

		private void Update()
		{
			m_hint.TestAndBroadcastState();
		}
	}
}
