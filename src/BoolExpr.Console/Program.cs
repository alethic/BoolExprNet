namespace BoolExprNet.Console
{

    public static class Program
    {

        public static void Main(string[] args)
        {
            var ctx = new Context();

            var a1 = ctx.GetVariable("a1");
            var a2 = ctx.GetVariable("a2");

            System.Console.WriteLine(a1);
            System.Console.WriteLine(a2);

            System.Console.ReadLine();
        }

    }
}
