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
        public Variable(IntPtr ptr) : base(ptr)
        {

        }

        public override IEnumerable<object> ToAst()
        {
            throw new NotImplementedException();
        }

    }

}
