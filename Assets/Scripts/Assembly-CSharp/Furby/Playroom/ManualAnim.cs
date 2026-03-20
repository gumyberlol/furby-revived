using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class ManualAnim : RelentlessMonoBehaviour
	{
		public float m_AnimationDuration_On = 0.25f;

		public float m_AnimationDuration_Off = 0.25f;

		public float m_FromPositionY;

		public bool m_DisableOverride = true;

		public Vector3 m_FinalPosition = default(Vector3);

		public bool m_EnableAnimations = true;

		public void AnimateOn()
		{
			base.gameObject.SetActive(true);
			if (m_EnableAnimations)
			{
				Vector3 vector = new Vector3(m_FinalPosition.x, m_FromPositionY, m_FinalPosition.z);
				Hashtable args = iTween.Hash("position", vector, "time", m_AnimationDuration_On, "islocal", true, "easetype", "easeInQuad");
				iTween.MoveFrom(base.gameObject, args);
			}
		}

		public void AnimateOff()
		{
			base.gameObject.SetActive(false);
		}
	}
}
