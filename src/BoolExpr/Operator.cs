using System;
using System.Collections.Generic;

namespace BoolExprNet
{

    public abstract class Operator : Expression
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        internal Operator(IntPtr ptr) : base(ptr)
        {

        }

        public override IEnumerable<object> ToAst()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return <c>true</c> if the operator is simple.
        /// </summary>
        /// <remarks>
        /// An operator is only deemed simple if it has been returned by the <see cref="Expression.Simplify"/> method.
        /// </remarks>
        public bool Simple => Native.boolexpr_Operator_simple(Ptr);

        /// <summary>
        /// Return an enumeration of the operator's arguments.
        /// </summary>
        public IReadOnlyList<Expression> Args => new Vec(Native.boolexpr_Operator_args(Ptr));

        /// <summary>
        /// Return <c>true</c> if the operator is a clause.
        /// </summary>
        /// <remarks>
        /// A clause is defined as having only <see cref="Literal"/> arguments.
        /// </remarks>
        public bool IsClause => Native.boolexpr_Operator_is_clause(Ptr);

    }

}
