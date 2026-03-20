using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class ApplicationModifiers
	{
		public float m_ComAirVolume = 1f;

		public float m_ResolutionMultiplier = 1f;

		public DeviceOrientation m_Orientation = DeviceOrientation.Portrait;

		public bool m_HaveBackgroundLoadingPriorityOverride;

		public ThreadPriority m_BackgroundLoadingPriority = ThreadPriority.Normal;

		public bool m_RequiresDisplayFixer;

		public BannerDimension m_BannerDimension = BannerDimension.BannerSize_768x66;
	}
}
