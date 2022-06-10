using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Threading;
using System.Threading.Tasks;

using static IDEASLabUT.MSBandWearable.Util.TaskUtil;

namespace IDEASLabUT.MSBandWearable.Util
{
    /// <summary>
    /// Unit test for <see cref="TaskUtil"/>
    /// </summary>
    [TestClass]
    public class TaskUtilTest
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
            Action continuationAction = () => changed = true;

            await Task.FromCanceled(new CancellationToken(true)).ContinueWithAction(continuationAction);
            Assert.IsFalse(changed, "Action hasn't been invoked as task is cancelled");

            await Task.FromException(new Exception()).ContinueWithAction(continuationAction);
            Assert.IsFalse(changed, "Action hasn't been invoked as task is halted with exception");

            await Task.FromResult(true).ContinueWithAction(continuationAction);
            Assert.IsTrue(changed, "Action has been invoked as task from result is completed succesfully");
            changed = false;

            await Task.CompletedTask.ContinueWithAction(continuationAction);
            Assert.IsTrue(changed, "Action has been invoked as task is completed succesfully");
        }

        [TestMethod]
        public async Task ShouldContinueWithActionWithInput()
        {
            bool changed = false;
            Action<bool> continuationAction = (result) => changed = result;

            await Task.FromCanceled<bool>(new CancellationToken(true)).ContinueWithAction(continuationAction);
            Assert.IsFalse(changed, "Action hasn't been invoked as task is cancelled");

            await Task.FromException<bool>(new Exception()).ContinueWithAction(continuationAction);
            Assert.IsFalse(changed, "Action hasn't been invoked as task is halted with exception");

            await Task.FromResult(true).ContinueWithAction(continuationAction);
            Assert.IsTrue(changed, "Action has been invoked as task from result is completed succesfully");
            changed = false;

            await Task.FromResult(false).ContinueWithAction(continuationAction);
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
     }
}
