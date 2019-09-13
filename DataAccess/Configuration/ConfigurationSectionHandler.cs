namespace DataAccess.Configuration
{
    /*************************************
     * from castle project
     *************************************/

    using System;
    using System.Configuration;
    using System.Xml;

    public class ConfigurationSectionHandler :XmlConfigurationSource, IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            PopulateSource(section);

            return this;
        }
    }
}
