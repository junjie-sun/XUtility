// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XUtility.Threading
{
    /// <summary>
    /// Task扩展类
    /// </summary>
    public static class TaskExtentions
    {
        #region WithCancellation

        private struct Void { }

        /// <summary>
        /// 检测Task是否被取消，如果被取消则抛出OperationCanceledException，否则返回原始Task
        /// </summary>
        /// <param name="originalTask"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task WithCancellation(this Task originalTask, CancellationToken ct)
        {
            await DoWithCancellation(originalTask, ct);

            //等待原始任务（以同步方式）；若任务失败，等待它将抛出第一个内部异常，而不是抛出AggregateException
            await originalTask;
        }

        /// <summary>
        /// 检测Task是否被取消，如果被取消则抛出OperationCanceledException，否则返回原始Task
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="originalTask"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<TResult> WithCancellation<TResult>(this Task<TResult> originalTask, CancellationToken ct)
        {
            await DoWithCancellation(originalTask, ct);

            //等待原始任务（以同步方式）；若任务失败，等待它将抛出第一个内部异常，而不是抛出AggregateException
            return await originalTask;
        }

        private static async Task DoWithCancellation(Task originalTask, CancellationToken ct)
        {
            //创建在CancellationToken被取消时完成的一个Task
            var cancelTask = new TaskCompletionSource<Void>();

            //一旦CancellationToken被取消，就完成Task
            using (ct.Register(t => ((TaskCompletionSource<Void>)t).TrySetResult(new Void()), cancelTask))
            {
                //创建在原始Task或CancellationToken Task先完成的一个Task
                Task any = await Task.WhenAny(originalTask, cancelTask.Task);

                //任何Task因为CancellationToken而完成，就抛出OperationCanceledException
                if (any == cancelTask.Task)
                {
                    ct.ThrowIfCancellationRequested();
                }
            }
        }

        #endregion
    }
}
