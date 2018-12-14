// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace XUtility.Notice
{
    /// <summary>
    /// 订阅者
    /// </summary>
    public interface ISubscriber : IDisposable
    {
        /// <summary>
        /// 订阅者名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 发布者名称
        /// </summary>
        string PublisherName { get; }

        /// <summary>
        /// 订阅者状态
        /// </summary>
        SubscriberStatus Status { get; }

        /// <summary>
        /// 等待通知
        /// </summary>
        /// <param name="callback">收到通知后的回调</param>
        /// <param name="completedCallback"></param>
        /// <returns></returns>
        void StartWaitNotice(Action callback, Action<SubscribeCompletedInfo> completedCallback = null);

        /// <summary>
        /// 取消订阅
        /// </summary>
        void UnSubscriber();
    }
}
