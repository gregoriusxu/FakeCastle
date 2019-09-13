namespace DataAccess.Configuration
{
    /*************************************
     * from castle project
     *************************************/

    using System;
    using System.Collections.Specialized;
    using System.Runtime.Serialization;

    [Serializable]
    public class ConfigurationAttributeCollection : NameValueCollection
    {
        public ConfigurationAttributeCollection()
        {
        }

        protected ConfigurationAttributeCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
