using System;

namespace BoolExprNet
{

    public abstract class LatticeOperator : Operator
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        public LatticeOperator(IntPtr ptr) : base(ptr)
        {

        }

    }

}
