namespace DataAccess.Util
{
    /*************************************
    * jack 20120705
    * E-Mail: kujt1999@yahoo.com.cn
    *************************************/

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data;
    using System.Reflection;
    using NewLife.Reflection;

    public class _DefaultModelBinder : IModelBinder
    {
        public IEnumerable<T> BindModel<T>(object source)
        {
            DataTable dt = (DataTable)source;
            IEnumerable<DataColumn> dclist = dt.Columns.Cast<DataColumn>();
            List<T> modellist = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                TypeX typex = TypeX.Create(typeof(T));
                T modelInstance = (T)typex.CreateInstance();
                //T modelInstance = (T)Activator.CreateInstance(typeof(T));
                foreach (PropertyInfo property in typeof(T).GetProperties())
                {
                    DataColumn dc = dclist.Where(item => string.Compare(property.Name, item.ColumnName, true) == 0).FirstOrDefault();

                    if (null != dc)
                    {
                        PropertyInfoX propx = PropertyInfoX.Create(property);
                        propx.SetValue(modelInstance, dr[dc.ColumnName]);

                        //property.SetValue(modelInstance, Convert.ChangeType(dr[dc.ColumnName], property.PropertyType),null);
                    }
                }
                modellist.Add(modelInstance);
            }

            return modellist;
        }
    }

    public class DefaultModelBinder : _DefaultModelBinder
    {
        public static IEnumerable<T> BindModel<T>(DataTable dt)
        {
            _DefaultModelBinder binder = new _DefaultModelBinder();
            return binder.BindModel<T>(dt);
        }
    }


}
