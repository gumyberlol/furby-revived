using UnityEngine;

namespace Relentless
{
	public abstract class ScreenViewModelSingleton<T> : Singleton<ScreenViewModelSingleton<T>>, IScreenViewModel where T : class, IDataModel, new()
	{
		public GameObject ViewRoot;

		protected T m_model;

		public virtual void OnShow()
		{
			if (m_model == null)
			{
				m_model = new T();
			}
			m_model.Load();
			if (ViewRoot == null)
			{
				ViewRoot = base.gameObject;
			}
			InitialiseView();
		}

		public virtual void OnExit()
		{
		}

		public virtual void OnHide()
		{
		}

		public void Start()
		{
		}

		public void OnEnable()
		{
		}

		protected virtual void InitialiseView()
		{
		}
	}
}
