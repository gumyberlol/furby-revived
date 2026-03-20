namespace Relentless
{
	public interface IFactoryHandler
	{
		object Call(params object[] args);
	}
}
