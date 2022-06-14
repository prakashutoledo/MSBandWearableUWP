using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Threading;

namespace IDEASLabUT.MSBandWearable.Test
{
    /// <summary>
    /// Class that provides await latch for testing values that gets updated asynchronously 
    /// </summary>
    public class AwaitableTest
    {
        private EventWaitHandle awaitLatch;

        [TestInitialize]
        public void InitializeAwaitable()
        {
            awaitLatch = new AutoResetEvent(false);
        }
        
        /// <summary>
        /// Sets the await latch to send signal after apply given action
        /// </summary>
        /// <param name="action"></param>
        protected void ApplyLatch(Action action = null)
        {
            action?.Invoke();
            awaitLatch.Set();
        }

        /// <summary>
        /// Wait for await latch to receive the signals which blocks the current thread
        /// </summary>
        protected void WaitFor()
        {
            awaitLatch.WaitOne();
        }

        [TestCleanup]
        public void CleanupAwaitable()
        {
            awaitLatch = null;
        }
    }
}
