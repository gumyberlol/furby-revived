using System.Collections;
using UnityEngine;

namespace Relentless
{
	[AddComponentMenu("RS System/AutoLoad")]
	public class SubsceneLoader : RelentlessMonoBehaviour
	{
		private enum LoadState
		{
			NotLoaded = 0,
			Loading = 1,
			Loaded = 2,
			FailedToLoad = 3
		}

		private LoadState m_loadedState;

		public LevelReference SceneRef;

		public bool LoadImmediately = true;

		public bool DeactivateRootObjectAfterLoad = true;

		public bool MoveSceneNameNodeToChild = true;

		[SerializeField]
		private bool m_additiveLoad = true;

		public bool IsLoaded
		{
			get
			{
				return m_loadedState == LoadState.Loaded;
			}
		}

		public bool HasFailed
		{
			get
			{
				return m_loadedState == LoadState.FailedToLoad;
			}
		}

		public event SubSceneLoadStatusHandler OnSubsceneStartedLoading;

		public event SubSceneLoadStatusHandler OnSubsceneFinishedLoading;

		private void SetState(LoadState newState)
		{
			m_loadedState = newState;
			SubSceneLoadStatusHandler subSceneLoadStatusHandler;
			switch (m_loadedState)
			{
			default:
				return;
			case LoadState.Loading:
				subSceneLoadStatusHandler = this.OnSubsceneStartedLoading;
				break;
			case LoadState.Loaded:
			case LoadState.FailedToLoad:
				subSceneLoadStatusHandler = this.OnSubsceneFinishedLoading;
				break;
			}
			if (subSceneLoadStatusHandler != null)
			{
				subSceneLoadStatusHandler(this);
			}
		}

		public void LoadScene()
		{
			if (m_loadedState != LoadState.NotLoaded)
			{
				Logging.LogWarning("Already loading subscene " + base.gameObject.name);
			}
			else
			{
				StartCoroutine(StartLoadingScene());
			}
		}

		private IEnumerator Start()
		{
			if (LoadImmediately)
			{
				return StartLoadingScene();
			}
			return null;
		}

		private IEnumerator StartLoadingScene()
		{
			SetState(LoadState.Loading);
			if (string.IsNullOrEmpty(SceneRef.Path))
			{
				Logging.LogError("Subscene object " + base.gameObject.name + " has no path set", this);
				SetState(LoadState.FailedToLoad);
				yield break;
			}
			yield return (!m_additiveLoad) ? Application.LoadLevelAsync(SceneRef.SceneName) : Application.LoadLevelAdditiveAsync(SceneRef.SceneName);
			GameObject rootNode = GameObject.Find("_" + SceneRef.SceneName);
			if (rootNode == null)
			{
				rootNode = GameObject.Find(SceneRef.SceneName);
			}
			if (rootNode != null && DeactivateRootObjectAfterLoad)
			{
				rootNode.SetActive(false);
			}
			if (MoveSceneNameNodeToChild)
			{
				if (rootNode == null)
				{
					Logging.LogError(string.Format("Node {0} loaded scene {1} and was expecting to find an object called {1}, but it could not be found.\nTo fix this error, either deselect 'Move Scene Name Node To Child' or create a node in the scene called {1}.", base.name, SceneRef.SceneName), this);
					SetState(LoadState.FailedToLoad);
					yield break;
				}
				GameObject oldParent = null;
				if (rootNode.transform.parent != null)
				{
					oldParent = rootNode.transform.parent.gameObject;
					while (oldParent.transform.parent != null)
					{
						oldParent = oldParent.transform.parent.gameObject;
					}
				}
				rootNode.transform.parent = base.transform;
				if (oldParent != null)
				{
					Object.Destroy(oldParent);
				}
				SetState(LoadState.Loaded);
				MoveAndHideScreen moveAndHide = rootNode.GetComponent<MoveAndHideScreen>();
				if (moveAndHide != null)
				{
					moveAndHide.OnSubsceneLoaded();
				}
			}
			else
			{
				SetState(LoadState.Loaded);
			}
		}
	}
}
