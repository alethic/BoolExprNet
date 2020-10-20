using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace BoolExprNet.Internal
{

    /// <summary>
    /// Provides native library management for Windows.
    /// </summary>
    static class NativeWindows
    {

        /// <summary>
        /// Initializes the native libraries for Windows.
        /// </summary>
        public static void Init()
        {
            LoadLibLibrary();
        }

        /// <summary>
        /// Loads the specified library into memory.
        /// </summary>
        /// <param name="hModule"></param>
        /// <param name="procName"></param>
        /// <returns></returns>
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        /// <summary>
        /// Attempts to load the native library from various paths.
        /// </summary>
        /// <returns></returns>
        static IntPtr LoadLibLibrary()
        {
            foreach (var path in GetWin32LibPaths())
                if (File.Exists(path))
                    if (LoadLibrary(path) is IntPtr ptr && ptr != IntPtr.Zero)
                        return ptr;

            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets some library paths the assembly might be located in.
        /// </summary>
        /// <returns></returns>
        static IEnumerable<string> GetWin32LibPaths()
        {
            var self = Directory.GetParent(typeof(NativeWindows).Assembly.Location)?.FullName;
            if (self == null)
                yield break;

            switch (Marshal.SizeOf<IntPtr>())
            {
                case 4:
                    yield return Path.Combine(self, $@"runtimes\win7-x86\native\{Native.LIB_NAME}.dll");
                    yield return Path.Combine(self, $@"runtimes\win-x86\native\{Native.LIB_NAME}.dll");
                    yield return Path.Combine(self, $@"x86\{Native.LIB_NAME}.dll");
                    break;
                case 8:
                    yield return Path.Combine(self, $@"runtimes\win7-x64\native\{Native.LIB_NAME}.dll");
                    yield return Path.Combine(self, $@"runtimes\win-x64\native\{Native.LIB_NAME}.dll");
                    yield return Path.Combine(self, $@"x64\{Native.LIB_NAME}.dll");
                    break;
                default:
                    throw new NotSupportedException("Unknown OS architecture.");
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibrary(string dllToLoad);

    }

}
