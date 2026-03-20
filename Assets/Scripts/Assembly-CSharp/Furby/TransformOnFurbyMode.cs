using UnityEngine;

namespace Furby
{
	public class TransformOnFurbyMode : MonoBehaviour
	{
		private enum Method
		{
			Relative = 0,
			Absolute = 1,
			Disabled = 2
		}

		private enum Presence
		{
			ApplyOnFurbyPresent = 0,
			ApplyOnFurbyAbsent = 1
		}

		private enum Activation
		{
			Enable = 0,
			Disable = 1
		}

		[SerializeField]
		private Method TransformationMethod = Method.Absolute;

		[SerializeField]
		private Presence PresenceRequired;

		[SerializeField]
		private Activation ObjectState;

		[SerializeField]
		private Vector3 Position = Vector3.zero;

		[SerializeField]
		private Vector3 Rotation = Vector3.zero;

		[SerializeField]
		private Vector3 Scale = Vector3.one;

		[SerializeField]
		private bool m_WantToApplyOppositeStateIfPresenceIsNotTheRequiredPresence;

		private void Start()
		{
			if (PresenceRequired == CurrentPresence())
			{
				Vector3 localPosition = base.transform.localPosition;
				Vector3 eulerAngles = base.transform.localRotation.eulerAngles;
				Vector3 localScale = base.transform.localScale;
				switch (TransformationMethod)
				{
				case Method.Absolute:
					base.transform.localPosition = Position;
					base.transform.localRotation = Quaternion.Euler(Rotation);
					base.transform.localScale = Scale;
					break;
				case Method.Relative:
					base.transform.localPosition = localPosition + Position;
					base.transform.localScale = new Vector3(localScale.x * Scale.x, localScale.y * Scale.y);
					base.transform.localRotation = Quaternion.Euler(eulerAngles + Rotation);
					break;
				}
				base.gameObject.SetActive(ObjectState == Activation.Enable);
			}
			else if (m_WantToApplyOppositeStateIfPresenceIsNotTheRequiredPresence)
			{
				base.gameObject.SetActive(ObjectState != Activation.Enable);
			}
		}

		private Presence CurrentPresence()
		{
			Presence result = Presence.ApplyOnFurbyPresent;
			if (FurbyGlobals.Player.NoFurbyOnSaveGame())
			{
				result = Presence.ApplyOnFurbyAbsent;
			}
			return result;
		}
	}
}
