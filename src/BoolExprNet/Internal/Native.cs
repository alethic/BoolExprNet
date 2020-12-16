using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BoolExprNet.Internal
{

    /// <summary>
    /// Provides the native method calls.
    /// </summary>
    static class Native
    {

        public const string LIB_NAME = "boolexpr";

        /// <summary>
        /// Native Win32 LoadLibrary method.
        /// </summary>
        /// <param name="dllToLoad"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        /// <summary>
        /// Native Linux library loader method.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        [DllImport("dl")]
        static extern IntPtr dlopen(string path, int flag);

        /// <summary>
        /// Initializes the static instance.
        /// </summary>
        static Native()
        {
#if NET5_0 || NETCOREAPP3_0
            NativeLibrary.SetDllImportResolver(typeof(Native).Assembly, DllImportResolver);
#else
            LegacyImportDll();
#endif
        }

        /// <summary>
        /// Invokes the appropriate LoadLibrary function based on the platform.
        /// </summary>
        /// <param name="dllToLoad"></param>
        /// <returns></returns>
        static IntPtr LoadLibraryFunc(string dllToLoad)
        {
#if NET46
            return LoadLibrary(dllToLoad);
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return LoadLibrary(dllToLoad);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return dlopen(dllToLoad, 2);
#endif

            throw new NotSupportedException();
        }

        /// <summary>
        /// Preloads the native DLL for down-level platforms.
        /// </summary>
        static void LegacyImportDll()
        {
#if NET46
            // attempt to load with default loader
            var h = LoadLibrary(LIB_NAME);
            if (h != IntPtr.Zero)
                return;

            // scan known paths
            foreach (var path in GetLibraryPaths(LIB_NAME))
            {
                h = LoadLibrary(path);
                if (h != IntPtr.Zero)
                    return;
            }
#elif NETCOREAPP2_0
            // attempt to load with default loader
            var h = LoadLibraryFunc(LIB_NAME);
            if (h != IntPtr.Zero)
                return;

            // scan known paths
            foreach (var path in GetLibraryPaths(LIB_NAME))
            {
                h = LoadLibraryFunc(path);
                if (h != IntPtr.Zero)
                    return;
            }
#endif
        }

#if NET5_0 || NETCOREAPP3_0

        /// <summary>
        /// Attempts to resolve the specified assembly when running on .NET Core 3 and above.
        /// </summary>
        /// <param name="libraryName"></param>
        /// <param name="assembly"></param>
        /// <param name="searchPath"></param>
        /// <returns></returns>
        static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName == LIB_NAME)
            {
                // attempt to load with default loader
                if (NativeLibrary.TryLoad(libraryName, out var h) && h != IntPtr.Zero)
                    return h;

                // scan known paths
                foreach (var path in GetLibraryPaths(libraryName))
                    if (NativeLibrary.TryLoad(path, out h) && h != IntPtr.Zero)
                        return h;
            }

            return IntPtr.Zero;
        }

#endif

        /// <summary>
        /// Gets the RID architecture.
        /// </summary>
        /// <returns></returns>
        static string GetRuntimeIdentifierArch()
        {
            switch (Marshal.SizeOf<IntPtr>())
            {
                case 4:
                    return "x86";
                case 8:
                    return "x64";
                default:
                    break;
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the runtime identifiers of the current platform.
        /// </summary>
        /// <returns></returns>
        static IEnumerable<string> GetRuntimeIdentifiers()
        {
            var arch = GetRuntimeIdentifierArch();

#if NET46
            yield return $"win-{arch}";
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                yield return $"win-{arch}";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                yield return $"linux-{arch}";

#if NET5_0 || NETCOREAPP3_0
            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                yield return $"freebsd-{arch}";
#endif
#endif
        }

        /// <summary>
        /// Gets the appropriate 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static string GetLibraryFileName(string name)
        {
#if NET46
            return $"{name}.dll";
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return $"{name}.dll";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return $"lib{name}.so";

#if NET5_0 || NETCOREAPP3_0
            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                return $"lib{name}.so";
#endif
#endif

            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets some library paths to check.
        /// </summary>
        /// <returns></returns>
        static IEnumerable<string> GetLibraryPaths(string name)
        {
            var self = Directory.GetParent(typeof(Native).Assembly.Location)?.FullName;
            if (self == null)
                yield break;

            var file = GetLibraryFileName(name);

            // search in runtime specific directories
            foreach (var rid in GetRuntimeIdentifiers())
                yield return Path.Combine(self, "runtimes", rid, "native", file);
        }

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
