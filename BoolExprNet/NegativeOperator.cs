using System;

namespace BoolExprNet
{

    public abstract class NegativeOperator : Operator
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        internal NegativeOperator(IntPtr ptr) : base(ptr)
        {

        }

    }

}
