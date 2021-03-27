using FluentAssertions;

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

        [TestMethod]
        public void Should_not_return_null_when_using_zero_literal()
        {
            using var ctx = new Context();
            var v = Zero;
            var f = Not(v);
            f.Should().NotBeNull();
        }

        [TestMethod]
        public void Should_not_return_null_when_using_one_literal()
        {
            using var ctx = new Context();
            var v = One;
            var f = Not(v);
            f.Should().NotBeNull();
        }

    }
}
