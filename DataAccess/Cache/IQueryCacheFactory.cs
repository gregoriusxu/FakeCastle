using System.Collections.Generic;
//using NHibernate.Cfg;

namespace DataAccess.Cache
{
	/// <summary>
	/// Defines a factory for query cache instances.  These factories are responsible for
	/// creating individual QueryCache instances.
	/// </summary>
	public interface IQueryCacheFactory
	{
		IQueryCache GetQueryCache();
	}
}