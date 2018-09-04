using System;
using System.Collections;
using System.Collections.Generic;

namespace BoolExprNet.Internal
{

    /// <summary>
    /// Provides a managed iterator over the DOMAIN_ITER type.
    /// </summary>
    class _DomainIter : ManagedRef, IEnumerator<Point>
    {

        Point currentVar;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        public _DomainIter(IntPtr ptr) :
            base(ptr)
        {

        }

        public Point Current => currentVar;

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            throw new NotImplementedException();

            //// zero out existing values
            //currentVar = null;

            //// advance next
            //Native.boolexpr_VarSet_next(Ptr);

            //// check and store
            //var var = Native.boolexpr_VarSet_val(Ptr);
            //if (var != IntPtr.Zero)
            //{
            //    currentVar = (Variable)Expression.FromPtr(var);
            //    return true;
            //}

            //return false;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        protected override void Free(IntPtr ptr)
        {
            Native.boolexpr_DomainIter_del(ptr);
        }

    }

}