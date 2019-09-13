namespace Sample
{
    /*************************************
    * jack 20120705
    * E-Mail: kujt1999@yahoo.com.cn
    *************************************/

    //system
    using System;
    using System.Data;
    using System.Configuration;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Reflection;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;
    using System.IO;
    using System.Diagnostics;

    //dataaccess
    using DataAccess;
    using DataAccess.Driver;
    using DataAccess.Configuration;
    using DataAccess.Util;

    //castle
    using Castle.DynamicProxy;
    using Castle.Windsor;
    using Castle.MicroKernel.Registration;
    using Castle.Core;
    using Castle.Core.Logging;
    using Castle.Facilities.Logging;

    class Employee
    {
        public string Employeeid { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }

    class Blog
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
    }

    class Program
    {
        public int Call(object o1, object o2, object o3) {
            Console.WriteLine(o1.ToString());
            return 1;
        }

        static void Main(string[] args)
        {
            int times = 2;
            DynamicMethodExecutor executor = new DynamicMethodExecutor(typeof(Program).GetMethod("Call"));
            Stopwatch watch3 = new Stopwatch();
            watch3.Start();
            for (int i = 0; i < times; i++)
            {
                executor.Execute(new Program(), new object[3]{"test","",""});
            }
            watch3.Stop();
            Console.WriteLine(watch3.Elapsed + " (Dynamic executor)");


            ModelBinderFactory.Current.SetModelBinder(new DefaultModelBinder());//a way set selfdefined modelbinder,you can ignore it
            IConfigurationSource src = ConfigurationManager.GetSection("DataAccess") as IConfigurationSource;

            DriverBase db = DatabaseFactory.CreateDriver(src);
            //DataTable dt = db.GetDataTable("select employeeid,lastname,firstname from dbo.Employees");
            //IEnumerable<Employee> list = db.FindAll<Employee>("select employeeid,lastname,firstname from dbo.Employees");
            //int count = 0;
            //IEnumerable<Employee> list = db.PagerList<Employee>("select employeeid,lastname,firstname from dbo.Employees", 1, count, "employeeid");
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            //int count;
            //IEnumerable<Blog> list = db.SetPageSize(1)
            //                           .PagerList<Blog>("select * from blog", 1,out count);
           // DataTable dt = db.GetDataTable("select * from blog");
            IEnumerable<Blog> list = db.FindAll<Blog>("select * from blog");
            timer.Stop();

            Console.WriteLine("first timespan:" + timer.Elapsed);

            timer.Reset();

            timer.Start();
            list = db.FindAll<Blog>("select * from blog");
            timer.Stop();

            Console.WriteLine("second timespan:" + timer.Elapsed);

            // list = db.FindAll<Employee>("select employeeid,lastname,firstname from dbo.Employees");

            //foreach (Employee model in DefaultModelBinder.BindModel<Employee>(dt))
            //{
            //    Console.WriteLine(model.FirstName);
            //}

            //Console.WriteLine("Count:"+count);

            //foreach (Blog model in list)
            //{
            //    Console.WriteLine(model.Name);
            //}

            //foreach (DataRow dr in dt.Rows)
            //{
            //    foreach (DataColumn dc in dt.Columns)
            //    {
            //        Console.Write(dc.ColumnName + ":" + dr[dc.ColumnName] + "\t");
            //    }
            //    Console.WriteLine();
            //}

            //Console.WriteLine("Run 1 - configuration with xml file");
            //using (WindsorContainer container = new WindsorContainer("castle.config"))
            //{
            //    container.AddFacility<LoggingFacility>(f => f.UseLog4Net());
            //    ISomething something = container.Resolve<ISomething>();
            //    something.DoSomething("");
            //    Console.WriteLine("Augment 10 returns " + something.Augment(10));
            //}

            //Console.WriteLine("Run 2 - configuration fluent");
            //using (WindsorContainer container = new WindsorContainer())
            //{
            //    container.Register(
            //        Component.For<IInterceptor>()
            //        .ImplementedBy<BetterDumpInterceptor>()
            //        .Named("myinterceptor"));
            //    container.Register(
            //        Component.For<ISomething>()
            //        .ImplementedBy<Something>()
            //        .Interceptors(InterceptorReference.ForKey("myinterceptor")).Anywhere);
            //    ISomething something = container.Resolve<ISomething>();
            //    something.DoSomething("");
            //    Console.WriteLine("Augment 10 returns " + something.Augment(10));
            //}

            //Console.WriteLine("Run 3 - configuration fluent");
            //using (WindsorContainer container = new WindsorContainer())
            //{
            //    Dictionary<Type, List<String>> RegexSelector = new Dictionary<Type, List<string>>();
            //    RegexSelector.Add(typeof(DumpInterceptor), new List<string>() { "DoSomething" });
            //    RegexSelector.Add(typeof(LogInterceptor), new List<string>() { "Augment" });
            //    InterceptorSelector selector = new InterceptorSelector(RegexSelector);

            //    container.Register(
            //        Component.For<IInterceptor>()
            //        .ImplementedBy<DumpInterceptor>()
            //        .Named("myinterceptor"));

            //    container.Register(
            //      Component.For<IInterceptor>()
            //      .ImplementedBy<LogInterceptor>()
            //      .Named("LogInterceptor"));

            //    container.AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));

            //    container.Register(
            //     Component.For<ISomething>()
            //              .ImplementedBy<Something>()
            //              .Interceptors(InterceptorReference.ForKey("myinterceptor") ,
            //              InterceptorReference.ForKey("LogInterceptor")
            //    )
            //    .SelectedWith(selector).Anywhere);

            //    ISomething something = container.Resolve<ISomething>();
            //    something.DoSomething("");
            //    Console.WriteLine("Augment 10 returns " + something.Augment(10));
            //}

            Console.ReadKey();
        }
    }

    public class LogInterceptor : IInterceptor
    {
        public ILogger Logger { get; set; }
        public LogInterceptor(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            Loggers = new Dictionary<Type, ILogger>();
        }

        //public void Intercept(IInvocation invocation)
        //{
        //    if (Logger.IsDebugEnabled) 
        //        Logger.Debug(CreateInvocationLogString(invocation));
        //    try
        //    {
        //        invocation.Proceed();
        //    }
        //    catch (Exception ex)
        //    {
        //        if (Logger.IsErrorEnabled) Logger.Error(CreateInvocationLogString(invocation), ex);
        //        throw;
        //    }
        //}
        public void Intercept(IInvocation invocation)
        {
            if (!Loggers.ContainsKey(invocation.TargetType))
            {
                Loggers.Add(invocation.TargetType, LoggerFactory.Create(invocation.TargetType));
            }
            ILogger logger = Loggers[invocation.TargetType];
            if (logger.IsDebugEnabled) 
                logger.Debug(CreateInvocationLogString(invocation));
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled) Logger.Error(CreateInvocationLogString(invocation), ex);
                throw;
            }
        }

        public static String CreateInvocationLogString(IInvocation invocation)
        {
            StringBuilder sb = new StringBuilder(100);
            sb.AppendFormat("Called: {0}.{1}(", invocation.TargetType.Name, invocation.Method.Name);
            foreach (object argument in invocation.Arguments)
            {
                String argumentDescription = argument == null ? "null" : DumpObject(argument);
                sb.Append(argumentDescription).Append(",");
            }
            if (invocation.Arguments.Count() > 0) sb.Length--;
            sb.Append(")");
            return sb.ToString();
        }

        private static string DumpObject(object argument)
        {
            Type objtype = argument.GetType();
            if (objtype == typeof(String) || objtype.IsPrimitive || !objtype.IsClass)
                return objtype.ToString();

            MemoryStream ms = new MemoryStream();
            new DataContractSerializer(objtype).WriteObject(ms,argument);
            StreamReader sr = new StreamReader(ms);
            return sr.ReadToEnd();
        }

        public ILoggerFactory LoggerFactory { get; set; }

        public Dictionary<Type, ILogger> Loggers { get; set; }
    }


    public class InterceptorSelector:IInterceptorSelector
    {
        public Dictionary<Type, List<String>> RegexSelector { get; set; }

        public InterceptorSelector(Dictionary<Type, List<string>> regexSelector)
        {
            RegexSelector = regexSelector;
        }

        private Boolean CanIntercept(MethodInfo methodInfo, IInterceptor interceptor)
        {
            List<String> regexForInterceptor;
            if (!RegexSelector.TryGetValue(interceptor.GetType(), out regexForInterceptor))
                return false;
            if (regexForInterceptor == null) return false;

            foreach (var regex in regexForInterceptor)
            {
                if (Regex.IsMatch(methodInfo.Name, regex))
                {
                    return true;
                }
            }
            return false;
        }

        #region IInterceptorSelector Members

        public IInterceptor[] SelectInterceptors(Type type, System.Reflection.MethodInfo method, IInterceptor[] interceptors)
        {
            Utils.ConsoleWriteline(ConsoleColor.Green, "Called interceptor selector for method {0}.{1} and interceptors {2}",
                type.FullName,
                method.Name,
                interceptors
                .Select(i => i.GetType().Name)
                .Aggregate((s1, s2) => s1 + " " + s2));
            return interceptors.Where(i => CanIntercept(method, i)).ToArray();
        }

        #endregion
    }

    public class Utils
    {
        public static void ConsoleWriteline(ConsoleColor color, string stringformat,params string[] showstr)
        { 
            Console.ForegroundColor=color;
            Console.WriteLine(stringformat, showstr);
        }
    }


    public class BetterDumpInterceptor : IInterceptor
    {
        public List<String> RegexSelector { get; set; }

        private Dictionary<RuntimeMethodHandle, Boolean> _scanned
          = new Dictionary<RuntimeMethodHandle, bool>();


        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            if (!CanIntercept(invocation))
            {
                invocation.Proceed();
                return;
            }
        }

        #endregion

        private Boolean CanIntercept(IInvocation invocation)
        {
            if (RegexSelector == null || RegexSelector.Count == 0) return true;
            foreach (var regex in RegexSelector)
            {
                if (Regex.IsMatch(invocation.Method.Name, regex))
                    return true;
            }
            return false;
        }
    }


    public class DumpInterceptor : IInterceptor
    {
        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("DumpInterceptorCalled on method " + invocation.Method.Name);
            invocation.Proceed();
            if (invocation.Method.ReturnType == typeof(Int32))
            {
                invocation.ReturnValue = (Int32)invocation.ReturnValue + 1;
            }
            Console.WriteLine("DumpInterceptor returnvalue is " + (invocation.ReturnValue ?? "NULL"));
        }

        #endregion
    }

    public interface ISomething
    {
        Int32 Augment(Int32 input);
        void DoSomething(String input);
        Int32 Property { get; set; }
    }

    class Something : ISomething
    {
        #region ISomething Members

        public int Augment(int input)
        {
            return input + 1;
        }

        public void DoSomething(string input)
        {
            Console.WriteLine("I'm doing something: " + input);
        }

        public int Property
        {
            get;
            set;
        }

        #endregion
    }
}
