namespace Relentless
{
	public class ReturnToPoolAfterTime : RelentlessMonoBehaviour
	{
		public float m_destroyTime;

		private void Start()
		{
			Invoke(DestroySelf, m_destroyTime);
		}

		private void DestroySelf()
		{
			SingletonInstance<PrefabPool>.Instance.ReturnToPool(base.gameObject);
		}

		private void ResetForPool()
		{
			Start();
		}
	}
}
