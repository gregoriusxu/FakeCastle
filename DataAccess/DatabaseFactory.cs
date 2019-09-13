namespace DataAccess
{
    /*************************************
    * jack 20120705
    * E-Mail: kujt1999@yahoo.com.cn
    *************************************/

    using System;
    using System.Data;
    using System.Reflection;
    using DataAccess.Driver;
    using DataAccess.Util;
    using DataAccess.Configuration;
    using Environment = DataAccess.Configuration.Environment;
    using NewLife.Reflection;

    public class DatabaseFactory
    {
        public const string TypeName = "Default";
        static DriverBase db;
        static readonly object lockobj = new object();

        public static DriverBase CreateDriver(IConfigurationSource src = null, string typename = TypeName)
        {
            lock (lockobj)
            {
                string driverString;
                string connString;
                try
                {
                    if (db != null)
                        return db;

                    if (Starter.Properties == null)
                        Starter.Initialize(typename, src);
                    Starter.Properties.TryGetValue(Environment.ConnectionDriver, out driverString);
                    Starter.Properties.TryGetValue(Environment.ConnectionString, out connString);

                    TypeX typex = TypeX.Create(ReflectHelper.ClassForName(driverString));
                    db = (DriverBase)typex.CreateInstance();
                    //db = (DriverBase)Activator.CreateInstance(ReflectHelper.ClassForName(driverString));
                    db.ConnString = connString;
                    return db;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
