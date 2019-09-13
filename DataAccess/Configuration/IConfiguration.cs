namespace DataAccess.Configuration
{
    /*************************************
     * from castle project
     *************************************/

    using System;
    using System.Collections;

    public  interface IConfiguration
    {
        String Name { get; }
        String Value { get; }
        ConfigurationAttributeCollection Attributes { get; }
        ConfigurationCollection Children { get; }
        object GetValue(Type type, object defaultValue);
    }
}
