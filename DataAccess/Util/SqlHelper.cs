using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using DataAccess.Driver;

namespace DataAccess.Util
{
    public enum OperateType : int
    {
        /// <summary>
        /// 
        /// </summary>
        Equal,
        /// <summary>
        /// 
        /// </summary>
        Like,
        /// <summary>
        /// 
        /// </summary>
        Max,
        /// <summary>
        /// 
        /// </summary>
        Min,
        /// <summary>
        /// 
        /// </summary>
        NotEqual,
        /// <summary>
        /// 
        /// </summary>
        Or,
        /// <summary>
        /// 
        /// </summary>
        IsNull
    }

    public enum FunType
    {
        Null,
        To_Date,
        To_Char
    }

    public enum FunFormat
    {
        Null,
        YYYYMM,
        YYYYMMDD,
        YYYYMMDDHH24MI,
        YYYYMMDDHH24MISS
    }

    public class SqlHelper
    {
        public static Collection<string> Conditions = new Collection<string>();
        public static DriverBase Driver;

        public SqlHelper(DriverBase _driver)
        {
            Driver = _driver;
        }

        public static void Add<T>(IList<T> strList, string fieldName, List<IDbDataParameter> paramList)
        {
            StringBuilder whereCondition = new StringBuilder();
            if ((strList != null) && (strList.Count > 0))
            {
                bool flag = true;
                whereCondition.Append(" AND ( ");
                string str = fieldName;
                for (int i = 0; i < strList.Count; i++)
                {
                    IDbDataParameter parameter;
                    string str2 = string.Format(":{0}{1}", str, i);
                    string str3 = string.Format("{0}={1}", fieldName, str2);
                    if (flag)
                    {
                        whereCondition.Append(str3);
                        flag = false;
                    }
                    else
                    {
                        whereCondition.Append(" OR " + str3);
                    }
                    object obj2 = strList[i];
                    if (obj2.GetType() == typeof(string))
                    {
                        parameter = Driver.GenerateParameter(str2, (DbType)13);
                    }
                    else
                    {
                        if (obj2.GetType() != typeof(int))
                        {
                            throw new Exception("不支持的参数类型");
                        }
                        parameter = Driver.GenerateParameter(str2, (DbType)2);
                    }
                    parameter.Value = strList[i];
                    paramList.Add(parameter);
                }
                whereCondition.Append(" )");

                Conditions.Add(whereCondition.ToString());
                //sqltext.Replace("{0}", whereCondition.ToString());
            }
        }

        public static void Add<T>(T value, string fieldName, List<IDbDataParameter> paramList)
        {
            Add<T>(value, fieldName, paramList, OperateType.Equal);
        }

