namespace DataAccess.Driver
{
    /*************************************
    * jack 20120705
    * E-Mail: kujt1999@yahoo.com.cn
    *************************************/

    using System.Data;
    using System.Data.SqlClient;

    public class SqlClientDriver:DriverBase
    {
        public override IDbConnection CreateConnection()
        {
            return new SqlConnection();
        }

        public override IDbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public override IDbDataAdapter CreateAdapter()
        {
            return new SqlDataAdapter();
        }

        public override IDbDataParameter GenerateParameter(string name, DbType type)
        {
            return new SqlParameter(name, type);
        }

        //public override DriverType DbDriverType
        //{
        //    get
        //    {
        //        return DriverType.SqlServer;
        //    }
        //}

        public override string PagerPart
        {
            get
            {
                return "ROW_NUMBER() OVER(ORDER BY {0}) ";
            }
        }
    }
}
