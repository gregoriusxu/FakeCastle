namespace DataAccess.Util
{
    /*************************************
    * jack 20120705
    * E-Mail: kujt1999@yahoo.com.cn
    *************************************/

    using System.Data;
    using System.Collections.Generic;

    public interface IModelBinder
    {
         IEnumerable<T> BindModel<T>(object source);
    }
}
