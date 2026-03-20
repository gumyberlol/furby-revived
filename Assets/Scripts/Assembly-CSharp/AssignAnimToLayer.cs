using System;
using UnityEngine;

public class AssignAnimToLayer : MonoBehaviour
{
	[Serializable]
	public class AnimLayerAssignment
	{
		public AnimationClip m_anim;

		public int m_layer;

		public Transform[] m_mixingTransforms;
	}

	public AnimLayerAssignment[] m_animLayerAssignments;

	public Animation m_animationComponent;

	private void Start()
	{
		AnimLayerAssignment[] animLayerAssignments = m_animLayerAssignments;
		foreach (AnimLayerAssignment animLayerAssignment in animLayerAssignments)
		{
			m_animationComponent[animLayerAssignment.m_anim.name].layer = animLayerAssignment.m_layer;
			if (animLayerAssignment.m_mixingTransforms[0] != null)
			{
				Transform[] mixingTransforms = animLayerAssignment.m_mixingTransforms;
				foreach (Transform mix in mixingTransforms)
				{
					m_animationComponent[animLayerAssignment.m_anim.name].AddMixingTransform(mix, false);
				}
			}
		}
	}
}
