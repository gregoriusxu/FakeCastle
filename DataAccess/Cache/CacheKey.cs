using System;

namespace DataAccess.Cache
{
	/// <summary>
	/// Allows multiple entity classes / collection roles to be 
	/// stored in the same cache region. Also allows for composite 
	/// keys which do not properly implement equals()/hashCode().
	/// </summary>
	[Serializable]
	public class CacheKey
	{
		private readonly object key;
		private readonly string entityOrRoleName;
		private readonly int hashCode;

		/// <summary> 
		/// Construct a new key for a collection or entity instance.
		/// Note that an entity name should always be the root entity 
		/// name, not a subclass entity name. 
		/// </summary>
		/// <param name="id">The identifier associated with the cached data </param>
		/// <param name="type">The Hibernate type mapping </param>
		/// <param name="entityOrRoleName">The entity or collection-role name. </param>
		/// <param name="entityMode">The entiyt mode of the originating session </param>
		/// <param name="factory">The session factory for which we are caching </param>
		public CacheKey(object id, string entityOrRoleName)
		{
			key = id;
			this.entityOrRoleName = entityOrRoleName;
			hashCode = id.GetHashCode();
		}

		//Mainly for SysCache and Memcache
		public override String ToString()
		{
			// For Component the user can override ToString
			return entityOrRoleName + '#' + key;
		}

		public override bool Equals(object obj)
		{
			CacheKey that = obj as CacheKey;
			if (that == null) return false;
			return entityOrRoleName.Equals(that.entityOrRoleName) && key.Equals(that.key);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public object Key
		{
			get { return key; }
		}

		public string EntityOrRoleName
		{
			get { return entityOrRoleName; }
		}
	}
}