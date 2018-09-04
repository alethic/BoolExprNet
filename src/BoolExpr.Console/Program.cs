using System.Linq;
using static BoolExprNet.Expression;

namespace BoolExprNet.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new Context();
            var a = ctx.GetVariable("a");
            var b = ctx.GetVariable("b");
            var c = ctx.GetVariable("c");

            var f = Or(And(a, b, Not(c)));
            var f2 = f.ToDnf();
            var o = (Operator)f;

            var z = o.ToPositiveOperator();

            foreach (var arg in o.Args)
                System.Console.WriteLine(arg.Kind);

            var arg11 = o.Args.ToArray()[0];
            var arg12 = o.Args.ToArray()[0];

            System.Console.WriteLine(z);
            System.Console.ReadLine();
        }
    }
}
