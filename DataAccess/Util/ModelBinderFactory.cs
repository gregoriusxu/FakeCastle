namespace DataAccess.Util
{
    /*************************************
    * jack 20120705
    * E-Mail: kujt1999@yahoo.com.cn
    *************************************/

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ModelBinderFactory
    {
        private Func<IModelBinder> binderThunk;
        public static ModelBinderFactory Current { get; private set; }

        static ModelBinderFactory()
        {
            Current = new ModelBinderFactory();
        }


        public void SetModelBinder(IModelBinder binder)
        {
            binderThunk = () => binder;
        }

        public  IModelBinder GetModelBinder()
        {
            if (binderThunk == null)
                return new DefaultModelBinder();

            return binderThunk();
        }
    }
}
