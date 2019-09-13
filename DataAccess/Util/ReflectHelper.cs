namespace DataAccess.Util
{
    /*************************************
     * from castle project
     *************************************/

    using System;
    using System.Reflection;

    public static class ReflectHelper
    {
        public static System.Type ClassForName(string name)
        {
            System.Type result = TypeFromAssembly(name, null, true);
            return result;
        }

        public static System.Type ClassForName(string name,string assemblyname)
        {
            System.Type result = TypeFromAssembly(name,assemblyname, true);
            return result;
        }

        public static System.Type TypeFromAssembly(string name,string assemblyname,bool throwOnError)
        {
            try
            {
                // Try to get the type from an already loaded assembly
                System.Type type = System.Type.GetType(name);

                if (type != null)
                {
                    return type;
                }

                if (assemblyname == null)
                {
                    // No assembly was specified for the type, so just fail
                    string message = "Could not load type " + name + ". Possible cause: no assembly name specified.";
                    if (throwOnError) throw new TypeLoadException(message);
                    return null;
                }

                Assembly assembly = Assembly.Load(assemblyname);

                if (assembly == null)
                {
                    return null;
                }

                type = assembly.GetType(name, throwOnError);

                if (type == null)
                {
                    return null;
                }

                return type;
            }
            catch (Exception e)
            {
                if (throwOnError) throw;
                return null;
            }
        }
    }
}
