using System;
using System.Threading.Tasks;
using System.Linq;

namespace IDEASLabUT.MSBandWearable.Util
{
    /// <summary>
    /// A utility for managing <see cref="Task"/>
    /// </summary>
    public static class TaskUtil
    {
        public static Task<bool> ContinueWithStatus(this Task currentTask) => currentTask.ContinueWith(previousTask => Task.FromResult(previousTask.IsCompletedWithSuccess())).Unwrap();
        
        public static Task ContinueWithAction(this Task currentTask, Action continuationAction)
        {
            return currentTask.ContinueWith(previousTask =>
            {
                if (previousTask.IsCompletedWithSuccess())
                {
                    continuationAction.Invoke();
                }
            });
        }

        public static Task ContinueWithAction<T>(this Task<T> currentTask, Action<T> continuationAction)
        {
            return currentTask.ContinueWith(previousTask =>
            {
                if (previousTask.IsCompletedWithSuccess())
                {
                    continuationAction.Invoke(previousTask.Result);
                }
            });
        }

        public static Task<bool> ContinueWithStatus(this Task<bool[]> currentTask) => currentTask.ContinueWith(previousTask => Task.FromResult(previousTask.IsCompletedWithSuccess() && previousTask.Result.All(status => status))).Unwrap();
        
        public static Task ContinueWithStatusSupplier(this Task currentTask, Func<bool, Task> continuationFunction) => currentTask.ContinueWith(result => continuationFunction?.Invoke(result.IsCompletedWithSuccess())).Unwrap();
        
        public static Task<T> ContinueWithStatusSupplier<T>(this Task<bool> currentTask, Func<Task<bool>, Task<T>> continuationFunction) => currentTask.ContinueWith(continueTask => continuationFunction?.Invoke(continueTask)).Unwrap();
       
        public static bool IsCompletedWithSuccess(this Task currentTask) => currentTask.IsCompleted && !(currentTask.IsCanceled || currentTask.IsFaulted);
    }
}
