// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace XUtility.Notice
{
    /// <summary>
    /// 订阅者状态
    /// </summary>
    public enum SubscriberStatus
    {
        /// <summary>
        /// 初始化状态
        /// </summary>
        Init,

        /// <summary>
        /// 就绪状态
        /// </summary>
        Ready,

        /// <summary>
        /// 等待通知状态
        /// </summary>
        Waitting,

        /// <summary>
        /// 处理通知状态
        /// </summary>
        Processing,

        /// <summary>
        /// 取消订阅状态
        /// </summary>
        UnSubscribe,

        /// <summary>
        /// 正常退出状态
        /// </summary>
        Exit,

        /// <summary>
        /// 出错状态
        /// </summary>
        Error
    }
}
