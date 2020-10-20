using System;

using BoolExprNet.Internal;

namespace BoolExprNet
{

    public abstract class Literal : Atom
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        internal Literal(IntPtr ptr) : base(ptr)
        {

        }

        public Expression Abs => FromPtr<Expression>(Native.boolexpr_abs(Ptr));

    }

}
