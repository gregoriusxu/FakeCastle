using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DataAccess.Engine;
using DataAccess.Util;

namespace DataAccess.Cache
{
	[Serializable]
	public class QueryKey
	{
		private readonly string sqlQueryString;
		private readonly string[] names;
		private readonly object[] values;
        private readonly IDictionary<string, object> namedParameters;
		private readonly int hashCode;

		/// <summary>
		/// Initializes a new instance of the <see cref="QueryKey"/> class.
		/// </summary>
		/// <param name="factory">the session factory for this query key, required to get the identifiers of entities that are used as values.</param>
		/// <param name="queryString">The query string.</param>
		/// <param name="queryParameters">The query parameters.</param>
		/// <param name="filters">The filters.</param>
		public QueryKey(string queryString, QueryParameters queryParameters)
		{
			sqlQueryString = queryString;
            names = queryParameters.PositionalParameterNames;
			values = queryParameters.PositionalParameterValues;
            namedParameters = queryParameters.NamedParameters;

			hashCode = ComputeHashCode();
		}

		public override bool Equals(object other)
		{
			QueryKey that = (QueryKey) other;
			if (!sqlQueryString.Equals(that.sqlQueryString))
			{
				return false;
			}

            if (names == null)
			{
                if (that.names != null)
				{
					return false;
				}
			}
			else
			{
                if (that.names == null)
				{
					return false;
				}
                if (names.Length != that.names.Length)
				{
					return false;
				}

                for (int i = 0; i < names.Length; i++)
				{
                    if (!names[i].Equals(that.names[i]))
					{
						return false;
					}
					if (!Equals(values[i], that.values[i]))
					{
						return false;
					}
				}
			}

			if (!CollectionHelper.DictionaryEquals(namedParameters, that.namedParameters))
			{
				return false;
			}

			return true;
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public int ComputeHashCode()
		{
			unchecked
			{
				int result = 13;

				result = 37 * result + ( namedParameters == null ? 0: CollectionHelper.GetHashCode(namedParameters));

				for (int i = 0; i < names.Length; i++)
				{
                    result = 37 * result + (names[i] == null ? 0 : names[i].GetHashCode());
				}
				for (int i = 0; i < values.Length; i++)
				{
					result = 37 * result + (values[i] == null ? 0 : values[i].GetHashCode());
				}
;
				result = 37 * result + sqlQueryString.GetHashCode();
				return result;
			}
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder()
				.Append("sql: ")
				.Append(sqlQueryString);


			if (namedParameters != null)
			{
				buf
					.Append("; named parameters: ")
                    .Append(CollectionPrinter.ToString(namedParameters));
			}

			return buf.ToString();
		}
	}
}
