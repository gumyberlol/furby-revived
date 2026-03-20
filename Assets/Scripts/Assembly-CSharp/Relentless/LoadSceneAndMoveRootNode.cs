using System;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Relentless
{
	[HutongGames.PlayMaker.Tooltip("Loads a scene asynchronously and moves the scene to a root node")]
	[ActionCategory("Relentless")]
	public class LoadSceneAndMoveRootNode : FsmStateAction
	{
		private AsyncOperation m_async;

		[RequiredField]
		[HutongGames.PlayMaker.Tooltip("Name of the scene to load.")]
		public FsmString m_SceneName;

		[HutongGames.PlayMaker.Tooltip("(Optional) Name of the root GameObject in the new Scene.")]
		public FsmString m_newSceneRootNode;

		[HutongGames.PlayMaker.Tooltip("(Optional) Gameobject to move the new scene's root object under.")]
		public FsmGameObject m_rootGameObject;

		[HutongGames.PlayMaker.Tooltip("Name of the scene to load.")]
		public FsmBool m_additive = false;

		public override void OnEnter()
		{
			if (m_additive.Value)
			{
				m_async = Application.LoadLevelAdditiveAsync(m_SceneName.Value);
			}
			else
			{
				m_async = Application.LoadLevelAsync(m_SceneName.Value);
			}
		}

		public override void OnUpdate()
		{
			if (m_async == null)
			{
				Finish();
			}
			else
			{
				if (!m_async.isDone)
				{
					return;
				}
				if (!m_additive.Value && string.Compare(Application.loadedLevelName, m_SceneName.Value) != 0)
				{
					throw new InvalidOperationException("Failed to load scene " + m_SceneName.Value + " (current scene is " + Application.loadedLevelName + ")");
				}
				if (m_newSceneRootNode != null && m_rootGameObject != null && !string.IsNullOrEmpty(m_newSceneRootNode.Value))
				{
					GameObject gameObject = GameObject.Find(m_newSceneRootNode.Value);
					Debug.DebugBreak();
					if (gameObject == null)
					{
						Logging.LogWarning("Couldn't find gameobject called " + m_newSceneRootNode.Value);
					}
					else
					{
						gameObject.transform.parent = m_rootGameObject.Value.transform;
					}
				}
				Finish();
			}
		}
	}
}
