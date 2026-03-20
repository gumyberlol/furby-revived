namespace Relentless
{
	public interface IProxyHandler
	{
		object Call(object target, params object[] args);
	}
}
