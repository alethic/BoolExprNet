using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using BoolExprNet.Internal;

namespace BoolExprNet
{

    public abstract class Expression : ManagedRef
    {

        static readonly Dictionary<Kind, Expression> KIND2CONST = new Dictionary<Kind, Expression>()
        {
            [Kind.Zero] = Zero,
            [Kind.One] = One,
            [Kind.Logical] = Logical,
            [Kind.Illogical] = Illogical,
        };

        static readonly Dictionary<Kind, Func<IntPtr, Literal>> KIND2LIT = new Dictionary<Kind, Func<IntPtr, Literal>>()
        {
            [Kind.Complement] = cbx => new Complement(cbx),
            [Kind.Variable] = cbx => new Variable(cbx),
        };

        static readonly Dictionary<Kind, Func<IntPtr, Operator>> KIND2OP = new Dictionary<Kind, Func<IntPtr, Operator>>()
        {
            [Kind.NotOr] = cbx => new NorOperator(cbx),
            [Kind.Or] = cbx => new OrOperator(cbx),
            [Kind.NotAnd] = cbx => new NandOperator(cbx),
            [Kind.And] = cbx => new AndOperator(cbx),
            [Kind.ExclusiveNotOr] = cbx => new XnorOperator(cbx),
            [Kind.ExclusiveOr] = cbx => new XorOperator(cbx),
            [Kind.NotEqual] = cbx => new UnequalOperator(cbx),
            [Kind.Equal] = cbx => new EqualOperator(cbx),
            [Kind.NotImplies] = cbx => new NotImpliesOperator(cbx),
            [Kind.Implies] = cbx => new Implies(cbx),
            [Kind.NotIfThenElse] = cbx => new NotIfThenElse(cbx),
            [Kind.IfThenElse] = cbx => new IfThenElse(cbx),
        };

        #region Known

        public static ZeroLiteral Zero = new ZeroLiteral(Native.boolexpr_zero());

        public static OneLiteral One = new OneLiteral(Native.boolexpr_one());

        public static Logical Logical = new Logical(Native.boolexpr_logical());

        public static Illogical Illogical = new Illogical(Native.boolexpr_illogical());

        #endregion

        #region Operators

        /// <summary>
        /// Invokes the given function accepting a single expression variable. Wraps the result in the specified
        /// <see cref="Expression"/> type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        static T InvokeExpr<T>(Expression arg, Func<IntPtr, IntPtr> func)
            where T : Expression
        {
            return FromPtr<T>(func(arg.Ptr));
        }

        /// <summary>
        /// Invokes the given function accepting two expressions variable. Wraps the result in the specified
        /// <see cref="Expression"/> type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        static T InvokeExpr<T>(Expression arg1, Expression arg2, Func<IntPtr, IntPtr, IntPtr> func)
            where T : Expression
        {
            return FromPtr<T>(func(arg1.Ptr, arg2.Ptr));
        }

        /// <summary>
        /// Invokes the given function accepting three expressions variable. Wraps the result in the specified
        /// <see cref="Expression"/> type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        static T InvokeExpr<T>(Expression arg1, Expression arg2, Expression arg3, Func<IntPtr, IntPtr, IntPtr, IntPtr> func)
            where T : Expression
        {
            return FromPtr<T>(func(arg1.Ptr, arg2.Ptr, arg3.Ptr));
        }

        /// <summary>
        /// Invokes the given function accepting a variable set of arguments in length/array format. Wraps the result
        /// in the specified <see cref="Expression"/> type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        static T InvokeExpr<T>(Expression[] args, Func<int, IntPtr, IntPtr> func)
            where T : Expression
        {
            return FromPtr<T>(WithPtrs(args, func));
        }

        public static Expression Not(Expression args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return InvokeExpr<Expression>(args, Native.boolexpr_not);
        }

        public static Expression NotOr(IEnumerable<Expression> args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return NotOr(args.ToArray());
        }

