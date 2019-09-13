namespace DataAccess.Driver
{
    /*************************************
    * jack 20120705
    * E-Mail: kujt1999@yahoo.com.cn
    *************************************/

    using System.Data;
    using System.Collections.Generic;

    public interface IDriver
    {
        IDbConnection CreateConnection();
        IDbCommand CreateCommand();
        IDbDataAdapter CreateAdapter();
        IDbDataParameter GenerateParameter(IDbCommand command, string name, DbType type);
        IDbDataParameter GenerateParameter(string name, DbType type);
        IDbCommand GenerateCommand(CommandType type, string sql, IDbDataParameter[] paramters);

        int ExecuteNonQuery(string sql, IDbDataParameter[] paramters = null);
        DataSet GetDataSet(string sql, IDbDataParameter[] paramters = null);
        DataTable GetDataTable(string sql, IDbDataParameter[] paramters = null);
        IDataReader ExecuteDataReader(string sql, IDbDataParameter[] paramters = null);
        object ExecuteScalar(string sql, IDbDataParameter[] paramters = null);

        int ExecuteQuery(IDbCommand command);
        IDataReader ExecuteReader(IDbCommand command, CommandBehavior cmdbehavior);

        DataSet GetDataSet(IDbDataAdapter adapter, string sql, IDbDataParameter[] paramters);
        DataTable GetDataTable(IDbDataAdapter adapter, string sql, IDbDataParameter[] paramters);

        IEnumerable<T> FindAll<T>(IDbDataAdapter adapter, string sql, IDbDataParameter[] paramters);
        IEnumerable<T> FindAll<T>(string sql, IDbDataParameter[] paramters = null);
        IEnumerable<T> PagerList<T>(string sql, int pageindex,out int count, string defaultsort = "", IDbDataParameter[] paramters = null);
    }
}
