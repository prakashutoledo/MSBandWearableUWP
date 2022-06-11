﻿using System;
using System.Threading.Tasks;
using System.Linq;

namespace IDEASLabUT.MSBandWearable.Util
{
    /// <summary>
    /// A utility for managing <see cref="Task"/>
    /// </summary>
    public static class TaskUtil
    {
        /// <summary>
        /// Creates a continuation from the successfull completion for the current task without cancelled or halted because of
        /// unhandled exception
        /// </summary>
        /// <param name="currentTask">A current task to add continuation with the completion status</param>
        /// <returns>A task that can be awaited to get the status result</returns>
        public static Task<bool> ContinueWithStatus(this Task currentTask)
        {
            return currentTask.ContinueWith(previousTask => Task.FromResult(previousTask.IsCompletedWithSuccess())).Unwrap();
        }

        /// <summary>
        /// Creates a continuation task which invoke given continuationAction if and only if current task is completed succesfully 
        /// without being cancelled or halted because unhandled exception
        /// </summary>
        /// <param name="currentTask">A current task to add continuation task to invoke continuationAction</param>
        /// <param name="continuationAction">A continuation action to invoke</param>
        /// <returns>A task that can be awaited</returns>
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

        /// <summary>
        /// Creates a continuation task which invoke given continuationAction with the result of current task if and only if 
        /// current task is completed succesfully without being cancelled or halted because unhandled exception
        /// </summary>
        /// <typeparam name="T">A type of paramater to pass into continuation action</typeparam>
        /// <param name="currentTask">A current task to add continuation task to invoke continuation action</param>
        /// <param name="continuationAction">A task that can be awaited</param>
        /// <returns>A task that can be awaited</returns>
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

        /// <summary>
        /// Creates a continuation from the succesfull completion status of the current task without being cancelled or halted and the 
        /// result 
        /// </summary>
        /// <param name="currentTask">A current task to add continuation status task</param>
        /// <returns>A task that can be awaited to get status</returns>
        public static Task<bool> ContinueWithStatus(this Task<bool[]> currentTask)
        {
            return currentTask.ContinueWith(previousTask => Task.FromResult(previousTask.IsCompletedWithSuccess() && previousTask.Result.All(status => status))).Unwrap();
        }

        /// <summary>
        /// Creates a continuation task by invoking given continuation function with the status of completion of the current task
        /// without being cancelled or being halted
        /// </summary>
        /// <param name="currentTask">A current task to add continuation status</param>
        /// <param name="continuationFunction">A continuation task supplier</param>
        /// <returns>A task that can be awaited</returns>
        public static Task ContinueWithStatusSupplier(this Task currentTask, Func<bool, Task> continuationFunction)
        {
            return currentTask.ContinueWith(previousTask => continuationFunction.Invoke(previousTask.IsCompletedWithSuccess())).Unwrap();
        }

        /// <summary>
        /// Creates a continuation task of type <see cref="Task{T}"/> by invoking given continuation function with current task
        /// </summary>
        /// <typeparam name="T">A type of task type parameter</typeparam>
        /// <param name="currentTask">A current bool task to add continuation task</param>
        /// <param name="continuationFunction">A continuation task supplier</param>
        /// <returns>A task that can be awaited to get <see cref="{T}"/></returns>
        public static Task<T> ContinueWithStatusSupplier<T>(this Task<bool> currentTask, Func<Task<bool>, Task<T>> continuationFunction)
        {
            return currentTask.ContinueWith(continueTask => continuationFunction.Invoke(continueTask)).Unwrap();
        }

        /// <summary>
        /// Checks if the current task is completed without being cancelled or halted due to unhandled exception
        /// </summary>
        /// <param name="currentTask">A current task to check completion status for</param>
        /// <returns>A completion status of the current task</returns>
        public static bool IsCompletedWithSuccess(this Task currentTask)
        {
            return currentTask.IsCompleted && !(currentTask.IsCanceled || currentTask.IsFaulted);
        }
    }
}
