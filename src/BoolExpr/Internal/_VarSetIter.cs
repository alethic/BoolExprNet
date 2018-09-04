using System;
using System.Collections;
using System.Collections.Generic;

namespace BoolExprNet.Internal
{

    /// <summary>
    /// Provides a managed iterator over the VARSET type.
    /// </summary>
    class _VarSetIter : ManagedRef, IEnumerator<Variable>
    {

        bool init = true;
        bool done = false;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        internal _VarSetIter(IntPtr ptr) :
            base(ptr)
        {

        }

        public Variable Current { get; private set; }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            // prevent operations when at the end
            if (done)
                throw new InvalidOperationException();

            // zero out value before moving
            Current = null;

            if (init)
            {
                // initial initialization
                Native.boolexpr_VarSet_iter(Ptr);
                init = false;
            }
            else
            {
                // advance to next variable
                Native.boolexpr_VarSet_next(Ptr);
            }

            // check and store
            var expr = Native.boolexpr_VarSet_val(Ptr);
            if (expr != IntPtr.Zero)
            {
                Current = Expression.FromPtr<Variable>(expr);
                return true;
            }

            // set finished
            done = true;
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