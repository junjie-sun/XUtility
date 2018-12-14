// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace XUtility.Notice
{
    /// <summary>
    /// 通知模式接口
    /// </summary>
    public interface INoticeMode<T>
    {
        /// <summary>
        /// 向订阅者发起通知
        /// </summary>
        /// <param name="notice">需要进行处理的通知</param>
        /// <param name="subscribers">需要进行通知的订阅者</param>
        void Notice(T notice, IList<NoticeSubscriber<T>> subscribers);
    }
}
