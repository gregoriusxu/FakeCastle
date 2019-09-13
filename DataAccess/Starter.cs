namespace DataAccess
{
    /*************************************
    * jack 20120705
    * E-Mail: kujt1999@yahoo.com.cn
    *************************************/

    using System.IO;
    using System.Configuration;
    using System.Collections.Generic;
    using DataAccess.Configuration;

    public class Starter
    {
        protected static IDictionary<string, string> properties;
        public const string DefaultCfgFileName="cfg.xml";

        public static void Initialize(string typename,IConfigurationSource src=null)
        {
            if (src == null)
            {
                src = ConfigurationManager.GetSection("DataAccess") as IConfigurationSource;
                if (src == null && File.Exists(DefaultCfgFileName))
                {
                    src = new XmlConfigurationSource(DefaultCfgFileName);
                }
            }

            properties = new Dictionary<string, string>();

            IConfiguration config = src.GetConfiguration(typename);

            foreach (IConfiguration childConfig in config.Children)
            {
                properties[childConfig.Name] = childConfig.Value;
            }
        }

        public static IDictionary<string, string> Properties
        {
            get { return properties; }
            set { properties = value; }
        }
    }
}
