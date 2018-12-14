// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace XUtility.Notice
{
    /// <summary>
    /// 订阅结束信息
    /// </summary>
    public class SubscribeCompletedInfo
    {
        /// <summary>
        /// 订阅者对象
        /// </summary>
        public Subscriber Subscriber { get; }

        /// <summary>
        /// 订阅结束时，如果有异常则Exception不为null
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="subscriber">订阅者对象</param>
        /// <param name="ex">订阅结束时发生的异常</param>
        public SubscribeCompletedInfo(Subscriber subscriber, Exception ex = null)
        {
            Subscriber = subscriber;
            Exception = ex;
        }
    }
}
