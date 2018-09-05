using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using BoolExprNet.Internal;

namespace BoolExprNet
{

    /// <summary>
    /// Represents a boolean expression.
    /// </summary>
    public abstract class Expression : ManagedRef
    {

        static readonly Dictionary<Kind, Constant> KIND2CONST = new Dictionary<Kind, Constant>()
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

        /// <summary>
        /// Describes the constant zero value.
        /// </summary>
        public static ZeroLiteral Zero = new ZeroLiteral(Native.boolexpr_zero());

        /// <summary>
        /// Describes the constant one value.
        /// </summary>
        public static OneLiteral One = new OneLiteral(Native.boolexpr_one());

        /// <summary>
        /// Describes the constant logical value.
        /// </summary>
        public static Logical Logical = new Logical(Native.boolexpr_logical());

        /// <summary>
        /// Describes the constant illogical value.
        /// </summary>
        public static Illogical Illogical = new Illogical(Native.boolexpr_illogical());

        #endregion

        #region Operator Methods

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
            if (ReferenceEquals(args, null))
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

        public static Expression ExclusiveOr(IEnumerable<Expression> args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return ExclusiveOr(args.ToArray());
        }

        public static Expression ExclusiveOr(params Expression[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            return InvokeExpr<Expression>(args, Native.boolexpr_xor);
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
            if (ReferenceEquals(p, null))
                throw new ArgumentNullException(nameof(p));
            if (ReferenceEquals(q, null))
                throw new ArgumentNullException(nameof(q));

            return InvokeExpr<Expression>(p, q, Native.boolexpr_nimpl);
        }

        public static Expression Implies(Expression p, Expression q)
        {
            if (ReferenceEquals(p, null))
                throw new ArgumentNullException(nameof(p));
            if (ReferenceEquals(q, null))
                throw new ArgumentNullException(nameof(q));

            return InvokeExpr<Expression>(p, q, Native.boolexpr_impl);
        }

        public static Expression NotIfThenElse(Expression s, Expression d1, Expression d0)
        {
            if (ReferenceEquals(s, null))
                throw new ArgumentNullException(nameof(s));
            if (ReferenceEquals(d1, null))
                throw new ArgumentNullException(nameof(d1));
            if (ReferenceEquals(d0, null))
                throw new ArgumentNullException(nameof(d0));

            return InvokeExpr<Expression>(s, d1, d0, Native.boolexpr_nite);
        }

        public static Expression IfThenElse(Expression s, Expression d1, Expression d0)
        {
            if (ReferenceEquals(s, null))
                throw new ArgumentNullException(nameof(s));
            if (ReferenceEquals(d1, null))
                throw new ArgumentNullException(nameof(d1));
            if (ReferenceEquals(d0, null))
                throw new ArgumentNullException(nameof(d0));

            return InvokeExpr<Expression>(s, d1, d0, Native.boolexpr_ite);
        }

        #endregion

        #region Operator Overloads

        /// <summary>
        /// Applies the logical AND operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Expression operator &(Expression a, Expression b)
        {
            return And(a, b);
        }

        /// <summary>
        /// Applies the logical OR operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Expression operator |(Expression a, Expression b)
        {
            return Or(a, b);
        }

        /// <summary>
        /// Applies the logical NOT operator.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Expression operator !(Expression a)
        {
            return Not(a);
        }

        /// <summary>
        /// Applies the logical XOR operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Expression operator ^(Expression a, Expression b)
        {
            return ExclusiveOr(a, b);
        }

        /// <summary>
        /// Applies the logical EQ operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Expression operator ==(Expression a, Expression b)
        {
            return Equal(a, b);
        }

        /// <summary>
        /// Applies the logical NEQ operator.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Expression operator !=(Expression a, Expression b)
        {
            return NotEqual(a, b);
        }

        #endregion

        #region Tracking

        /// <summary>
        /// Creates a new <see cref="Expression"/> from the given pointer.
        /// </summary>
        /// <param name="cbx"></param>
        /// <returns></returns>
        internal static T FromPtr<T>(IntPtr cbx)
            where T : Expression
        {
            var kind = (Kind)Native.boolexpr_BoolExpr_kind(cbx);

            // expression kind is a constant, create instance
            if (KIND2CONST.ContainsKey(kind))
            {
                Native.boolexpr_BoolExpr_del(cbx);
                return (T)(Expression)KIND2CONST[kind];
            }

            // expression kind is a literal, create instance
            if (KIND2LIT.ContainsKey(kind))
            {
                var ctx = Context.GetContext(Native.boolexpr_Literal_ctx(cbx));
                if (ctx is null)
                    throw new InvalidOperationException();

                // get or create literal
                var lit = (T)(Expression)ctx.GetOrCreateLiteral(Native.boolexpr_Literal_id(cbx), () => KIND2LIT[kind](cbx));
                if (lit is null)
                    throw new InvalidOperationException();

                // existing literal returned, free pointer to new
                if (lit.Ptr != cbx)
                    Native.boolexpr_BoolExpr_del(cbx);

                return lit;
            }

            // expression kind is an operator, create new instance
            if (KIND2OP.ContainsKey(kind))
                return (T)(Expression)KIND2OP[kind](cbx);

            throw new InvalidOperationException();
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

        #endregion

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

        /// <summary>
        /// Return the Tseytin transformation.
        /// </summary>
        /// <param name="ctx">The 'ctx' parameter is a <see cref="Context "/> object that will be used tostore
        /// auxiliary variables.</param>
        /// <param name="auxvarname">The 'auxvarname' parameter is the prefix of auxiliary variable names. The suffix
        /// will be in the form '_0', '_1', etc.</param>
        /// <returns></returns>
        public Expression Tseytin(Context ctx, string auxvarname = "a")
        {
            return FromPtr<Expression>(Native.boolexpr_BoolExpr_tseytin(Ptr, ctx.Ptr, auxvarname));
        }

        /// <summary>
        /// Substitute a subset of support variables with other Boolean expressions.
        /// </summary>
        /// <param name="var2bx"></param>
        /// <returns></returns>
        public Expression Compose(IEnumerable<KeyValuePair<Variable, Expression>> var2bx)
        {
            throw new NotImplementedException();
        }

        public Expression Restrict(IEnumerable<KeyValuePair<Variable, Constant>> point)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a satisying input point if the expression is satisfiable. If the expression is not satisfiable,
        /// the point will be <c>null</c>.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyDictionary<Variable, Constant> Satisify()
        {
            // attempt to satisfy
            var sats = Native.boolexpr_BoolExpr_sat(Ptr);
            if (sats == IntPtr.Zero)
                throw new InvalidOperationException();

            // acquire solution information
            var eval = Native.boolexpr_Soln_first(sats);
            var soln = Native.boolexpr_Soln_second(sats);
            if (soln == IntPtr.Zero)
                throw new InvalidOperationException();

            // release solution
            Native.boolexpr_Soln_del(sats);

            // return solution
            return eval ? new Point(soln) : null;
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

        /// <summary>
        /// Returns the support set of the <see cref="Expression"/>.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<Variable> Support()
        {
            return new VarSet(Native.boolexpr_BoolExpr_support(Ptr));
        }

        /// <summary>
        /// Return the degree of a function.
        /// </summary>
        public uint Degree => Native.boolexpr_BoolExpr_degree(Ptr);

        /// <summary>
        /// Return the Shannon expansion with respect to a sequence of variables.
        /// </summary>
        /// <param name="xs"></param>
        /// <returns></returns>
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

        public void Dfs()
        {
            throw new NotImplementedException();
        }

        public void Domain()
        {
            throw new NotImplementedException();
        }

        public void Cofactor(params Variable[] xs)
        {
            throw new NotImplementedException();
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

        public override int GetHashCode()
        {
            return Ptr.GetHashCode();
        }

    }

}
