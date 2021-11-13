using System.Collections.Generic;
using System.Linq;

namespace DotNetProxy
{
    public class JobA : Job<JobA.Argument, JobA.Result>
    {
        public class Argument
        {
            public int A { get; set; }
            public int B { get; set; }
            public int C { get; set; }
        }

        public class Result
        {
            public int Sum { get; set; }

            public override string ToString()
            {
                return $"SUM = {Sum}";
            }
        }

        ILog log;

        public JobA(ILog log)
        {
            this.log = log;
        }

        public override Result Run(List<Argument> args)
        {
            log.Debug($"{GetType().Name} Run");

            return new Result { Sum = args.Select(x => x.A).Sum() };
        }
    }
}
