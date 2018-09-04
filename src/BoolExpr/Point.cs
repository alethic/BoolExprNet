using System;
using System.Collections;
using System.Collections.Generic;

using BoolExprNet.Internal;

namespace BoolExprNet
{

    public class Point :
        IEnumerable<KeyValuePair<Expression, Expression>>
    {

        readonly Func<IntPtr> iter;

        /// <summary>
        /// Initializes a new instnace.
        /// </summary>
        /// <param name="iter"></param>
        internal Point(Func<IntPtr> iter)
        {
            this.iter = iter ?? throw new ArgumentNullException(nameof(iter));
        }

        public IEnumerator<KeyValuePair<Expression, Expression>> GetEnumerator()
        {
            return new _Point(iter());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
