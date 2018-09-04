using System;
using System.Runtime.InteropServices;

namespace BoolExprNet.Internal
{

    /// <summary>
    /// Provides a managed wrapper over the STRING type.
    /// </summary>
    class _String : ManagedRef
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        public _String(IntPtr ptr) :
            base(ptr)
        {

        }

        protected override void Free(IntPtr ptr)
        {
            Native.boolexpr_String_del(ptr);
        }

        public override string ToString()
        {
            return Marshal.PtrToStringBSTR(Ptr);
        }

    }

}