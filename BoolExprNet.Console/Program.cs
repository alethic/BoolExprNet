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

            var f = Equal(IfThenElse(Not(Or(And(a, b, Not(c)), And(a, Not(b), c), And(Not(a), b, c))), b, c), z).ToDnf().ToCnf().ToDnf();
            System.Console.WriteLine(f);
            System.Console.ReadLine();
        }

    }

}
