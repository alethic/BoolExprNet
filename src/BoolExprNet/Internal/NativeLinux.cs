using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace BoolExprNet.Internal
{

    /// <summary>
    /// Provides native library management for Linux.
    /// </summary>
    static class NativeLinux
    {

        /// <summary>
        /// Initializes the native libraries for Linux.
        /// </summary>
        public static void Init()
        {
            LoadLibLibrary();
        }

        /// <summary>
        /// Attempts to load the native library from various paths.
        /// </summary>
        /// <returns></returns>
        static IntPtr LoadLibLibrary()
        {
            foreach (var path in GetLinuxLibPaths())
                if (File.Exists(path))
                    if (LoadLibrary(path) is IntPtr ptr && ptr != IntPtr.Zero)
                        return ptr;

            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets some library paths the assembly might be located in.
        /// </summary>
        /// <returns></returns>
        static IEnumerable<string> GetLinuxLibPaths()
        {
            var self = Directory.GetParent(typeof(NativeWindows).Assembly.Location)?.FullName;
            if (self == null)
                yield break;

            switch (Marshal.SizeOf<IntPtr>())
            {
                case 4:
                    yield return Path.Combine(self, "runtimes", "linux-x86", "native", $"{Native.LIB_NAME}.so");
                    yield return Path.Combine(self, "x86", $"{Native.LIB_NAME}.so");
                    break;
                case 8:
                    yield return Path.Combine(self, "runtimes", "linux-x64", "native", $"{Native.LIB_NAME}.so");
                    yield return Path.Combine(self, "x64", $"{Native.LIB_NAME}.so");
                    break;
                default:
                    throw new NotSupportedException("Unknown architecture.");
            }
        }

        static IntPtr LoadLibrary(string libToLoad)
        {
            throw new NotImplementedException();
        }

    }

}
