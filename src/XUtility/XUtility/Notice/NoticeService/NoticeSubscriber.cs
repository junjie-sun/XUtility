// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace XUtility.Notice
{
    /// <summary>
    /// 订阅者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NoticeSubscriber<T>
    {
        private ConcurrentQueue<T> noticeQueue = new ConcurrentQueue<T>();

        private int status = 1;

        /// <summary>
        /// 订阅者名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 订阅者状态
        /// </summary>
        public NoticeSubscriberStatus Status { get { return (NoticeSubscriberStatus)status; } }

        /// <summary>
        /// 通知回调
        /// </summary>
        public Action<T> Callback { get; set; }

        /// <summary>
        /// 调用通知回调时出现未处理异常时执行的回调
        /// </summary>
        public Action<T, Exception> ExceptionCallback { get; set; }

        /// <summary>
        /// 订阅者准备处理的通知队列
        /// </summary>
        public ConcurrentQueue<T> NoticeQueue { get { return noticeQueue; } }

        /// <summary>
        /// 设置订阅者状态设置为准备就绪
        /// </summary>
        /// <returns></returns>
        public bool Normal()
        {
            return Interlocked.CompareExchange(ref status, 1, 2) == 2;
        }

        /// <summary>
        /// 将订阅者状态设置为处理通知中
        /// </summary>
        /// <returns></returns>
        public bool Processing()
        {
            return Interlocked.CompareExchange(ref status, 2, 1) == 1;
        }
    }

    /// <summary>
    /// 订阅者状态
    /// </summary>
    public enum NoticeSubscriberStatus
    {
        /// <summary>
        /// 准备就绪
        /// </summary>
        Normal = 1,

        /// <summary>
        /// 处理通知中
        /// </summary>
        Processing = 2,
    }
}
