using System.Linq.Expressions;

namespace JsonFx.Linq
{
	public interface IQueryTextProvider
	{
		string GetQueryText(Expression expression);
	}
}
