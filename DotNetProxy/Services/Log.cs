using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetProxy
{
    public interface ILog
    {
        void Debug(string str);
    }

    public class Log : ILog
    {
        public void Debug(string str)
        {
            Console.WriteLine(str);
        }
    }
}
