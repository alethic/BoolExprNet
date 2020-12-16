using System;
using System.Collections;
using System.Collections.Generic;

namespace BoolExprNet.Internal
{

    /// <summary>
    /// Provides a managed iterator over the POINT type.
    /// </summary>
    class _PointIter : ManagedRef, IEnumerator<KeyValuePair<Expression, Expression>>
    {

        Expression currentKey;
        Expression currentVal;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        internal _PointIter(IntPtr ptr) :
            base(ptr)
        {

        }

        public KeyValuePair<Expression, Expression> Current => new KeyValuePair<Expression, Expression>(currentKey, currentVal);

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            // zero out existing values
            currentKey = null;
            currentVal = null;

            // advance next
            Native.boolexpr_Point_next(Ptr);

            // check and store
            var key = Native.boolexpr_Point_key(Ptr);
            if (key != IntPtr.Zero)
            {
                currentKey = Expression.FromPtr<Expression>(key);
                currentVal = Expression.FromPtr<Expression>(Native.boolexpr_Point_val(Ptr));
                return true;
            }

            return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        protected override void Free(IntPtr ptr)
        {
            Native.boolexpr_Point_del(ptr);
        }

    }

}