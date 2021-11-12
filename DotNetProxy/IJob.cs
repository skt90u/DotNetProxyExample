using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetProxy
{
    public abstract class Job<TArgument, TResult>
    // where TArgument : class
    // where TResult : class
    {
        static MockServiceLocator locator;

        public static void Setup()
        {
            locator = new MockServiceLocator(new object[] { 
                                             new IA(),
                                             new IB(),
                                             new NullReferenceException() });
        }

        public abstract TResult Run(IEnumerable<TArgument> args);
    }

    public class IA
    {
        public void Hello() { Console.WriteLine("IA"); }
    }

    public class IB
    {
        public void Hello() { Console.WriteLine("IB"); }

    }

    public class Job1 : Job<int, string>
    {
        IA ia;
        IB ib;

        public Job1(IA ia, IB ib)
        {
            this.ia = ia;
            this.ib = ib;
        }

        public override string Run(IEnumerable<int> args)
        {
            ia.Hello();
            ib.Hello();
            return string.Empty;
        }
    }

    public class MockServiceLocator : ServiceLocatorImplBase
    {
        private readonly IEnumerable<object> _objects;

        public MockServiceLocator(IEnumerable<object> list)
        {
            _objects = list;
        }



        protected override object DoGetInstance(Type serviceType, string key)
        {
            return null == key ? _objects.First(o => serviceType.IsAssignableFrom(o.GetType()))
                               : _objects.First(o => serviceType.IsAssignableFrom(o.GetType()) && Equals(key, o.GetType().FullName));
        }

        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            return _objects.Where(o => serviceType.IsAssignableFrom(o.GetType()));
        }
    }
}
