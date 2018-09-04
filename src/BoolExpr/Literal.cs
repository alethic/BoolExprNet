using System;

namespace BoolExprNet
{

    public abstract class Literal : Atom
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        public Literal(IntPtr ptr) : base(ptr)
        {

        }

        public Expression Abs => FromPtr<Expression>(Native.boolexpr_abs(Ptr));

    }

}
