using System;
using System.Collections.Generic;

namespace BoolExprNet
{

    public class ZeroLiteral : Known
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        internal ZeroLiteral(IntPtr ptr) : base(ptr)
        {

        }

        public override IEnumerable<object> ToAst()
        {
            yield return Kind;
        }

    }

}
