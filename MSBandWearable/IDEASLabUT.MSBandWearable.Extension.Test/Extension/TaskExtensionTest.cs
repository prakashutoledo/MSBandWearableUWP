using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace IDEASLabUT.MSBandWearable.Extension
{
    /// <summary>
    /// Unit test for <see cref="TaskExtension"/>
    /// </summary>
    [TestClass]
    public class TaskExtensionTest
    {
        [TestMethod]
        public async Task ShouldContinueWithStatus()
        {
            var status = await Task.FromCanceled(new CancellationToken(true)).ContinueWithStatus();
            Assert.IsFalse(status, "Task is completed but cancelled with token source");

            status = await Task.FromException(new Exception()).ContinueWithStatus();
            Assert.IsFalse(status, "Task is completed but faulted with exception");

            status = await Task.FromResult(false).ContinueWithStatus();
            Assert.IsTrue(status, "Task is completed from result without being cancelled or faulted with exception");

            status = await Task.CompletedTask.ContinueWithStatus();
            Assert.IsTrue(status, "Task is completed without being cancelled or faulted with exception");
        }

        [TestMethod]
        public async Task ShouldContinueWithAction()
        {
            bool changed = false;
            void ContinuationAction() => changed = true;

            await Task.FromCanceled(new CancellationToken(true)).ContinueWithAction(ContinuationAction);
            Assert.IsFalse(changed, "Action hasn't been invoked as task is cancelled");

            await Task.FromException(new Exception()).ContinueWithAction(ContinuationAction);
            Assert.IsFalse(changed, "Action hasn't been invoked as task is halted with exception");

            await Task.FromResult(true).ContinueWithAction(ContinuationAction);
            Assert.IsTrue(changed, "Action has been invoked as task from result is completed succesfully");
            changed = false;

            await Task.CompletedTask.ContinueWithAction(ContinuationAction);
            Assert.IsTrue(changed, "Action has been invoked as task is completed succesfully");
        }

        [TestMethod]
        public async Task ShouldContinueWithActionWithInput()
        {
            bool changed = false;
            void ContinuationAction(bool result) => changed = result;

            await Task.FromCanceled<bool>(new CancellationToken(true)).ContinueWithAction(ContinuationAction);
            Assert.IsFalse(changed, "Action hasn't been invoked as task is cancelled");

            await Task.FromException<bool>(new Exception()).ContinueWithAction(ContinuationAction);
            Assert.IsFalse(changed, "Action hasn't been invoked as task is halted with exception");

            await Task.FromResult(true).ContinueWithAction(ContinuationAction);
            Assert.IsTrue(changed, "Action has been invoked as task from result is completed succesfully");
            changed = false;

            await Task.FromResult(false).ContinueWithAction(ContinuationAction);
            Assert.IsFalse(changed, "Action has been invoked as task from false is completed succesfully");
        }

        [TestMethod]
        public async Task ShouldContinueWithStatusResult()
        {
            bool status = await Task.FromCanceled<bool[]>(new CancellationToken(true)).ContinueWithStatus();
            Assert.IsFalse(status, "Task is completed but is cancellation complete with cancellation token");

            status = await Task.FromException<bool[]>(new Exception()).ContinueWithStatus();
            Assert.IsFalse(status, "Task is completed but is halted with unhandeled exception");

            status = await Task.FromResult(new bool[] { true, true, false }).ContinueWithStatus();
            Assert.IsFalse(status, "Task is completed but it's result is not all true");

            status = await Task.WhenAll(Task.FromResult(true), Task.FromCanceled<bool>(new CancellationToken(true))).ContinueWithStatus();
            Assert.IsFalse(status, "Task is completed but not all task are completed without cancelled");

            status = await Task.WhenAll(Task.FromResult(true), Task.FromException<bool>(new Exception())).ContinueWithStatus();
            Assert.IsFalse(status, "All tasks are completed but halted with exception");

            status = await Task.WhenAll(Task.FromResult(true), Task.FromResult(false)).ContinueWithStatus();
            Assert.IsFalse(status);

            status = await Task.FromResult(new bool[] { true, true, true }).ContinueWithStatus();
            Assert.IsTrue(status, "All task results are true");

            status = await Task.WhenAll(Task.FromResult(true), Task.FromResult(true)).ContinueWithStatus();
            Assert.IsTrue(status, "Tasks are complete and result is all true");
        }

        [TestMethod]
        public async Task ShouldContinueWithStatusSupplier()
        {
            int changedCount = 0;
            bool changed = false;

            Task ContinuationFunction(bool result)
            {
                changedCount++;
                changed = result;
                return Task.CompletedTask;
            }

            await Task.CompletedTask.ContinueWithStatusSupplier(ContinuationFunction);
            Assert.IsTrue(changed, "Continuation function is invoked with result set to true");
            Assert.AreEqual(1, changedCount, "Continuation function is invoked with changed count increment to 1");

            changed = false;
            await Task.FromCanceled(new CancellationToken(true)).ContinueWithStatusSupplier(ContinuationFunction);
            Assert.IsFalse(changed, "Continuation function is invoked with result set to false");
            Assert.AreEqual(2, changedCount, "Continuation function is invoked with changed count increment to 2");

            changed = false;
            await Task.FromException(new Exception()).ContinueWithStatusSupplier(ContinuationFunction);
            Assert.IsFalse(changed, "Continuation function is invoked with result set to false");
            Assert.AreEqual(3, changedCount, "Continuation function is invoked with changed count increment to 1");
        }

        [TestMethod]
        public async Task ShouldContinueWithStatusSupplierGeneric()
        {
            int changedCount = 0;
            Task<bool> ContinuationFunction(Task<bool> task)
            {
                changedCount++;
                return Task.FromResult(task.Result);
            }

            var result = await Task.FromResult(true).ContinueWithSupplier(ContinuationFunction);
            Assert.IsTrue(result, "Continuation function is invoked with result set to true");
            Assert.AreEqual(1, changedCount, "Continuation function is invoked which increment changed count to 1");

            result = await Task.FromResult(false).ContinueWithSupplier(ContinuationFunction);
            Assert.IsFalse(result, "Continuation function is invoked with result set to false");
            Assert.AreEqual(2, changedCount, "Continuation function is invoked which increment changed count to 2");
        }

        [TestMethod]
        public void IsCompletedWithSuccess()
        {
            var status = Task.FromResult(false).IsCompletedWithSuccess();
            Assert.IsTrue(status, "Task is completed from result");

            status = Task.FromResult(true).IsCompletedWithSuccess();
            Assert.IsTrue(status, "Task is completed from result");

            status = Task.FromCanceled(new CancellationToken(true)).IsCompletedWithSuccess();
            Assert.IsFalse(status, "Task is completed but is cancelled");

            status = Task.FromException(new Exception()).IsCompletedWithSuccess();
            Assert.IsFalse(status, "Task is completed but is halted with exception");
        }
    }
}
