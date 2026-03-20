using Furby;
using Relentless;
using UnityEngine;

public class FindToggleActiveAction : ToggleActiveAction
{
	public string[] m_gameObjectPath;

	private void Start()
	{
		m_targets = new GameObject[m_gameObjectPath.Length];
		for (int i = 0; i < m_gameObjectPath.Length; i++)
		{
			m_targets[i] = GameObjectExtensions.FindWithInactive(m_gameObjectPath[i]);
		}
	}
}
