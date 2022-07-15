/// Copyright 2022 IDEAS Lab @ University of Toledo. All rights reserved.
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Test
{
    /// <summary>
    /// Class that provides await latch for testing values that gets updated asynchronously 
    /// </summary>
    public class AwaitableTest
    {
        /// <summary>
        /// Private wrapper implementation of <see cref="TaskCompletionSource{bool}"/>
        /// </summary>
        private sealed class SingleTaskCompletionSource
        {
            private readonly TaskCompletionSource<bool> taskCompletionSource;
            private bool isSet;

            /// <summary>
            /// Creates a new instance of <see cref="SingleTaskCompletionSource"/>
            /// </summary>
            public SingleTaskCompletionSource()
            {
                taskCompletionSource = new TaskCompletionSource<bool>();
                isSet = false;
            }

            /// <summary>
            /// Sets the value of underlying task completion source to true
            /// </summary>
            public void Set()
            {
                isSet = true;
                taskCompletionSource.TrySetResult(isSet);
            }

            /// <summary>
            /// Waits for task created by underlying task completion source to complete if it has set the result.
            /// If no set is called before calling wait it will not actually wait
            /// </summary>
            /// <returns></returns>
            public Task WaitAsync()
            {
                if (!isSet)
                {
                    return Task.CompletedTask;
                }

                return taskCompletionSource.Task;
            }
        }

        private SingleTaskCompletionSource awaitLatch;

        [TestInitialize]
        public void InitializeAwaitable()
        {
            awaitLatch = new SingleTaskCompletionSource();
        }
        
        /// <summary>
        /// Sets the await latch to send signal after applying given action
        /// </summary>
        /// <param name="action"></param>
        protected void ApplyLatch(Action action = null)
        {
            action?.Invoke();
            awaitLatch.Set();
        }

        /// <summary>
        /// Waits for await latch to set asynchronously
        /// </summary>
        /// <returns>A task that can be awaited</returns>
        protected Task WaitAsync()
        {
            return awaitLatch.WaitAsync();
        }

        [TestCleanup]
        public void CleanupAwaitable()
        {
            awaitLatch = null;
        }
    }
}