        public static Expression NotOr(params Expression[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return InvokeExpr<Expression>(args, Native.boolexpr_nor);
        }

        public static Expression Or(IEnumerable<Expression> args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return Or(args.ToArray());
        }

        public static Expression Or(params Expression[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return InvokeExpr<Expression>(args, Native.boolexpr_or);
        }

        public static Expression NotAnd(IEnumerable<Expression> args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return NotAnd(args.ToArray());
        }

        public static Expression NotAnd(params Expression[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return InvokeExpr<Expression>(args, Native.boolexpr_nand);
        }

        public static Expression And(IEnumerable<Expression> args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return And(args.ToArray());
        }

        public static Expression And(params Expression[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return InvokeExpr<Expression>(args, Native.boolexpr_and);
        }

        public static Expression ExclusiveNotOr(IEnumerable<Expression> args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return ExclusiveNotOr(args.ToArray());
        }

        public static Expression ExclusiveNotOr(params Expression[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return InvokeExpr<Expression>(args, Native.boolexpr_xnor);
        }

        public static Expression NotEqual(IEnumerable<Expression> args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return NotEqual(args.ToArray());
        }

        public static Expression NotEqual(params Expression[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return InvokeExpr<Expression>(args, Native.boolexpr_neq);
        }

        public static Expression Equal(IEnumerable<Expression> args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return Equal(args.ToArray());
        }

        public static Expression Equal(params Expression[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            if (args.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(args));

            return InvokeExpr<Expression>(args, Native.boolexpr_eq);
        }

        public static Expression NotImplies(Expression p, Expression q)
        {
            if (p == null)
                throw new ArgumentNullException(nameof(p));
            if (q == null)
                throw new ArgumentNullException(nameof(q));

            return InvokeExpr<Expression>(p, q, Native.boolexpr_nimpl);
        }

        public static Expression Implies(Expression p, Expression q)
        {
            if (p == null)
                throw new ArgumentNullException(nameof(p));
            if (q == null)
                throw new ArgumentNullException(nameof(q));

            return InvokeExpr<Expression>(p, q, Native.boolexpr_impl);
        }

        public static Expression NotIfThenElse(Expression s, Expression d1, Expression d0)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (d1 == null)
                throw new ArgumentNullException(nameof(d1));
            if (d0 == null)
                throw new ArgumentNullException(nameof(d0));

            return InvokeExpr<Expression>(s, d1, d0, Native.boolexpr_nite);
        }

        public static Expression IfThenElse(Expression s, Expression d1, Expression d0)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (d1 == null)
                throw new ArgumentNullException(nameof(d1));
            if (d0 == null)
                throw new ArgumentNullException(nameof(d0));

            return InvokeExpr<Expression>(s, d1, d0, Native.boolexpr_ite);
        }

        #endregion

        /// <summary>
        /// Creates a new <see cref="Expression"/> from the given pointer.
        /// </summary>
        /// <param name="cbx"></param>
        /// <returns></returns>
        internal static T FromPtr<T>(IntPtr cbx)
            where T : Expression
        {
            var kind = (Kind)Native.boolexpr_BoolExpr_kind(cbx);

            // expression kind is a constant, return cached instance
            if (KIND2CONST.ContainsKey(kind))
            {
                Native.boolexpr_BoolExpr_del(cbx);
                return (T)KIND2CONST[kind];
            }

            // expression kind is a literal, return cached instance
            if (KIND2LIT.ContainsKey(kind))
            {
                // try to look up literal within context somehow
                // can't do this until we can look up the context
                return (T)(Expression)KIND2LIT[kind](cbx);
            }

            return (T)(Expression)KIND2OP[kind](cbx);
        }

        /// <summary>
        /// Invokes the given function with the set of managed references converted to a pointer array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="refs"></param>
        /// <param name="with"></param>
        /// <returns></returns>
        internal static unsafe T WithPtrs<T>(ManagedRef[] refs, Func<int, IntPtr, T> with)
        {
            var a = refs.Select(i => i.Ptr).ToArray();
            fixed (IntPtr* p = a)
                return with(a.Length, (IntPtr)p);
        }

        /// <summary>
        /// Initializes a new instance from the specified AST.
        /// </summary>
        /// <param name="ast"></param>
        /// <returns></returns>
        public static Expression FromAst(object ast)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        internal Expression(IntPtr ptr) : base(ptr)
        {

        }

        protected override void Free(IntPtr ptr)
        {
            Native.boolexpr_BoolExpr_del(ptr);
        }

        public Kind Kind => (Kind)Native.boolexpr_BoolExpr_kind(Ptr);

        public uint Depth => Native.boolexpr_BoolExpr_depth(Ptr);

        public uint Size => Native.boolexpr_BoolExpr_size(Ptr);

        public bool IsCnf => Native.boolexpr_BoolExpr_is_cnf(Ptr);

        public bool IsDnf => Native.boolexpr_BoolExpr_is_dnf(Ptr);

        public Expression Simplify()
        {
            return FromPtr<Expression>(Native.boolexpr_BoolExpr_simplify(Ptr));
        }

        public Operator ToBinaryOperator()
        {
            return FromPtr<Operator>(Native.boolexpr_BoolExpr_to_binop(Ptr));
        }

        public LatticeOperator ToLatticeOperator()
        {
            return FromPtr<LatticeOperator>(Native.boolexpr_BoolExpr_to_latop(Ptr));
        }

        public Expression ToPositiveOperator()
        {
            return FromPtr<Expression>(Native.boolexpr_BoolExpr_to_posop(Ptr));
        }

        public Expression Tseytin(Context ctx = null, string auxvarname = "a")
        {
            throw new NotImplementedException();
        }

        public Expression Compose(IEnumerable<KeyValuePair<Variable, Expression>> var2bx)
        {
            throw new NotImplementedException();
        }

        public Expression Restrict(IEnumerable<KeyValuePair<Variable, Constant>> point)
        {
            throw new NotImplementedException();
        }

        public (bool, Point) Satisify()
        {
            return new _Soln(Native.boolexpr_BoolExpr_sat(Ptr)).Tuple;
        }

        public Expression ToCnf()
        {
            return FromPtr<Expression>(Native.boolexpr_BoolExpr_to_cnf(Ptr));
        }

        public Expression ToDnf()
        {
            return FromPtr<Expression>(Native.boolexpr_BoolExpr_to_dnf(Ptr));
        }

        public Expression ToNnf()
        {
            return FromPtr<Expression>(Native.boolexpr_BoolExpr_to_nnf(Ptr));
        }

        public bool Equiv(Expression other)
        {
            return Native.boolexpr_BoolExpr_equiv(Ptr, other.Ptr);
        }

        public VarSet Support()
        {
            return new VarSet(() => Native.boolexpr_BoolExpr_support(Ptr));
        }

        public uint Degree => Native.boolexpr_BoolExpr_degree(Ptr);

        public unsafe Expression Expand(params Variable[] xs)
        {
            fixed (IntPtr* c_vars = xs.Select(i => i.Ptr).ToArray())
                return FromPtr<Expression>(Native.boolexpr_BoolExpr_expand(Ptr, xs.Length, (IntPtr)c_vars));
        }

        public Expression Smoothing(params Variable[] xs)
        {
            throw new NotImplementedException();
        }

        public Expression Consensus(params Variable[] xs)
        {
            throw new NotImplementedException();
        }

        public Expression Derivative(params Variable[] xs)
        {
            throw new NotImplementedException();
        }

        public DfsIter IterateDfs()
        {
            return new DfsIter(() => Native.boolexpr_DfsIter_new(Ptr));
        }

        public DomainIter IterateDomain()
        {
            return new DomainIter(() => Native.boolexpr_DomainIter_new(Ptr));
        }

        public CofactorIter IterateCofactor(params Variable[] xs)
        {
            return new CofactorIter(() =>
            {
                unsafe
                {
                    fixed (IntPtr* c_vars = xs.Select(i => i.Ptr).ToArray())
                        return Native.boolexpr_CofactorIter_new(Ptr, xs.Length, (IntPtr)c_vars);
                }
            });
        }

        public virtual IEnumerable<object> ToAst()
        {
            throw new NotImplementedException();
        }

        public virtual Expression ToDot()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string representation of this expression.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var p = Native.boolexpr_BoolExpr_to_string(Ptr);
            var s = Marshal.PtrToStringAnsi(p);
            Native.boolexpr_String_del(p);
            return s;
        }

        /// <summary>
        /// Returns <c>true</c> if the two instances are equals.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            return obj is Expression e && Equals(e);
        }

        public bool Equals(Expression other)
        {
            return Equiv(other);
        }

    }

}
