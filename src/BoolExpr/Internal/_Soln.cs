using System;

namespace BoolExprNet.Internal
{

    /// <summary>
    /// Provides a managed wrapper over the SOLN type.
    /// </summary>
    class _Soln : ManagedRef
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        public _Soln(IntPtr ptr) :
            base(ptr)
        {

        }

        public bool Satisfiable => Native.boolexpr_Soln_first(Ptr);

        public Point Point => !Satisfiable ? null : new Point(() => Native.boolexpr_Soln_second(Ptr));

        public (bool, Point) Tuple => (Satisfiable, Point);

        protected override void Free(IntPtr ptr)
        {
            Native.boolexpr_Soln_del(ptr);
        }

    }

}