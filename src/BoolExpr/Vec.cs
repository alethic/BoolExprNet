using System;
using System.Collections;
using System.Collections.Generic;

namespace BoolExprNet
{

    /// <summary>
    /// VEC instance.
    /// </summary>
    struct Vec : IReadOnlyList<Expression>
    {

        readonly List<Expression> items;

        /// <summary>
        /// Initializes a new instnace.
        /// </summary>
        /// <param name="iter"></param>
        public Vec(IntPtr iter)
        {
            items = new List<Expression>();

            // begin iteration
            Native.boolexpr_Vec_iter(iter);

            while (true)
            {
                var expr = Native.boolexpr_Vec_val(iter);
                if (expr == IntPtr.Zero)
                    break;

                // add current and move next
                items.Add(Expression.FromPtr<Expression>(expr));
                Native.boolexpr_Vec_next(iter);
            }

            Native.boolexpr_Vec_del(iter);
        }

        public Expression this[int index] => items[index];

        public int Count => items.Count;

        public IEnumerator<Expression> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }

}
