using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataAccess.Engine
{
    public class QueryParameters
    {
        private string[] _positionalParameterNames;
        private object[] _positionalParameterValues;

        public QueryParameters(IDbDataParameter[] parameters)
        { 
            int i=0;
            if (parameters != null)
            {
                foreach (IDbDataParameter param in parameters)
                {
                    _positionalParameterNames[i++] = param.ParameterName;
                    _positionalParameterValues[i++] = param.Value;
                    NamedParameters.Add(param.ParameterName, param.Value);
                }
            }
            else
            {
                _positionalParameterNames = new string[] { };
                _positionalParameterValues = new object[] { };
                NamedParameters = new Dictionary<string, object>();
            }
             
        }

        private IDictionary<string, object> _namedParameters;

        /// <summary>
        /// Named parameters.
        /// </summary>
        public IDictionary<string, object> NamedParameters
        {
            get { return _namedParameters; }
            set { _namedParameters = value; }
        }

        /// <summary>
        /// Gets or sets an array of <see cref="IType"/> objects that is stored at the index 
        /// of the Parameter.
        /// </summary>
        public string[] PositionalParameterNames
        {
            get { return _positionalParameterNames; }
            set { _positionalParameterNames = value; }
        }

        /// <summary>
        /// Gets or sets an array of <see cref="object"/> objects that is stored at the index 
        /// of the Parameter.
        /// </summary>
        public object[] PositionalParameterValues
        {
            get { return _positionalParameterValues; }
            set { _positionalParameterValues = value; }
        }
    }
}
