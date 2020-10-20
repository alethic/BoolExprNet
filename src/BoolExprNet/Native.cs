using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace BoolExprNet
{

    static class Native
    {

        const string LIB_NAME = "boolexpr.dll";

        /// <summary>
        /// Initializes the static type.
        /// </summary>
        static Native()
        {
            LoadLibLibrary();
        }

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        /// <summary>
        /// Attempts to load the native library from various paths.
        /// </summary>
        /// <returns></returns>
        static IntPtr LoadLibLibrary()
        {
            foreach (var path in GetLibPaths())
                if (File.Exists(path))
                    if (LoadLibrary(path) is IntPtr ptr && ptr != IntPtr.Zero)
                        return ptr;

            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets some library paths the assembly might be located in.
        /// </summary>
        /// <returns></returns>
        static IEnumerable<string> GetLibPaths()
        {
            var self = Directory.GetParent(typeof(Native).Assembly.Location)?.FullName;
            if (self == null)
                yield break;

            switch (Marshal.SizeOf<IntPtr>())
            {
                case 4:
                    yield return Path.Combine(self, $@"runtimes\win7-x86\native\{LIB_NAME}");
                    yield return Path.Combine(self, $@"runtimes\win-x86\native\{LIB_NAME}");
                    yield return Path.Combine(self, $@"x86\{LIB_NAME}");
                    break;
                case 8:
                    yield return Path.Combine(self, $@"runtimes\win7-x64\native\{LIB_NAME}");
                    yield return Path.Combine(self, $@"runtimes\win-x64\native\{LIB_NAME}");
                    yield return Path.Combine(self, $@"x64\{LIB_NAME}");
                    break;
                default:
                    throw new NotSupportedException("Unknown OS architecture.");
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibrary(string dllToLoad);

        #region Context

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_Context_new();

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_Context_del(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_Context_get_var(IntPtr c_self, string c_name);

        #endregion

        #region String

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_String_del(IntPtr c_str);

        #endregion

        #region Vec

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_Vec_iter(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_Vec_next(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_Vec_val(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_Vec_del(IntPtr c_self);

        #endregion

        #region VarSet

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_VarSet_iter(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_VarSet_next(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_VarSet_val(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_VarSet_del(IntPtr c_self);

        #endregion

        #region Point

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_Point_iter(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_Point_next(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_Point_key(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_Point_val(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_Point_del(IntPtr c_self);

        #endregion

        #region Soln

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool boolexpr_Soln_first(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_Soln_second(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_Soln_del(IntPtr c_self);

        #endregion

        #region DfsIter

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_DfsIter_new(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_DfsIter_next(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_DfsIter_val(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_DfsIter_del(IntPtr c_self);

        #endregion

        #region DomainIter

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_DomainIter_new(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_DomainIter_next(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_DomainIter_val(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_DomainIter_del(IntPtr c_self);

        #endregion

        #region CofactorIter

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_CofactorIter_new(IntPtr c_self, int size, IntPtr c_vars);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_CofactorIter_del(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_CofactorIter_next(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_CofactorIter_val(IntPtr c_self);

        #endregion

        #region BoolExpr

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_del(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_to_string(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte boolexpr_BoolExpr_kind(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint boolexpr_BoolExpr_depth(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint boolexpr_BoolExpr_size(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool boolexpr_BoolExpr_is_cnf(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool boolexpr_BoolExpr_is_dnf(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_simplify(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_to_binop(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_to_latop(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_to_posop(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_tseytin(IntPtr c_self, IntPtr c_ctx, string c_auxvarname);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_compose(IntPtr c_self, int size, IntPtr c_vars, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_restrict(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_sat(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_to_cnf(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_to_dnf(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_to_nnf(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool boolexpr_BoolExpr_equiv(IntPtr c_self, IntPtr c_other);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_support(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint boolexpr_BoolExpr_degree(IntPtr c_self);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_expand(IntPtr c_self, int size, IntPtr c_vars);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_smoothing(IntPtr c_self, int size, IntPtr c_vars);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_consensus(IntPtr c_self, int size, IntPtr c_vars);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_BoolExpr_derivative(IntPtr c_self, int size, IntPtr c_vars);

        #endregion

        #region Literal

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_Literal_ctx(IntPtr c_bx);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint boolexpr_Literal_id(IntPtr c_bx);

        #endregion

        #region Operator

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool boolexpr_Operator_simple(IntPtr c_bx);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_Operator_args(IntPtr c_bx);    

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool boolexpr_Operator_is_clause(IntPtr c_bx);

        #endregion

        #region Expressions

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_zero();

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_one();

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_logical();

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_illogical();

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_not(IntPtr c_bx);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_abs(IntPtr c_lit);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_nor(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_or(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_nand(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_and(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_xnor(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_xor(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_neq(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_eq(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_nimpl(IntPtr c_bx1, IntPtr c_bx2);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_impl(IntPtr c_bx1, IntPtr c_bx2);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_nite(IntPtr c_bx1, IntPtr c_bx2, IntPtr c_bx3);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_ite(IntPtr c_bx1, IntPtr c_bx2, IntPtr c_bx3);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_onehot0(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_onehot(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_nor_s(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_or_s(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_nand_s(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_and_s(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_xnor_s(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_xor_s(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_neq_s(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_eq_s(int size, IntPtr c_bxs);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_nimpl_s(IntPtr c_bx1, IntPtr c_bx2);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_impl_s(IntPtr c_bx1, IntPtr c_bx2);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_nite_s(IntPtr c_bx1, IntPtr c_bx2, IntPtr c_bx3);

        [DllImport(LIB_NAME, CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_ite_s(IntPtr c_bx1, IntPtr c_bx2, IntPtr c_bx3);

        #endregion

    }

}
