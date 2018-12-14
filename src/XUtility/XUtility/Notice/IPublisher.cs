// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace XUtility.Notice
{
    /// <summary>
    /// 发布者接口
    /// </summary>
    public interface IPublisher : IDisposable
    {
        /// <summary>
        /// 发布者名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 通知
        /// </summary>
        void Notice();
    }
}
