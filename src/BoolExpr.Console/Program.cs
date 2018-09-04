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
            var d = ctx.GetVariable("d");
            var e = ctx.GetVariable("e");
            var f = ctx.GetVariable("f");
            var g = ctx.GetVariable("g");

            var f1 = Or(a, b);
            var f2 = Or(c, d);
            var f3 = Or(e, And(f, g));
            var fs = And(f1, f2, f3);

            fs = fs.ToDnf();

            System.Console.WriteLine(fs);
            System.Console.ReadLine();
        }

    }
}
