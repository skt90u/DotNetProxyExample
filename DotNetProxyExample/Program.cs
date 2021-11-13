using DotNetProxy;
using System;

namespace DotNetProxyExample
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"result = {Job.Run(jobId: "JobA", taskId: "2021111300005")}");
            Console.WriteLine($"result = {Job.Run(jobId: "JobB", taskId: "2021111300005")}");
        }
    }
}
