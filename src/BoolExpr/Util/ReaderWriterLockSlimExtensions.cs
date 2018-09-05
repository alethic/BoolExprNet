using System;
using System.Threading;

namespace BoolExprNet.Util
{

    /// <summary>
    /// Extension methods for the <see cref="ReaderWriterLockSlim"/> class.
    /// </summary>
    static class ReaderWriterLockSlimExtensions
    {

        struct DisposableReadLock :
            IDisposable
        {

            readonly ReaderWriterLockSlim sync;

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            /// <param name="sync"></param>
            public DisposableReadLock(ReaderWriterLockSlim sync)
            {
                this.sync = sync;
                this.sync.EnterReadLock();
            }

            public void Dispose()
            {
                sync.ExitReadLock();
            }

        }

        struct DisposableUpgradeableReadLock :
            IDisposable
        {

            readonly ReaderWriterLockSlim sync;

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            /// <param name="sync"></param>
            public DisposableUpgradeableReadLock(ReaderWriterLockSlim sync)
            {
                this.sync = sync;
                this.sync.EnterUpgradeableReadLock();
            }

            public void Dispose()
            {
                sync.ExitUpgradeableReadLock();
            }

        }

        struct DisposableWriteLock :
            IDisposable
        {

            readonly ReaderWriterLockSlim sync;

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            /// <param name="sync"></param>
            public DisposableWriteLock(ReaderWriterLockSlim sync)
            {
                this.sync = sync;
                this.sync.EnterWriteLock();
            }

            public void Dispose()
            {
                sync.ExitWriteLock();
            }

        }

        /// <summary>
        /// Begins a region of code that obtains a read lock.
        /// </summary>
        /// <param name="sync"></param>
        /// <returns></returns>
        public static IDisposable BeginReadLock(this ReaderWriterLockSlim sync)
        {
            if (sync == null)
                throw new ArgumentNullException(nameof(sync));

            return new DisposableReadLock(sync);
        }

        /// <summary>
        /// Begins a region of code that obtains an upgradable read lock.
        /// </summary>
        /// <param name="sync"></param>
        /// <returns></returns>
        public static IDisposable BeginUpgradableReadLock(this ReaderWriterLockSlim sync)
        {
            if (sync == null)
                throw new ArgumentNullException(nameof(sync));

            return new DisposableUpgradeableReadLock(sync);
        }

        /// <summary>
        /// Begins a region of code that obtains a write lock.
        /// </summary>
        /// <param name="sync"></param>
        /// <returns></returns>
        public static IDisposable BeginWriteLock(this ReaderWriterLockSlim sync)
        {
            if (sync == null)
                throw new ArgumentNullException(nameof(sync));

            return new DisposableWriteLock(sync);
        }

        /// <summary>
        /// Executes the given function within an ugradeable read lock.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sync"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T WithReadLock<T>(this ReaderWriterLockSlim sync, Func<T> func)
        {
            if (sync == null)
                throw new ArgumentNullException(nameof(sync));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            try
            {
                sync.EnterReadLock();
                return func();
            }
            finally
            {
                sync.ExitReadLock();
            }
        }

        /// <summary>
        /// Executes the given function within an ugradeable read lock.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sync"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static void WithReadLock(this ReaderWriterLockSlim sync, Action func)
        {
            if (sync == null)
                throw new ArgumentNullException(nameof(sync));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            try
            {
                sync.EnterReadLock();
                func();
            }
            finally
            {
                sync.ExitReadLock();
            }
        }

        /// <summary>
        /// Executes the given function within an ugradeable read lock.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sync"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T WithUpgradableReadLock<T>(this ReaderWriterLockSlim sync, Func<T> func)
        {
            if (sync == null)
                throw new ArgumentNullException(nameof(sync));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            try
            {
                sync.EnterUpgradeableReadLock();
                return func();
            }
            finally
            {
                sync.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Executes the given function within an ugradeable read lock.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sync"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static void WithUpgradableReadLock(this ReaderWriterLockSlim sync, Action func)
        {
            if (sync == null)
                throw new ArgumentNullException(nameof(sync));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            try
            {
                sync.EnterUpgradeableReadLock();
                func();
            }
            finally
            {
                sync.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Executes the given function within a write lock.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sync"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static void WithWriteLock(this ReaderWriterLockSlim sync, Action func)
        {
            if (sync == null)
                throw new ArgumentNullException(nameof(sync));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            try
            {
                sync.EnterWriteLock();
                func();
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

        /// <summary>
        /// Executes the given function within a write lock.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sync"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static T WithWriteLock<T>(this ReaderWriterLockSlim sync, Func<T> func)
        {
            if (sync == null)
                throw new ArgumentNullException(nameof(sync));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            try
            {
                sync.EnterWriteLock();
                return func();
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

    }

}
