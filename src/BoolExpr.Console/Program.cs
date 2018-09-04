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
            var o = (Operator)f;

            foreach (var arg in o.Args)
                System.Console.WriteLine(arg.Kind);

            System.Console.ReadLine();
        }
    }
}
