namespace DataAccess.Configuration
{
    /*************************************
     * from castle project
     *************************************/

    using System;

    public interface IConfigurationSource
    {
        IConfiguration GetConfiguration(String typeName);
    }
}
