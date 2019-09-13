using System;
using System.Collections;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using log4net;

namespace DataAccess.Cache
{
	/// <summary>
	/// The standard implementation of the Hibernate <see cref="IQueryCache" />
	/// interface.  This implementation is very good at recognizing stale query
	/// results and re-running queries when it detects this condition, recaching
	/// the new results.
	/// </summary>
	public class StandardQueryCache : IQueryCache
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (StandardQueryCache));
		private readonly ICache queryCache;

		public StandardQueryCache()
		{
			this.queryCache=new HashtableCache();
		}

		#region IQueryCache Members

		public ICache Cache
		{
			get { return queryCache; }
		}


		public void Clear()
		{
			queryCache.Clear();
		}

        public bool Put(QueryKey key, object result, long ts)
        {
            if (result == null)
            {
                return false;
            }

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("caching query results : '{0}'", key);
            }

            IList cacheable = new List<object>(2) { ts };
            cacheable.Add(result);

            queryCache.Put(key, cacheable);
            return true;
        }

		public object Get(QueryKey key)
		{
			if (log.IsDebugEnabled)
			{
				log.DebugFormat("checking cached query results in region:  {1}",  key);
			}
			var cacheable = (IList)queryCache.Get(key);
			if (cacheable == null)
			{
				log.DebugFormat("query results were not found in cache: {0}", key);
				return null;
			}
			var timestamp = (long)cacheable[0];
			if (IsUpToDate(timestamp))
			{
                queryCache.Remove(key);
				log.DebugFormat("cached query results were up to date for: {0}", key);
				return null;
			}

			log.DebugFormat("returning cached query results for: {0}", key);
            object result = cacheable[1];
			return result;
		}

		public void Destroy()
		{
			try
			{
				queryCache.Destroy();
			}
			catch (Exception e)
			{
				log.Warn("could not destroy query cache: " , e);
			}
		}

		#endregion

		protected virtual bool IsUpToDate(long timestamp)
		{
			return DateTime.Now.Ticks>timestamp;
		}
	}
}