namespace DataAccess.Driver
{
    /*************************************
    * jack 20120705
    * E-Mail: kujt1999@yahoo.com.cn
    *************************************/

    using System;
    using System.Data.Common;
    using System.Data;
    using System.Collections.Generic;
    using System.Collections;
    using System.Text;

    using DataAccess.Util;
    using DataAccess.Cache;
    using DataAccess.Engine;

    public abstract class DriverBase : IDriver
    {
        public abstract IDbConnection CreateConnection();
        public abstract IDbCommand CreateCommand();
        public abstract IDbDataAdapter CreateAdapter();
        IQueryCache cache;
        int pagesize = 100;
        OrderBy orderby = OrderBy.Asc;

        //public abstract DriverType DbDriverType{get;}

        public abstract string PagerPart { get; }

        public int PageSize
        {
            get { return pagesize; }
            set { pagesize = value; }
        }

        public string OrderByName
        {
            get;
            set;
        }

        public OrderBy OrderByOrder
        {
            get { return orderby; }
            set { orderby = value; }
        }

        public DriverBase SetPageSize(int pagenum)
        {
            PageSize = pagenum;
            return this;
        }

        public DriverBase SetOrderByName(string orderbyname)
        {
            OrderByName = orderbyname;
            return this;
        }

        public DriverBase SetOrderByOrder(OrderBy orderby)
        {
            OrderByOrder = orderby;
            return this;
        }

        public DriverBase()
        {
            StandardQueryCacheFactory cachefactory = new StandardQueryCacheFactory();
            cache = cachefactory.GetQueryCache();
        }

        public string ConnString
        {
            get;
            set;
        }

        public virtual int ExecuteNonQuery(string sql, IDbDataParameter[] paramters = null)
        {
            IDbCommand command = GenerateCommand(CommandType.Text, sql, paramters);
            return ExecuteQuery(command);
        }

        public virtual DataSet GetDataSet(string sql, IDbDataParameter[] paramters = null)
        {
            QueryKey key = new QueryKey(sql, new QueryParameters(paramters));
            DataSet result = (DataSet)cache.Get(key);
            if (result != null)
                return result;

            IDbDataAdapter adapter = CreateAdapter();
            DataSet ds = GetDataSet(adapter, sql, paramters);

            cache.Put(key, ds, DateTime.Now.AddMinutes(1).Ticks);

            return ds;
        }

        public virtual DataTable GetDataTable(string sql, IDbDataParameter[] paramters = null)
        {
            QueryKey key = new QueryKey(sql, new QueryParameters(paramters));
            DataTable result = (DataTable)cache.Get(key);
            if (result != null)
                return result;

            IDbDataAdapter adapter = CreateAdapter();
            DataTable dt = GetDataTable(adapter, sql, paramters);

            cache.Put(key, dt, DateTime.Now.AddMinutes(1).Ticks);

            return dt;
        }

        public virtual IDataReader ExecuteDataReader(string sql, IDbDataParameter[] paramters = null)
        {
            IDbCommand command = GenerateCommand(CommandType.Text, sql, paramters);
            return ExecuteReader(command);
        }

        public virtual object ExecuteScalar(string sql, IDbDataParameter[] paramters = null)
        {
            IDbConnection connection = CreateConnection();
            connection.ConnectionString = ConnString;
            using (connection)
            {
                try
                {
                    IDbCommand command = GenerateCommand(CommandType.Text, sql, paramters);
                    command.Connection = connection;
                    connection.Open();

                    return command.ExecuteScalar();
                }
                catch
                {
                    throw;
                }
            }
        }

        public virtual IDbCommand GenerateCommand(CommandType type, string sql, IDbDataParameter[] paramters)
        {
            IDbCommand cmd = CreateCommand();
            cmd.CommandType = type;
            cmd.CommandText = sql;

            if (paramters != null)
            {
                foreach (IDbDataParameter parameter in paramters)
                {
                    cmd.Parameters.Add(parameter);
                }
            }

            return cmd;
        }

        public IDbDataParameter GenerateParameter(IDbCommand command, string name, DbType type)
        {
            IDbDataParameter dbParam = command.CreateParameter();
            dbParam.ParameterName = name;
            dbParam.DbType = type;

            return dbParam;
        }

        public abstract IDbDataParameter GenerateParameter(string name, DbType type);

