using System;
using System.Collections;
using System.Collections.Generic;

using BoolExprNet.Internal;

namespace BoolExprNet
{

    public class Vec :
        IEnumerable<Expression>
    {

        readonly Func<IntPtr> iter;

        /// <summary>
        /// Initializes a new instnace.
        /// </summary>
        /// <param name="iter"></param>
        internal Vec(Func<IntPtr> iter)
        {
            this.iter = iter ?? throw new ArgumentNullException(nameof(iter));
        }

        public IEnumerator<Expression> GetEnumerator()
        {
            return new _VecIter(iter());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
