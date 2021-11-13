using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DotNetProxy
{
    public abstract class Job<TArgument, TResult>
    // where TArgument : class
    // where TResult : class
    {
        public abstract TResult Run(List<TArgument> args);
    }

    public static class Job
    {
        public static object Run(string jobId, string taskId)
        {
            var jobType = GetJobType(jobId);
            var jobArgs = GetJobArgs(jobType, taskId);
            var service = GetServiceProvider().GetService(jobType);

            var method = jobType.GetMethod("Run");
            var jobResult = method.Invoke(service, new object[] { jobArgs });

            return jobResult;
        }

        private static ServiceProvider GetServiceProvider()
        {
            var serviceCollection = new ServiceCollection();

            // TODO: 註冊所有 Service
            serviceCollection.AddTransient(typeof(ILog), typeof(Log));

            foreach (var jobType in GetJobTypes())
                serviceCollection.AddTransient(jobType);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }

        private static Type GetJobType(string jobId)
        {
            try
            {
                return GetJobTypes().First(type => type.Name == jobId);
            }
            catch (Exception ex)
            {
                throw new Exception($"JobId ({jobId}) 找不到對應 Job", ex);
            }
        }

        private static List<Type> GetJobTypes()
        {
            return Assembly.GetExecutingAssembly().GetTypes().Where(type =>
                    type.BaseType != null
                 && type.BaseType.IsGenericType
                 && type.BaseType.GetGenericTypeDefinition() == typeof(Job<,>)).ToList();
        }

        private static DataTable GetDataTable(Type itemType, string taskId)
        {
            DataTable dt = new DataTable();

            // ----------------------------------------
            // 模擬抓取資料庫結果
            // ----------------------------------------
            var itemTypePropertyNames = itemType.GetProperties().Select(property => property.Name).ToList();

            foreach (var itemTypePropertyName in itemTypePropertyNames)
                dt.Columns.Add(itemTypePropertyName, typeof(int));

            for (var i = 0; i < 1000; i++)
            {
                var row = dt.NewRow();

                foreach (var itemTypePropertyName in itemTypePropertyNames)
                    row[itemTypePropertyName] = i;

                dt.Rows.Add(row);
            }
            // ----------------------------------------

            return dt;
        }

        private static object GetJobArgs(Type jobType, string taskId)
        {
            var method = jobType.GetMethod("Run");
            var parameterType = method.GetParameters()[0].ParameterType; // List<Argument>
            var itemType = parameterType.GenericTypeArguments.First(); // Argument
            var itemTypeProperties = itemType.GetProperties();

            var dataTable = GetDataTable(itemType, taskId);
            var values = Array.CreateInstance(itemType, dataTable.Rows.Count);

            var columnNames = new List<string>();
            foreach (DataColumn column in dataTable.Columns)
                columnNames.Add(column.ColumnName);

            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                var item = Activator.CreateInstance(itemType);
                var row = dataTable.Rows[i];

                foreach (var itemTypePropery in itemTypeProperties)
                {
                    if (columnNames.Contains(itemTypePropery.Name))
                        itemTypePropery.SetValue(item, row[itemTypePropery.Name]);
                }

                values.SetValue(item, i);
            }

            return Activator.CreateInstance(parameterType, new object[] { values });
        }
    }
}
