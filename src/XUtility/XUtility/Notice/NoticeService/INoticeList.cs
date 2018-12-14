// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace XUtility.Notice
{
    /// <summary>
    /// 通知列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INoticeList<T>
    {
        /// <summary>
        /// 添加通知
        /// </summary>
        /// <param name="item"></param>
        void Add(T item);

        /// <summary>
        /// 取出通知
        /// </summary>
        /// <returns></returns>
        T Take();

        /// <summary>
        /// 清除所有通知
        /// </summary>
        void Clear();

        /// <summary>
        /// 通知数量
        /// </summary>
        int Count { get; }
    }
}