        public IDbDataParameter GenerateOutputParameter(IDbCommand command)
        {
            IDbDataParameter param = GenerateParameter(command, "ReturnValue", DbType.Int32);
            param.Direction = ParameterDirection.Output;
            return param;
        }

        public int ExecuteQuery(IDbCommand command)
        {
            IDbConnection connection = CreateConnection();
            connection.ConnectionString = ConnString;
            using (connection)
            {
                try
                {
                    command.Connection = connection;
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public IDataReader ExecuteReader(IDbCommand command, CommandBehavior cmdbehavior = CommandBehavior.CloseConnection)
        {
            IDbConnection connection = CreateConnection();
            connection.ConnectionString = ConnString;
            using (connection)
            {
                try
                {
                    command.Connection = connection;
                    connection.Open();
                    return command.ExecuteReader();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public DataSet GetDataSet(IDbDataAdapter adapter, string sql, IDbDataParameter[] paramters)
        {
            DataSet ds = new DataSet();
            try
            {
                QueryKey key = new QueryKey(sql, new QueryParameters(paramters));
                DataSet result = (DataSet)cache.Get(key);
                if (result != null)
                    return result;

                IDbConnection connection = CreateConnection();
                connection.ConnectionString = ConnString;
                IDbCommand command = GenerateCommand(CommandType.Text, sql, paramters);
                command.Connection = connection;
                adapter.SelectCommand = command;
                adapter.Fill(ds);

                cache.Put(key, ds, DateTime.Now.AddMinutes(1).Ticks);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;
        }

        public DataTable GetDataTable(IDbDataAdapter adapter, string sql, IDbDataParameter[] paramters)
        {
            QueryKey key = new QueryKey(sql, new QueryParameters(paramters));
            DataTable result = (DataTable)cache.Get(key);
            if (result != null)
                return result;

            DataTable dt = GetDataSet(adapter, sql, paramters).Tables[0];

            cache.Put(key, dt, DateTime.Now.AddMinutes(1).Ticks);

            return dt;
        }

        public IEnumerable<T> FindAll<T>(IDbDataAdapter adapter, string sql, IDbDataParameter[] paramters)
        {
            QueryKey key = new QueryKey(sql, new QueryParameters(paramters));
            IEnumerable<T> result = (IEnumerable<T>)cache.Get(key);
            if (result != null)
                return result;

            IModelBinder binder = ModelBinderFactory.Current.GetModelBinder();

            IEnumerable<T> list = binder.BindModel<T>(GetDataTable(adapter, sql, paramters));
            cache.Put(key, list, DateTime.Now.AddMinutes(1).Ticks);

            return list;
        }

        public IEnumerable<T> FindAll<T>(string sql, IDbDataParameter[] paramters = null)
        {
            QueryKey key = new QueryKey(sql, new QueryParameters(paramters));
            IEnumerable<T> result = (IEnumerable<T>)cache.Get(key);
            if (result != null)
                return result;

            IModelBinder binder = ModelBinderFactory.Current.GetModelBinder();
            IDbDataAdapter adapter = CreateAdapter();

            IEnumerable<T> list = binder.BindModel<T>(GetDataTable(adapter, sql, paramters));

            cache.Put(key, list, DateTime.Now.AddMinutes(1).Ticks);

            return list;
        }

        public IEnumerable<T> PagerList<T>(string sql, int pageindex,out int count, string defaultsort = "", IDbDataParameter[] paramters = null)
        {
            StringBuilder sqlcountbuilder = new StringBuilder(sql);
            StringBuilder sqlrownumbuilder = new StringBuilder(sql);

            SqlHelper.Driver = this;
            SqlHelper.AddCounter(sqlcountbuilder);
            SqlHelper.AddPager(pageindex, pagesize, sqlrownumbuilder,defaultsort);

            if (OrderByName != null)
                SqlHelper.AddOrderBy(OrderByName, OrderByOrder == OrderBy.Asc ? true : false, defaultsort, sqlrownumbuilder);

            count = Int32.Parse(ExecuteScalar(sqlcountbuilder.ToString(), paramters).ToString());

            return FindAll<T>(sqlrownumbuilder.ToString(), paramters);
        }
    }

    public enum OrderBy
    {
        Desc,
        Asc
    }

    //public enum DriverType
    //{
    //    SqlServer,
    //    Oracle
    //}
}
