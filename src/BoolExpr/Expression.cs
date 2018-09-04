using System;
using System.Collections.Generic;
using System.Linq;

using BoolExprNet.Internal;

namespace BoolExprNet
{

    public abstract class Expression : ManagedRef
    {

        static readonly Dictionary<Kind, Expression> KIND2CONST = new Dictionary<Kind, Expression>()
        {
            [Kind.ZERO] = Zero,
            [Kind.ONE] = One,
            [Kind.LOG] = Logical,
            [Kind.ILL] = Illogical,
        };

        static readonly Dictionary<Kind, Func<IntPtr, Literal>> KIND2LIT = new Dictionary<Kind, Func<IntPtr, Literal>>()
        {
            [Kind.COMP] = cbx => new Complement(cbx),
            [Kind.VAR] = cbx => new Variable(cbx),
        };

        static readonly Dictionary<Kind, Func<IntPtr, Operator>> KIND2OP = new Dictionary<Kind, Func<IntPtr, Operator>>()
        {
            [Kind.NOR] = cbx => new NorOperator(cbx),
            [Kind.OR] = cbx => new OrOperator(cbx),
            [Kind.NAND] = cbx => new NandOperator(cbx),
            [Kind.AND] = cbx => new AndOperator(cbx),
            [Kind.XNOR] = cbx => new XnorOperator(cbx),
            [Kind.XOR] = cbx => new XorOperator(cbx),
            [Kind.NEQ] = cbx => new UnequalOperator(cbx),
            [Kind.EQ] = cbx => new EqualOperator(cbx),
            [Kind.NIMPL] = cbx => new NotImpliesOperator(cbx),
            [Kind.IMPL] = cbx => new Implies(cbx),
            [Kind.NITE] = cbx => new NotIfThenElse(cbx),
            [Kind.ITE] = cbx => new IfThenElse(cbx),
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
        static T InvokeExpr<T>(IEnumerable<Expression> args, Func<int, IntPtr, IntPtr> func)
            where T : Expression
        {
            return FromPtr<T>(WithPtrs(args, func));
        }

        public static Expression Not(Expression args)
        {
            return InvokeExpr<Expression>(args, Native.boolexpr_not);
        }

        public static Expression Nor(IEnumerable<Expression> args)
        {
            return InvokeExpr<Expression>(args, Native.boolexpr_nor);
        }

        public static Expression Nor(params Expression[] args)
        {
            return Nor(args.AsEnumerable());
        }

        public static Expression Or(IEnumerable<Expression> args)
        {
            return InvokeExpr<Expression>(args, Native.boolexpr_or);
        }

        public static Expression Or(params Expression[] args)
        {
            return Or(args.AsEnumerable());
        }

        public static Expression Nand(IEnumerable<Expression> args)
        {
            return InvokeExpr<Expression>(args, Native.boolexpr_nand);
        }

        public static Expression Nand(params Expression[] args)
        {
            return Nand(args.AsEnumerable());
        }

        public static Expression And(IEnumerable<Expression> args)
        {
            return InvokeExpr<Expression>(args, Native.boolexpr_and);
        }

        public static Expression And(params Expression[] args)
        {
            return And(args.AsEnumerable());
        }

        public static Expression Xnor(IEnumerable<Expression> args)
        {
            return InvokeExpr<Expression>(args, Native.boolexpr_xnor);
        }

        public static Expression Xnor(params Expression[] args)
        {
            return Xnor(args.AsEnumerable());
        }

        public static Expression Neq(IEnumerable<Expression> args)
        {
            return InvokeExpr<Expression>(args, Native.boolexpr_neq);
        }

        public static Expression Neq(params Expression[] args)
        {
            return Neq(args.AsEnumerable());
        }

        public static Expression Eq(IEnumerable<Expression> args)
        {
            return InvokeExpr<Expression>(args, Native.boolexpr_eq);
        }

        public static Expression Eq(params Expression[] args)
        {
            return Eq(args.AsEnumerable());
        }

        public static Expression Nimpl(Expression p, Expression q)
        {
            return InvokeExpr<Expression>(p, q, Native.boolexpr_nimpl);
        }

        public static Expression Impl(Expression p, Expression q)
        {
            return InvokeExpr<Expression>(p, q, Native.boolexpr_impl);
        }

        public static Expression NotIfThenElse(Expression s, Expression d1, Expression d0)
        {
            return InvokeExpr<Expression>(s, d1, d0, Native.boolexpr_nite);
        }

        public static Expression IfThenElse(Expression s, Expression d1, Expression d0)
        {
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
        /// <param name="ptrs"></param>
        /// <param name="with"></param>
        /// <returns></returns>
        internal static unsafe T WithPtrs<T>(IEnumerable<ManagedRef> ptrs, Func<int, IntPtr, T> with)
        {
            var a = ptrs.Select(i => i.Ptr).ToArray();
            fixed (IntPtr* p = a)
                return with(a.Length, (IntPtr)p);
        }

        public static Expression FromAst(object ast)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="ptr"></param>
        protected Expression(IntPtr ptr) : base(ptr)
        {

        }

        protected override void Free(IntPtr ptr)
        {
            Native.boolexpr_BoolExpr_del(ptr);
        }

        public Kind Kind => (Kind)Native.boolexpr_BoolExpr_kind(Ptr);

        public uint Depth => Native.boolexpr_BoolExpr_depth(Ptr);

        public uint Size => Native.boolexpr_BoolExpr_size(Ptr);

        public bool IsCNF => Native.boolexpr_BoolExpr_is_cnf(Ptr);

        public bool IsDNF => Native.boolexpr_BoolExpr_is_dnf(Ptr);

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

        public Expression ToPosOp()
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

        public Expression ToCNF()
        {
            return FromPtr<Expression>(Native.boolexpr_BoolExpr_to_cnf(Ptr));
        }

        public Expression ToDNF()
        {
            return FromPtr<Expression>(Native.boolexpr_BoolExpr_to_dnf(Ptr));
        }

        public Expression ToNNF()
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

        public override string ToString()
        {
            return new _String(Native.boolexpr_BoolExpr_to_string(Ptr)).ToString();
        }

    }

}
