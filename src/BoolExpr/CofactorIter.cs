using System;
using System.Collections;
using System.Collections.Generic;

using BoolExprNet.Internal;

namespace BoolExprNet
{

    public class CofactorIter :
        IEnumerable<Point>
    {

        readonly Func<IntPtr> iter;

        /// <summary>
        /// Initializes a new instnace.
        /// </summary>
        /// <param name="iter"></param>
        internal CofactorIter(Func<IntPtr> iter)
        {
            this.iter = iter ?? throw new ArgumentNullException(nameof(iter));
        }

        public IEnumerator<Point> GetEnumerator()
        {
            return new _DomainIter(iter());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

}
