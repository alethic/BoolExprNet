﻿using System;
using System.Runtime.InteropServices;

namespace BoolExprNet.Internal
{

    /// <summary>
    /// Provides native library management for Windows.
    /// </summary>
    static class NativeWindows
    {

        /// <summary>
        /// Invokes the LoadLibrary function.
        /// </summary>
        /// <param name="dllToLoad"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string dllToLoad);

    }

}