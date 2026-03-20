namespace Relentless
{
	public class ReturnToPoolAfterAnimation : RelentlessMonoBehaviour
	{
		private void Update()
		{
			if ((bool)base.animation && !base.animation.isPlaying)
			{
				SingletonInstance<PrefabPool>.Instance.ReturnToPool(base.gameObject);
			}
		}
	}
}
