using Fabric;
using Relentless;
using UnityEngine;

public class FabricRealtimeTimer : RelentlessMonoBehaviour, ICustomTimer
{
	private double m_realTime;

	private float m_realTimeLastFrame;

	private void Start()
	{
		m_realTime = 0.0;
		m_realTimeLastFrame = Time.realtimeSinceStartup;
		FabricTimer.customTimer = this;
	}

	private void Update()
	{
		if (Time.timeScale > 0.01f)
		{
			m_realTime += Time.deltaTime / Time.timeScale;
		}
		else
		{
			m_realTime += Time.realtimeSinceStartup - m_realTimeLastFrame;
		}
		m_realTimeLastFrame = Time.realtimeSinceStartup;
	}

	public float Get()
	{
		return (float)m_realTime;
	}
}
