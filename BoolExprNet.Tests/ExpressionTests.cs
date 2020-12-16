using Microsoft.VisualStudio.TestTools.UnitTesting;

using static BoolExprNet.Expression;

namespace BoolExprNet.Tests
{

    [TestClass]
    public class TestTests
    {

        [TestMethod]
        public void Can_process_complex_expression()
        {
            using var ctx = new Context();
            var a = ctx.GetVariable("a");
            var b = ctx.GetVariable("b");
            var c = ctx.GetVariable("c");
            var z = ctx.GetVariable("z");

            var f = Equal(IfThenElse(Not(Or(And(a, b, Not(c)), And(a, Not(b), c), And(Not(a), b, c))), b, c), z).ToDnf().ToCnf().ToDnf().Simplify();
            var s = f.ToString();
        }

    }
}
