using System.Linq;
using HutongGames.PlayMaker;
using Relentless;

[ActionCategory("Relentless")]
public class LoadMenuScenes : FsmStateAction
{
	public FsmGameObject ScreenManager;

	private SubsceneLoader[] m_sceneLoaders;

	private bool m_done;

	public override void OnEnter()
	{
		m_sceneLoaders = ScreenManager.Value.GetComponentsInChildren<SubsceneLoader>();
		if (m_sceneLoaders != null)
		{
			SubsceneLoader[] sceneLoaders = m_sceneLoaders;
			foreach (SubsceneLoader subsceneLoader in sceneLoaders)
			{
				subsceneLoader.LoadScene();
			}
		}
	}

	public override void OnUpdate()
	{
		if (m_done)
		{
			ScreenManager.Value.GetComponent<GUIScreenManager>().Initialise();
			Finish();
		}
		else
		{
			m_done = m_sceneLoaders.All((SubsceneLoader loader) => loader.IsLoaded || loader.HasFailed);
		}
	}
}
