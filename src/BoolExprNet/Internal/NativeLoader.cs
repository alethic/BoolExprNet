using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace BoolExprNet.Internal
{

    /// <summary>
    /// Initializes the required native libraries.
    /// </summary>
    static class NativeLoader
    {

        /// <summary>
        /// Loads the specified library name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IntPtr Load(string name)
        {
            foreach (var path in GetLibraryPaths(name))
                if (File.Exists(path))
                    if (LoadNativeLibrary(path) is IntPtr ptr && ptr != IntPtr.Zero)
                        return ptr;

            return IntPtr.Zero;
        }

        /// <summary>
        /// Loads the specified librar.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static IntPtr LoadNativeLibrary(string path)
        {
#if NET46
            return NativeWindows.LoadLibrary(path);
#elif NETCOREAPP2_0
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return NativeWindows.LoadLibrary(path);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return NativeUnix.dlopen(path, 2);
#elif NETCOREAPP3_0
            return NativeLibrary.Load(path);
#endif

            throw new NotSupportedException();
        }

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

#if NETCOREAPP3_0
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

#if NETCOREAPP3_0
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
            var self = Directory.GetParent(typeof(NativeWindows).Assembly.Location)?.FullName;
            if (self == null)
                yield break;

            var file = GetLibraryFileName(name);

            // search in runtime specific directories
            foreach (var rid in GetRuntimeIdentifiers())
                yield return Path.Combine(self, "runtimes", rid, "native", file);
        }

    }

}
