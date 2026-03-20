using HutongGames.PlayMaker;
using UnityEngine;

namespace Relentless
{
	[ActionCategory("Relentless")]
	[HutongGames.PlayMaker.Tooltip("Reloads the current scene asynchronously")]
	public class ReloadCurrentScene : FsmStateAction
	{
		private AsyncOperation m_async;

		[HutongGames.PlayMaker.Tooltip("(Optional) Name of the root GameObject in the new Scene.")]
		public FsmString m_newSceneRootNode;

		[HutongGames.PlayMaker.Tooltip("(Optional) Gameobject to move the new scene's root object under.")]
		public FsmGameObject m_rootGameObject;

		public override void OnEnter()
		{
			string loadedLevelName = Application.loadedLevelName;
			m_async = Application.LoadLevelAsync(loadedLevelName);
		}

		public override void OnUpdate()
		{
			if (m_async == null || !m_async.isDone)
			{
				return;
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
