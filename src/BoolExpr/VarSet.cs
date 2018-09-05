using System;
using System.Collections;
using System.Collections.Generic;

namespace BoolExprNet
{

    /// <summary>
    /// VARSET instance.
    /// </summary>
    struct VarSet : IReadOnlyList<Variable>
    {

        readonly List<Variable> items;

        /// <summary>
        /// Initializes a new instnace.
        /// </summary>
        /// <param name="iter"></param>
        public VarSet(IntPtr iter)
        {
            items = new List<Variable>();

            // begin iteration
            Native.boolexpr_VarSet_iter(iter);

            while (true)
            {
                var expr = Native.boolexpr_VarSet_val(iter);
                if (expr == null)
                    break;

                // add current and move next
                items.Add(Expression.FromPtr<Variable>(expr));
                Native.boolexpr_VarSet_next(iter);
            }

            Native.boolexpr_VarSet_del(iter);
        }

        public Variable this[int index] => items[index];

        public int Count => items.Count;

        public IEnumerator<Variable> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }

}
