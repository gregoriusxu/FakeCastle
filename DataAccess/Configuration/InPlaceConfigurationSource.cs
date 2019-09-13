namespace DataAccess.Configuration
{
    /*************************************
     * from castle project
     *************************************/

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Configuration;

    public class InPlaceConfigurationSource:IConfigurationSource
    {
        private readonly IDictionary<String, IConfiguration> _type2Config = new Dictionary<String, IConfiguration>();

        public IConfiguration GetConfiguration(String typeName)
        {
            IConfiguration configuration;
            _type2Config.TryGetValue(typeName, out configuration);
            return configuration;
        }

        public void Add(String typeName, IConfiguration config)
        {
            ProcessConfiguration(config);

            _type2Config[typeName] = config;
        }

        public void Add(String typeName, IDictionary<string, string> properties)
        {
            Add(typeName, ConvertToConfiguration(properties));
        }

        private static IConfiguration ConvertToConfiguration(IDictionary<string, string> properties)
        {
            MutableConfiguration conf = new MutableConfiguration("Config");

            foreach (KeyValuePair<string, string> entry in properties)
            {
                conf.Children.Add(new MutableConfiguration(entry.Key, entry.Value));
            }

            return conf;
        }

        private static void ProcessConfiguration(IConfiguration config)
        {
            const string ConnectionStringKey = "connection.connection_string";

            for (int i = 0; i < config.Children.Count; ++i)
            {
                IConfiguration property = config.Children[i];

                if (property.Name.IndexOf(ConnectionStringKey) >= 0)
                {
                    String value = property.Value;
                    Regex connectionStringRegex = new Regex(@"ConnectionString\s*=\s*\$\{(?<ConnectionStringName>[^}]+)\}");

                    if (connectionStringRegex.IsMatch(value))
                    {
                        string connectionStringName = connectionStringRegex.Match(value).
                            Groups["ConnectionStringName"].Value;
                        value = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                        config.Children[i] = new MutableConfiguration(property.Name, value);
                    }
                }
            }
        }
    }
}
