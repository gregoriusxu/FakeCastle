using System.Collections.Generic;
using System.Data;

namespace DataAccess.Cache
{
	/// <summary>
	/// Standard Hibernate implementation of the IQueryCacheFactory interface.  Returns
	/// instances of <see cref="StandardQueryCache" />.
	/// </summary>
	public class StandardQueryCacheFactory : IQueryCacheFactory
	{
        static IQueryCache _cache;
        static readonly object lockobj=new object();

		public IQueryCache GetQueryCache()
		{
            lock (lockobj)
            {
                if (_cache == null)
                {
                    _cache = new StandardQueryCache();
                }

                return _cache;
            }
		}
	}
}