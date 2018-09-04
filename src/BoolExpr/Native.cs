using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace BoolExpr
{

    public static class Native
    {

        const string LIB_NAME = "boolexpr.dll";

        /// <summary>
        /// Initializes the static type.
        /// </summary>
        static Native()
        {
            LoadLibLibrary();
        }

        /// <summary>
        /// Attempts to load the native library from various paths.
        /// </summary>
        /// <returns></returns>
        static bool LoadLibLibrary()
        {
            foreach (var path in GetLibPaths())
                if (File.Exists(path))
                    if (LoadLibrary(path) != IntPtr.Zero)
                        return true;

            return false;
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

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_Context_new();

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern void boolexpr_Context_del(IntPtr c_self);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_Context_get_var(IntPtr c_self, string c_name);

        [DllImport(LIB_NAME, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr boolexpr_zero();

    }

}
