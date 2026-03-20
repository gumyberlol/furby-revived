using System;
using System.Linq;
using UnityEngine;

namespace Relentless
{
	public class ChangeTextureOnMessage : MonoBehaviour
	{
		[SerializeField]
		private SerialisableEnum[] m_reactionEvents;

		private void OnEnable()
		{
			foreach (Type item in m_reactionEvents.Select((SerialisableEnum x) => x.Type).Distinct())
			{
				GameEventRouter.AddDelegateForType(item, OnReactionEvent);
			}
			if (Screen.orientation == ScreenOrientation.Portrait)
			{
				base.gameObject.transform.Rotate(new Vector3(0f, 0f, 180f));
			}
		}

		private void OnDisable()
		{
			if (!GameEventRouter.Exists)
			{
				return;
			}
			foreach (Type item in m_reactionEvents.Select((SerialisableEnum x) => x.Type).Distinct())
			{
				GameEventRouter.RemoveDelegateForType(item, OnReactionEvent);
			}
		}

		private void OnReactionEvent(Enum eventType, GameObject originator, params object[] parameters)
		{
			if (m_reactionEvents.Any((SerialisableEnum x) => x.Value.Equals(eventType)))
			{
				int num = 0;
				if (num < parameters.Length)
				{
					object obj = parameters[num];
					WebCamTexture wcTexture = obj as WebCamTexture;
					UpdateWebCamTexture(wcTexture);
				}
			}
		}

		private void UpdateWebCamTexture(WebCamTexture wcTexture)
		{
			SetTexture(wcTexture);
		}

		private void SetTexture(Texture texture)
		{
			UITexture component = GetComponent<UITexture>();
			if ((bool)component)
			{
				component.mainTexture = texture;
			}
		}
	}
}
