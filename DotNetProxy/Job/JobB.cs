using System.Collections.Generic;
using System.Linq;

namespace DotNetProxy
{
    public class JobB : Job<JobB.Argument, JobB.Result>
    {
        public class Argument
        {
            public int A { get; set; }
            public int B { get; set; }
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

        public JobB(ILog log)
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
