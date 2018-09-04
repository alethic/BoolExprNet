using System;

namespace BoolExprNet
{

    public abstract class NegativeOperator : Operator
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        public NegativeOperator(IntPtr ptr) : base(ptr)
        {

        }

    }

}
