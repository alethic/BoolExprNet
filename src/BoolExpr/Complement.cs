using System;
using System.Collections.Generic;

namespace BoolExprNet
{

    public class Complement : Literal
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        internal Complement(IntPtr ptr) : base(ptr)
        {

        }

        public override IEnumerable<object> ToAst()
        {
            throw new NotImplementedException();
        }

    }

}
