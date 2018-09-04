using System;

namespace BoolExprNet
{

    /// <summary>
    /// 
    /// </summary>
    public class Context : ManagedRef
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public Context() : base(Native.boolexpr_Context_new())
        {

        }

        public Variable GetVariable(string name)
        {
            return Expression.FromPtr<Variable>(Native.boolexpr_Context_get_var(Ptr, name));
        }

        protected override void Free(IntPtr ptr)
        {
            Native.boolexpr_Context_del(ptr);
        }

    }

}
