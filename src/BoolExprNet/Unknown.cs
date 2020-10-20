using System;

namespace BoolExprNet
{

    public class Unknown : Constant
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        internal Unknown(IntPtr ptr) : base(ptr)
        {

        }

    }

}
