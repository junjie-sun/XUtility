// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XUtility.Notice
{
    /// <summary>
    /// 并行通知模式
    /// 所有订阅对一个通知进行并行处理
    /// 对于每个订阅处理完一个通知后再处理下一个
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ParallelNoticeMode<T> : INoticeMode<T>
    {
        /// <summary>
        /// 向订阅者发起通知
        /// </summary>
        /// <param name="notice"></param>
        /// <param name="subscribers"></param>
        public void Notice(T notice, IList<NoticeSubscriber<T>> subscribers)
        {
            foreach (var subscriber in subscribers)
            {
                if (subscriber == null || subscriber.Callback == null)
                {
                    return;
                }

                subscriber.NoticeQueue.Enqueue(notice);

                if (subscriber.Processing())        //返回false表示已有线程在处理
                {
                    Task.Run(() =>
                    {
                        do
                        {
                            while (subscriber.NoticeQueue.TryDequeue(out T message))
                            {
                                try
                                {
                                    subscriber.Callback(message);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        subscriber.ExceptionCallback?.Invoke(message, ex);
                                    }
                                    catch { }
                                }
                            }

                            subscriber.Normal();        //线程处理完成将状态设置回Normal

                            //内循环与调用subscriber.Normal方法之间前有可能会有通知入队列
                            //如果此时subscriber.Processing方法已经被执行将不会执行新入队列的这个通知
                            //所以此处再次判断队列中是否有未处理通知并且通过调用subscriber.Processing方法避免重复开启处理线程
                        } while (subscriber.NoticeQueue.Count > 0 && subscriber.Processing());
                    });
                }
            }
        }
    }
}