        public static void Add<T>(T value, string fieldName, List<IDbDataParameter> paramList, OperateType operateType)
        {
            if (value != null)
            {
                string str;
                IDbDataParameter parameter;
                string str2;
                string str3 = Regex.Replace(fieldName, "[.,\\(\\)\\s]", "", RegexOptions.IgnoreCase);
                switch (operateType)
                {
                    case OperateType.NotEqual:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" AND {0}<>{1}", fieldName, str2);
                        break;

                    case OperateType.Min:
                        str2 = string.Format(":Min{0}", str3);
                        str = string.Format(" AND {0}>={1}", fieldName, str2);
                        break;

                    case OperateType.Max:
                        str2 = string.Format(":Max{0}", str3);
                        str = string.Format(" AND {0}<={1}", fieldName, str2);
                        break;

                    case OperateType.Like:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" AND {0} like '%'||{1}||'%'", fieldName, str2);
                        break;
                    case OperateType.Or:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" Or {0}={1}", fieldName, str2);
                        break;
                    case OperateType.IsNull:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" AND {0} IS NULL", fieldName);
                        break;
                    default:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" AND {0}={1}", fieldName, str2);
                        break;
                }
                if (operateType != OperateType.IsNull)
                {
                    if (value.GetType() == typeof(string))
                    {
                        object obj2 = value;
                        if (string.IsNullOrEmpty((string)obj2))
                        {
                            Conditions.Add("");
                            return;
                        }
                        parameter = Driver.GenerateParameter(str2, (DbType)13);
                    }
                    else if (value.GetType() == typeof(int))
                    {
                        parameter = Driver.GenerateParameter(str2, (DbType)2);
                    }
                    else if (value.GetType() == typeof(float))
                    {
                        parameter = Driver.GenerateParameter(str2, (DbType)2);
                    }
                    else if (value.GetType() == typeof(DateTime))
                    {
                        parameter = Driver.GenerateParameter(str2, (DbType)11);
                    }
                    else
                    {
                        if (value.GetType() != typeof(decimal))
                        {
                            throw new Exception("不支持的参数类型");
                        }
                        parameter = Driver.GenerateParameter(str2, (DbType)7);
                    }
                    parameter.Value = value;
                    paramList.Add(parameter);
                }
                //sqltext.Replace("{0}", str.ToString());
                Conditions.Add(str);
            }
        }

        //for between...and
        public static void Add<T>(T value1, T value2, string fieldName, FunType funType, FunFormat funFormat, List<IDbDataParameter> paramList)
        {
            if (value1 != null && value2 != null)
            {
                IDbDataParameter parameter1;
                IDbDataParameter parameter2;
                string str1 = fieldName.Replace('.', '_') + "_1";
                string str2 = string.Format(":{0}", str1);
                string str3 = fieldName.Replace('.', '_') + "_2";
                string str4 = string.Format(":{0}", str3);
                string str = string.Format(" AND {0} BETWEEN {1} AND {2}", fieldName, str2, str4);
                if (funType != FunType.Null)
                {
                    string formatstring = "YYYY-MM-DD";
                    if (funFormat == FunFormat.YYYYMM)
                        formatstring = "YYYY-MM";
                    else if (funFormat == FunFormat.YYYYMMDDHH24MI)
                        formatstring = "YYYY-MM-DD HH24:MI";
                    else if (funFormat == FunFormat.YYYYMMDDHH24MISS)
                        formatstring = "YYYY-MM-DD HH24:MI:SS";

                    str = string.Format(" AND {0} BETWEEN {1}({2},'{3}') AND {1}({4},'{3}')", fieldName, funType.ToString(), str2, formatstring, str4);
                }
                if (value1.GetType() == typeof(string) && value2.GetType() == typeof(string))
                {
                    object obj1 = value1;
                    object obj2 = value2;
                    if (string.IsNullOrEmpty((string)obj1) || string.IsNullOrEmpty((string)obj2))
                    {
                        Conditions.Add("");
                        return;
                    }
                    parameter1 = Driver.GenerateParameter(str2, (DbType)13);
                    parameter2 = Driver.GenerateParameter(str4, (DbType)13);
                }
                else if (value1.GetType() == typeof(int) && value2.GetType() == typeof(int))
                {
                    parameter1 = Driver.GenerateParameter(str2, (DbType)2);
                    parameter2 = Driver.GenerateParameter(str4, (DbType)2);
                }
                else if (value1.GetType() == typeof(float) && value2.GetType() == typeof(float))
                {
                    parameter1 = Driver.GenerateParameter(str2, (DbType)2);
                    parameter2 = Driver.GenerateParameter(str4, (DbType)2);
                }
                else if (value1.GetType() == typeof(DateTime) && value2.GetType() == typeof(DateTime))
                {
                    parameter1 = Driver.GenerateParameter(str2, (DbType)11);
                    parameter2 = Driver.GenerateParameter(str4, (DbType)11);
                }
                else
                {
                    if (value1.GetType() != typeof(decimal) || value2.GetType() != typeof(decimal))
                    {
                        throw new Exception("不支持的参数类型");
                    }
                    parameter1 = Driver.GenerateParameter(str2, (DbType)7);
                    parameter2 = Driver.GenerateParameter(str4, (DbType)7);
                }
                parameter1.Value = value1;
                parameter2.Value = value2;
                paramList.Add(parameter1);
                paramList.Add(parameter2);
                //sqltext.Replace("{0}", str.ToString());
                Conditions.Add(str);
            }
        }

        public static void AddLike<T>(T value, string fieldName, List<IDbDataParameter> paramList)
        {
            Add(value, fieldName, paramList, OperateType.Like);
        }

        public static void AddMax<T>(T value, string fieldName, List<IDbDataParameter> paramList)
        {
            Add(value, fieldName, paramList, OperateType.Max);
        }

        public static void AddMin<T>(T value, string fieldName, List<IDbDataParameter> paramList)
        {
            Add(value, fieldName, paramList, OperateType.Min);
        }

        public static void AddNotEqual<T>(T value, string fieldName, List<IDbDataParameter> paramList)
        {
            Add(value, fieldName, paramList, OperateType.NotEqual);
        }

        public static void Or<T>(T value, string fieldName, List<IDbDataParameter> paramList)
        {
            Add(value, fieldName, paramList, OperateType.Or);
        }

        public static void BetweenAnd<T>(T value1, T value2, string fieldName, List<IDbDataParameter> paramList)
        {
            Add(value1, value2, fieldName, FunType.Null, FunFormat.Null, paramList);
        }

        public static void BetweenAnd<T>(T value1, T value2, string fieldName, FunType funType, FunFormat funFormat, List<IDbDataParameter> paramList)
        {
            Add(value1, value2, fieldName, funType, funFormat, paramList);
        }

        #region add to oraclecommand directly
        public static void Add<T>(IList<T> strList, string fieldName, IDbCommand cmd)
        {
            StringBuilder whereCondition = new StringBuilder();
            if ((strList != null) && (strList.Count > 0))
            {
                bool flag = true;
                whereCondition.Append(" AND ( ");
                string str = fieldName;
                for (int i = 0; i < strList.Count; i++)
                {
                    IDbDataParameter parameter;
                    string str2 = string.Format(":{0}{1}", str, i);
                    string str3 = string.Format("{0}={1}", fieldName, str2);
                    if (flag)
                    {
                        whereCondition.Append(str3);
                        flag = false;
                    }
                    else
                    {
                        whereCondition.Append(" OR " + str3);
                    }
                    object obj2 = strList[i];
                    if (obj2.GetType() == typeof(string))
                    {
                        parameter = Driver.GenerateParameter(str2, (DbType)13);
                    }
                    else
                    {
                        if (obj2.GetType() != typeof(int))
                        {
                            throw new Exception("不支持的参数类型");
                        }
                        parameter = Driver.GenerateParameter(str2, (DbType)2);
                    }
                    parameter.Value = strList[i];
                    cmd.Parameters.Add(parameter);
                }
                whereCondition.Append(" )");

                //sqltext.Replace("{0}",whereCondition.ToString());
                Conditions.Add(whereCondition.ToString());
            }
        }

        public static void Add<T>(T value, string fieldName, FunType funType, FunFormat funFormat, IDbCommand cmd)
        {
            Add(value, fieldName, funType, funFormat, cmd, OperateType.Equal);
        }

        public static void Add<T>(T value, string fieldName, IDbCommand cmd)
        {
            Add(value, fieldName, cmd, OperateType.Equal);
        }

        public static void Add<T>(T value, string fieldName, IDbCommand cmd, OperateType operateType)
        {
            if (value != null)
            {
                string str;
                IDbDataParameter parameter;
                string str2;
                string str3 = Regex.Replace(fieldName, "[.,\\(\\)\\s]", "", RegexOptions.IgnoreCase);
                switch (operateType)
                {
                    case OperateType.NotEqual:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" AND {0}<>{1}", fieldName, str2);
                        break;

                    case OperateType.Min:
                        str2 = string.Format(":Min{0}", str3);
                        str = string.Format(" AND {0}>={1}", fieldName, str2);
                        break;

                    case OperateType.Max:
                        str2 = string.Format(":Max{0}", str3);
                        str = string.Format(" AND {0}<={1}", fieldName, str2);
                        break;

                    case OperateType.Like:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" AND {0} like '%'||{1}||'%'", fieldName, str2);
                        break;
                    case OperateType.Or:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" Or {0}={1}", fieldName, str2);
                        break;
                    case OperateType.IsNull:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" AND {0} IS NULL", fieldName);
                        break;
                    default:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" AND {0}={1}", fieldName, str2);
                        break;
                }
                if (operateType != OperateType.IsNull)
                {
                    if (value.GetType() == typeof(string))
                    {
                        object obj2 = value;
                        if (string.IsNullOrEmpty((string)obj2))
                        {
                            Conditions.Add("");
                            return;
                        }
                        parameter = Driver.GenerateParameter(str2, (DbType)13);
                    }
                    else if (value.GetType() == typeof(int))
                    {
                        parameter = Driver.GenerateParameter(str2, (DbType)2);
                    }
                    else if (value.GetType() == typeof(float))
                    {
                        parameter = Driver.GenerateParameter(str2, (DbType)2);
                    }
                    else if (value.GetType() == typeof(DateTime))
                    {
                        parameter = Driver.GenerateParameter(str2, (DbType)11);
                    }
                    else
                    {
                        if (value.GetType() != typeof(decimal))
                        {
                            throw new Exception("不支持的参数类型");
                        }
                        parameter = Driver.GenerateParameter(str2, (DbType)7);
                    }
                    parameter.Value = value;
                    cmd.Parameters.Add(parameter);
                }
                //sqltext.Replace("{0}", str.ToString());
                Conditions.Add(str);
            }
        }

        public static string Format(string str, FunType funType, FunFormat funFormat)
        {
            if (funType != FunType.Null)
            {
                string formatstring = "YYYY-MM-DD";
                if (funFormat == FunFormat.YYYYMM)
                    formatstring = "YYYY-MM";
                else if (funFormat == FunFormat.YYYYMMDDHH24MI)
                    formatstring = "YYYY-MM-DD HH24:MI";
                else if (funFormat == FunFormat.YYYYMMDDHH24MISS)
                    formatstring = "YYYY-MM-DD HH24:MI:SS";

                return string.Format(" {0}({1},'{2}')", funType.ToString(), str, formatstring);
            }
            return str;
        }

        public static void Add<T>(T value, string fieldName, FunType funType, FunFormat funFormat, IDbCommand cmd, OperateType operateType)
        {
            if (value != null)
            {
                string str;
                IDbDataParameter parameter;
                string str2;
                string str3 = Regex.Replace(fieldName, "[.,\\(\\)\\s]", "", RegexOptions.IgnoreCase);

              
                switch (operateType)
                {
                    case OperateType.NotEqual:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" AND {0}<>{1}", fieldName, Format(str2, funType, funFormat));
                        break;

                    case OperateType.Min:
                        str2 = string.Format(":Min{0}", str3);
                        str = string.Format(" AND {0}>={1}", fieldName, Format(str2, funType, funFormat));
                        break;

                    case OperateType.Max:
                        str2 = string.Format(":Max{0}", str3);
                        str = string.Format(" AND {0}<={1}", fieldName, Format(str2, funType, funFormat));
                        break;

                    case OperateType.Like:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" AND {0} like '%'||{1}||'%'", fieldName, Format(str2, funType, funFormat));
                        break;
                    case OperateType.Or:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" Or {0}={1}", fieldName, Format(str2, funType, funFormat));
                        break;
                    case OperateType.IsNull:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" AND {0} IS NULL", fieldName);
                        break;
                    default:
                        str2 = string.Format(":{0}", str3);
                        str = string.Format(" AND {0}={1}", fieldName, Format(str2, funType, funFormat));
                        break;
                }
                if (operateType != OperateType.IsNull)
                {
                    if (value.GetType() == typeof(string))
                    {
                        object obj2 = value;
                        if (string.IsNullOrEmpty((string)obj2))
                        {
                            Conditions.Add("");
                            return;
                        }
                        parameter = Driver.GenerateParameter(str2, (DbType)13);
                    }
                    else if (value.GetType() == typeof(int))
                    {
                        parameter = Driver.GenerateParameter(str2, (DbType)2);
                    }
                    else if (value.GetType() == typeof(float))
                    {
                        parameter = Driver.GenerateParameter(str2, (DbType)2);
                    }
                    else if (value.GetType() == typeof(DateTime))
                    {
                        parameter = Driver.GenerateParameter(str2, (DbType)11);
                    }
                    else
                    {
                        if (value.GetType() != typeof(decimal))
                        {
                            throw new Exception("不支持的参数类型");
                        }
                        parameter = Driver.GenerateParameter(str2, (DbType)7);
                    }
                    parameter.Value = value;
                    cmd.Parameters.Add(parameter);
                }
                //sqltext.Replace("{0}", str.ToString());
                Conditions.Add(str);
            }
        }

        //for between...and
        public static void Add<T>(T value1, T value2, string fieldName, FunType funType, FunFormat funFormat, IDbCommand cmd)
        {
            if (value1 != null && value2 != null)
            {
                IDbDataParameter parameter1;
                IDbDataParameter parameter2;
                string str1 = fieldName.Replace('.', '_') + "_1";
                string str2 = string.Format(":{0}", str1);
                string str3 = fieldName.Replace('.', '_') + "_2";
                string str4 = string.Format(":{0}", str3);
                string str = string.Format(" AND {0} BETWEEN {1} AND {2}", fieldName, str2, str4);
                if (funType != FunType.Null)
                {
                    string formatstring = "YYYY-MM-DD";
                    if (funFormat == FunFormat.YYYYMM)
                        formatstring = "YYYY-MM";
                    else if (funFormat == FunFormat.YYYYMMDDHH24MI)
                        formatstring = "YYYY-MM-DD HH24:MI";
                    else if (funFormat == FunFormat.YYYYMMDDHH24MISS)
                        formatstring = "YYYY-MM-DD HH24:MI:SS";

                    str = string.Format(" AND {0} BETWEEN {1}({2},'{3}') AND {1}({4},'{3}')", fieldName, funType.ToString(), str2, formatstring, str4);
                }
                if (value1.GetType() == typeof(string) && value2.GetType() == typeof(string))
                {
                    object obj1 = value1;
                    object obj2 = value2;
                    if (string.IsNullOrEmpty((string)obj1) || string.IsNullOrEmpty((string)obj2))
                    {
                        Conditions.Add("");
                        return;
                    }
                    parameter1 = Driver.GenerateParameter(str2, (DbType)13);
                    parameter2 = Driver.GenerateParameter(str4, (DbType)13);
                }
                else if (value1.GetType() == typeof(int) && value2.GetType() == typeof(int))
                {
                    parameter1 = Driver.GenerateParameter(str2, (DbType)2);
                    parameter2 = Driver.GenerateParameter(str4, (DbType)2);
                }
                else if (value1.GetType() == typeof(float) && value2.GetType() == typeof(float))
                {
                    parameter1 = Driver.GenerateParameter(str2, (DbType)2);
                    parameter2 = Driver.GenerateParameter(str4, (DbType)2);
                }
                else if (value1.GetType() == typeof(DateTime) && value2.GetType() == typeof(DateTime))
                {
                    parameter1 = Driver.GenerateParameter(str2, (DbType)11);
                    parameter2 = Driver.GenerateParameter(str4, (DbType)11);
                }
                else
                {
                    if (value1.GetType() != typeof(decimal) || value2.GetType() != typeof(decimal))
                    {
                        throw new Exception("不支持的参数类型");
                    }
                    parameter1 = Driver.GenerateParameter(str2, (DbType)7);
                    parameter2 = Driver.GenerateParameter(str4, (DbType)7);
                }
                parameter1.Value = value1;
                parameter2.Value = value2;
                cmd.Parameters.Add(parameter1);
                cmd.Parameters.Add(parameter2);
                //sqltext.Replace("{0}", str.ToString());
                Conditions.Add(str);
            }
        }

        public static void AddLike<T>(T value, string fieldName, IDbCommand cmd)
        {
            Add<T>(value, fieldName, cmd, OperateType.Like);
        }

        public static void AddMax<T>(T value, string fieldName,IDbCommand cmd)
        {
            Add<T>(value, fieldName, cmd, OperateType.Max);
        }

        public static void AddMax<T>(T value, string fieldName, FunType funType, FunFormat funFormat, IDbCommand cmd)
        {
            Add<T>(value, fieldName, funType, funFormat, cmd, OperateType.Max);
        }

        public static void AddMin<T>(T value, string fieldName, IDbCommand cmd)
        {
            Add<T>(value, fieldName, cmd, OperateType.Min);
        }

        public static void AddNotEqual<T>(T value, string fieldName, IDbCommand cmd)
        {
            Add<T>(value, fieldName, cmd, OperateType.NotEqual);
        }

        public static void Or<T>(T value, string fieldName, StringBuilder sqltext, IDbCommand cmd)
        {
            Add<T>(value, fieldName, cmd, OperateType.Or);
        }

        public static void BetweenAnd<T>(T value1, T value2, string fieldName, IDbCommand cmd)
        {
            Add<T>(value1, value2, fieldName, FunType.Null, FunFormat.Null, cmd);
        }

        public static void BetweenAnd<T>(T value1, T value2, string fieldName, FunType funType, FunFormat funFormat, IDbCommand cmd)
        {
            Add<T>(value1, value2, fieldName, funType, funFormat, cmd);
        }

        #endregion

        public static string TransformSql(string sqltext)
        {
            object[] conarg = new object[Conditions.Count];
            int i = 0;
            foreach (string condition in Conditions)
            {
                conarg[i++] = condition;
            }

            Conditions.Clear();

            return string.Format(sqltext, conarg);
        }

        public static void AddPager(int? pageindex, int? pagesize, StringBuilder sqltext, string tablesort = "")
        {
            //if (tablesort == "" && Driver.DbDriverType == DriverType.SqlServer)
            //    throw new Exception("Sql Server数据库分页时必须指定排序字段");
            sqltext.Insert(0, " SELECT * FROM(SELECT T1.*," + string.Format(Driver.PagerPart, tablesort) + " AS RN FROM (")
                   .Append(")T1)" + " WHERE RN>" + (pageindex - 1) * pagesize + " AND RN<=" + pageindex * pagesize);
        }

        public static void AddPager(int? pageindex, int? pagesize, StringBuilder sqltext, int? padindex, string tablesort = "")
        {
            //if (tablesort == "" && Driver.DbDriverType == DriverType.SqlServer)
            //    throw new Exception("Sql Server数据库分页时必须指定排序字段");

            sqltext.Insert(0, " SELECT * FROM(SELECT T1.*," + string.Format(Driver.PagerPart, tablesort) + " AS RN FROM (")
                   .Append(")T1)" + " WHERE RN>" + ((pageindex - 1) * pagesize + padindex) + " AND RN<=" + (pageindex * pagesize + padindex));
        }

        public static string AddPager(int? pageindex, int? pagesize, string sqltext, string tablesort = "")
        {
            //if (tablesort == "" && Driver.DbDriverType == DriverType.SqlServer)
            //    throw new Exception("Sql Server数据库分页时必须指定排序字段");

            sqltext = " SELECT * FROM(SELECT T1.*," + string.Format(Driver.PagerPart, tablesort) + " AS RN FROM (" +
                   sqltext + ")T1)" + " WHERE RN>" + (pageindex - 1) * pagesize + " AND RN<=" + pageindex * pagesize;

            return sqltext;
        }

        public static void AddCounter(StringBuilder sqltext)
        {
            sqltext.Insert(0, " SELECT COUNT(*) FROM(").Append(") T1");
        }

        public static string AddCounter(string sqltext)
        {
            sqltext = " SELECT COUNT(*) FROM(" + sqltext + ") T1";
            return sqltext;
        }

        public static void AddOrderBy(string sort, bool ascending, string defaultsort, StringBuilder sqltext)
        {
            if (sort != null && sort != "")
                sqltext.Append(" ORDER BY " + sort + " " + (ascending ? "ASC" : "DESC"));
            else
            {
                if(defaultsort!=null&&defaultsort!="")
                     sqltext.Append(" ORDER BY " + defaultsort);
            }
        }

        public static string AddOrderBy(string sort, bool ascending, string defaultsort, string sqltext)
        {
            if (sort != null && sort != "")
                sqltext += " ORDER BY " + sort + " " + (ascending ? "ASC" : "DESC");
            else
            {
                if (defaultsort != null && defaultsort != "")
                    sqltext += " ORDER BY " + defaultsort;
            }

            return sqltext;
        }

        public static void AddRowNumber(StringBuilder sqltext)
        {
            sqltext.Insert(0, " SELECT * FROM(SELECT T1.*,ROWNUM RN FROM (")
                   .Append(")t1)");
        }

        public static string AddRowNumber(string sqltext)
        {
            sqltext = " SELECT * FROM(SELECT T1.*,ROWNUM RN FROM (" +
                   sqltext + ")t1)";

            return sqltext;
        }

    }
}
