using System;
using System.Collections;
using System.Collections.Generic;

namespace BoolExprNet.Internal
{

    /// <summary>
    /// Provides a managed iterator over the VEC type.
    /// </summary>
    class _Vec : ManagedRef, IEnumerator<Expression>
    {

        bool init = true;
        Expression current;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        public _Vec(IntPtr ptr) :
            base(ptr)
        {

        }

        public Expression Current => current;

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            // zero out value before moving
            current = null;

            // initialize on first access
            if (init)
            {
                Native.boolexpr_Vec_iter(Ptr);
                init = false;
            }
            else
            {
                // advance next
                Native.boolexpr_Vec_next(Ptr);
            }

            // check and store
            var expr = Native.boolexpr_Vec_val(Ptr);
            if (expr != IntPtr.Zero)
            {
                current = Expression.FromPtr<Expression>(expr);
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
            Native.boolexpr_Vec_del(ptr);
        }

    }

}