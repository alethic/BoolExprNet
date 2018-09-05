using System;
using System.Collections;
using System.Collections.Generic;

namespace BoolExprNet
{

    class Point : IReadOnlyDictionary<Variable, Constant>
    {

        readonly Dictionary<Variable, Constant> items;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="iter"></param>
        internal Point(IntPtr ptr)
        {
            items = new Dictionary<Variable, Constant>();
            Native.boolexpr_Point_iter(ptr);

            while (true)
            {
                var key = Native.boolexpr_Point_key(ptr);
                if (key == IntPtr.Zero)
                    break;

                items[Expression.FromPtr<Variable>(key)] = Expression.FromPtr<Constant>(Native.boolexpr_Point_val(ptr));
                Native.boolexpr_Point_next(ptr);
            }

            Native.boolexpr_Point_del(ptr);
        }

        public Constant this[Variable key] => items[key];

        public IEnumerable<Variable> Keys => items.Keys;

        public IEnumerable<Constant> Values => items.Values;

        public int Count => items.Count;

        public bool ContainsKey(Variable key) => items.ContainsKey(key);

        public IEnumerator<KeyValuePair<Variable, Constant>> GetEnumerator() => items.GetEnumerator();

        public bool TryGetValue(Variable key, out Constant value) => items.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }

}
