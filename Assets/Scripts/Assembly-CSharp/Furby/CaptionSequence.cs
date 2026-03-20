using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class CaptionSequence
	{
		[SerializeField]
		public Caption[] m_Captions;

		public Caption GetCaption(int index)
		{
			if (index >= 0 && index < m_Captions.Length)
			{
				return m_Captions[index];
			}
			return null;
		}

		public int GetAppropriateCaptionIndex(float timeStamp)
		{
			for (int i = 0; i < m_Captions.Length; i++)
			{
				Caption caption = m_Captions[i];
				if (timeStamp >= caption.m_TimeStamp_Start && timeStamp <= caption.m_TimeStamp_End)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
