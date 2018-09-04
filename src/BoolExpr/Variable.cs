using System;
using System.Collections.Generic;

namespace BoolExprNet
{

    public class Variable : Literal
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        internal Variable(IntPtr ptr) : base(ptr)
        {

        }

        public override IEnumerable<object> ToAst()
        {
            throw new NotImplementedException();
        }

    }

}
