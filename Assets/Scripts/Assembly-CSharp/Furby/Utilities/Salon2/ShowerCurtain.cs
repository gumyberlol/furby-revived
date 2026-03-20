using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class ShowerCurtain : MonoBehaviour
	{
		[Serializable]
		private class AnimToPlay
		{
			public GameObject m_thing;

			public AnimationClip m_clip;
		}

		[SerializeField]
		private List<AnimToPlay> m_anims;

		private HashSet<AnimToPlay> m_playingAnims;

		public void Recede()
		{
			m_playingAnims = new HashSet<AnimToPlay>();
			foreach (AnimToPlay anim in m_anims)
			{
				Animation animation = FindAnimation(anim.m_thing);
				AnimationClip clip = anim.m_clip;
				if (animation == null)
				{
					Logging.Log(string.Format("Failed to play animation \"{0}\" on object \"{1}\", because it does not have an animation component", (!(clip != null)) ? "(default)" : clip.name, anim.m_thing.name));
					continue;
				}
				if (clip != null)
				{
					animation.Play(clip.name);
				}
				else
				{
					animation.Play();
				}
				m_playingAnims.Add(anim);
			}
		}

		private Animation FindAnimation(GameObject obj)
		{
			Animation animation = null;
			ModelInstance component = obj.GetComponent<ModelInstance>();
			animation = ((!(component != null)) ? animation : component.Instance.animation);
			return (!(animation == null)) ? animation : obj.animation;
		}

		public void Update()
		{
			if (!IsReceding())
			{
				return;
			}
			HashSet<AnimToPlay> hashSet = new HashSet<AnimToPlay>();
			foreach (AnimToPlay playingAnim in m_playingAnims)
			{
				Animation animation = FindAnimation(playingAnim.m_thing);
				AnimationClip clip = playingAnim.m_clip;
				if (!((!(clip != null)) ? animation.isPlaying : animation.IsPlaying(clip.name)))
				{
					hashSet.Add(playingAnim);
				}
			}
			foreach (AnimToPlay item in hashSet)
			{
				m_playingAnims.Remove(item);
			}
		}

		public bool IsReceding()
		{
			return m_playingAnims != null && m_playingAnims.Count > 0;
		}

		public void ResetCurtain()
		{
		}
	}
}
