namespace BoolExprNet.Console
{

    public static class Program
    {

        public static void Main(string[] args)
        {
            new Context()
                .Variable("a1", out var a1)
                .Variable("a2", out var a2)
                .Variable("b1", out var b1)
                .Variable("b2", out var b2);

            System.Console.WriteLine(a1);
            System.Console.WriteLine(a2);

            System.Console.WriteLine(a1 & b1 | a2 & b2);
            System.Console.WriteLine(!a1 & b1 | !a2 & b2);
            System.Console.ReadLine();
        }

    }
}
