using System;
using System.Runtime.InteropServices;

namespace BoolExprNet.Internal
{

    /// <summary>
    /// Provides native library management for Windows.
    /// </summary>
    static class NativeUnix
    {

        /// <summary>
        /// Invokes the dlopen function.
        /// </summary>
        /// <param name="dllToLoad"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport("dl", SetLastError = true)]
        public static extern IntPtr dlopen(string dllToLoad, int flags);

    }

}