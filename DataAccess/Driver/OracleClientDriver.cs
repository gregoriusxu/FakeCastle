namespace DataAccess.Driver
{
    /*************************************
    * jack 20120705
    * E-Mail: kujt1999@yahoo.com.cn
    *************************************/

    using System.Data;
    using System.Data.OracleClient;

    public class OracleClientDriver:DriverBase
    {
        public override IDbConnection CreateConnection()
        {
            return new OracleConnection();
        }

        public override IDbCommand CreateCommand()
        {
            return new OracleCommand();
        }

        public override IDbDataAdapter CreateAdapter()
        {
            return new OracleDataAdapter();
        }

        public override  IDbDataParameter GenerateParameter(string name, DbType type)
        {
            return new OracleParameter(name, type);
        }

        //public override DriverType DbDriverType
        //{
        //    get
        //    {
        //        return DriverType.Oracle;
        //    }
        //}

        public override string  PagerPart
        {
            get
            {
                return "ROWNUM{0}";
            }
        }
    }
}
