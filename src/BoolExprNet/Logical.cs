using System;
using System.Collections.Generic;

namespace BoolExprNet
{

    public class Logical : Unknown
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        internal Logical(IntPtr ptr) : base(ptr)
        {

        }

        public override IEnumerable<object> ToAst()
        {
            yield return Kind;
        }

    }

}
