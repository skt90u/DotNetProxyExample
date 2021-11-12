using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetProxyExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // 建立依賴注入的容器
            var serviceCollection = new ServiceCollection();
            // 註冊服務
            serviceCollection.AddTransient<App>();
            serviceCollection.AddTransient<IService, ChtService>();
            // 建立依賴服務提供者
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // 執行主服務
            //serviceProvider.GetRequiredService<App>().Run();
            //serviceProvider.GetService<App>().Run();

            args = new List<string> { "11", "22" }.ToArray();
            var jobId = args[0];
            var taskId = args[1];

            JobLauncher.Run(jobId: args[0], taskId: args[1]);
        }
    }

    public static class JobLauncher
    {
        public static void Run(string jobId, string taskId)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient(typeof(JobA));

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var jobType = GetJobType(jobId);
            var jobArgs = GetJobArgs(jobType, taskId);
            var service = serviceProvider.GetService(jobType);
            var method = jobType.GetMethod("Run");
            var jobResult = method.Invoke(service, new object[] { jobArgs });
            var a = 0;
        }

        private static Type GetJobType(string jobId)
        {
            return typeof(JobA);
        }

        private static DataTable GetDataTable(Type jobType, string taskId)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("A", typeof(int));
            dt.Columns.Add("B", typeof(int));
            dt.Columns.Add("C", typeof(int));

            for(var i=0; i<10; i++)
            {
                var row = dt.NewRow();
                row["A"] = i;
                row["B"] = i + i;
                row["C"] = i + i + i;
                dt.Rows.Add(row);
            }

            return dt;
        }

        private static object GetJobArgs(Type jobType, string taskId)
        {
            var method = jobType.GetMethod("Run");
            var itemType = method.GetParameters()[0].ParameterType.GenericTypeArguments.First();
            var argsType = typeof(List<>).MakeGenericType(new Type[] { itemType });

            var dt = GetDataTable(jobType, taskId);

            var values = Array.CreateInstance(itemType, dt.Rows.Count);

            for(var i=0; i<dt.Rows.Count; i++)
            {
                var row = dt.Rows[i];

                var item = Activator.CreateInstance(itemType);

                foreach(DataColumn column in dt.Columns)
                {
                    itemType.GetProperty(column.ColumnName).SetValue(item, row[column.ColumnName]);
                }
                
                values.SetValue(item, i);
            }

            return Activator.CreateInstance(argsType, new object[] { values });
        }
    }

    public class Argument
    {
        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
    }

    public class JobA : Job<Argument, int>
    {
        public override int Run(List<Argument> args)
        {
            //Console.WriteLine(args.Sum());
            //return args.Sum();
            Console.WriteLine(123);
            return 10;
        }
    }

    public abstract class Job<TArgument, TResult>
    // where TArgument : class
    // where TResult : class
    {
        public abstract TResult Run(List<TArgument> args);
    }


    public class App
    {
        private readonly IService _service;

        public App(IService service)
        {
            _service = service;
        }

        public void Run()
        {
            _service.SayHello();
        }
    }

    public interface IService
    {
        void SayHello();
    }

    public class ChtService : IService
    {
        public void SayHello()
        {
            Console.WriteLine("哈囉世界！");
        }
    }

    public class EngService : IService
    {
        public void SayHello()
        {
            Console.WriteLine("Hello World!");
        }
    }
}
