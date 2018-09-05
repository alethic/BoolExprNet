using System;

using BoolExprNet.Internal;
using BoolExprNet.Util;

namespace BoolExprNet
{

    /// <summary>
    /// Describes a context within which one can allocate variables.
    /// </summary>
    public class Context : ManagedRef
    {

        /// <summary>
        /// Cache of <see cref="IntPtr"/> to <see cref="Context"/> instance.
        /// </summary>
        static readonly ConcurrentWeakDictionary<IntPtr, Context> cache =
            new ConcurrentWeakDictionary<IntPtr, Context>();

        /// <summary>
        /// Cache of <see cref="Literal"/> instances by ID.
        /// </summary>
        static readonly ConcurrentWeakDictionary<uint, Literal> literals =
            new ConcurrentWeakDictionary<uint, Literal>();

        /// <summary>
        /// Gets the <see cref="Context"/> instance associated with the specified pointer.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        internal static Context GetContext(IntPtr ptr)
        {
            return cache.GetOrAdd(ptr, new Context(ptr));
        }

        /// <summary>
        /// Gets the <see cref="Literal"/> instance associated with the specified ID or invokes the <paramref
        /// name="create"/> method to generate it on demand.
        /// </summary>
        /// <param name="ptr"></param>
        /// <returns></returns>
        internal Literal GetOrCreateLiteral(uint id, Func<Literal> create)
        {
            return literals.GetOrAdd(id, _ => create());
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public Context() : base(Native.boolexpr_Context_new())
        {
            cache[Ptr] = this;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        Context(IntPtr ptr) : base(ptr)
        {

        }

        /// <summary>
        /// Gets the <see cref="Variable"/> of the specified name from the <see cref="Context"/> or creates a new one.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Variable GetVariable(string name)
        {
            return Expression.FromPtr<Variable>(Native.boolexpr_Context_get_var(Ptr, name));
        }

        /// <summary>
        /// Gets a variable with the specified name and returns it.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="var"></param>
        /// <returns></returns>
        public Context Variable(string name, out Variable var)
        {
            var = GetVariable(name);
            return this;
        }

        protected override void Free(IntPtr ptr)
        {
            Native.boolexpr_Context_del(ptr);
        }

    }

}
