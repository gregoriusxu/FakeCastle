using System.Collections;
using Iesi.Collections.Generic;

namespace DataAccess.Cache
{
	/// <summary>
	/// Defines the contract for caches capable of storing query results.  These
	/// caches should only concern themselves with storing the matching result ids.
	/// The transactional semantics are necessarily less strict than the semantics
	/// of an item cache.
	/// </summary>
	public interface IQueryCache
	{
		ICache Cache { get;}

		void Clear();
        bool Put(QueryKey key, object result, long ts);
		object Get(QueryKey key);
		void Destroy();
	}
}