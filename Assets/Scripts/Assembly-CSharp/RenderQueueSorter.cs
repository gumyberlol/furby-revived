using System;
using UnityEngine;

public class RenderQueueSorter : MonoBehaviour
{
	[Serializable]
	public class QueueGroup
	{
		public int queue;

		public Material[] materials;
	}

	public QueueGroup[] queueGroups;

	private static bool queuesSet;

	private void Awake()
	{
		if (queuesSet)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		for (int i = 0; i < queueGroups.Length; i++)
		{
			QueueGroup queueGroup = queueGroups[i];
			for (int j = 0; j < queueGroup.materials.Length; j++)
			{
				if (queueGroup.materials[j] != null)
				{
					queueGroup.materials[j].renderQueue = queueGroup.queue;
				}
			}
		}
		UnityEngine.Object.Destroy(this);
	}
}
