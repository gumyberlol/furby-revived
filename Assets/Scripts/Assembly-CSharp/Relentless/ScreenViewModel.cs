namespace Relentless
{
	public abstract class ScreenViewModel<T> : RelentlessMonoBehaviour, IScreenViewModel where T : class, IDataModel, new()
	{
		private bool m_initialised;

		protected T m_model;

		public virtual void OnShow()
		{
		}

		public virtual void OnHide()
		{
		}

		public virtual void OnExit()
		{
		}

		public void Initialise()
		{
			if (m_model == null)
			{
				m_model = new T();
			}
			if (!m_initialised)
			{
				m_model.Load();
				InitialiseView();
				m_initialised = true;
			}
			else
			{
				Logging.Log("ScreenModelView already initialised", this);
			}
		}

		protected virtual void InitialiseView()
		{
		}
	}
}
