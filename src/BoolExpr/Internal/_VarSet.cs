using System;
using System.Collections;
using System.Collections.Generic;

namespace BoolExprNet.Internal
{

    /// <summary>
    /// Provides a managed iterator over the VARSET type.
    /// </summary>
    class _VarSet : ManagedRef, IEnumerator<Variable>
    {

        bool init = true;
        Variable current;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        public _VarSet(IntPtr ptr) :
            base(ptr)
        {

        }

        public Variable Current => current;

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            // zero out value before moving
            current = null;

            // initialize on first access
            if (init)
            {
                Native.boolexpr_VarSet_iter(Ptr);
                init = false;
            }
            else
            {
                // advance next
                Native.boolexpr_VarSet_next(Ptr);
            }

            // check and store
            var expr = Native.boolexpr_VarSet_val(Ptr);
            if (expr != IntPtr.Zero)
            {
                current = Expression.FromPtr<Variable>(expr);
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
            Native.boolexpr_VarSet_del(ptr);
        }

    }

}