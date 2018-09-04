using static BoolExprNet.Expression;

namespace BoolExprNet.Console
{

    public static class Program
    {

        public static void Main(string[] args)
        {
            var ctx = new Context();
            var a = ctx.GetVariable("a");
            var b = ctx.GetVariable("b");
            var c = ctx.GetVariable("c");
            var z = ctx.GetVariable("z");

            var f = 
                    Or(
                        And(a, Not(b), Not(c)),
                        And(Not(a), Not(b), c),
                        And(Not(a), b, Not(c)))
                .ToDnf().ToCnf();

            System.Console.WriteLine(f);
            System.Console.ReadLine();
        }

    }
}
