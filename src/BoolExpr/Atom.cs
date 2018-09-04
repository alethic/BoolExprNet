using System;

namespace BoolExprNet
{

    public class Atom : Expression
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        public Atom(IntPtr ptr) : base(ptr)
        {

        }

    }

}
