using System;
using System.Collections;
using System.Collections.Generic;

using BoolExprNet.Internal;

namespace BoolExprNet
{

    public class DfsIter :
        IEnumerable<Expression>
    {

        readonly Func<IntPtr> iter;

        /// <summary>
        /// Initializes a new instnace.
        /// </summary>
        /// <param name="iter"></param>
        internal DfsIter(Func<IntPtr> iter)
        {
            this.iter = iter ?? throw new ArgumentNullException(nameof(iter));
        }

        public IEnumerator<Expression> GetEnumerator()
        {
            return new _DfsIter(iter());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
