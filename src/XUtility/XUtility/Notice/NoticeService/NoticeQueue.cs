// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace XUtility.Notice
{
    /// <summary>
    /// 通知队列
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NoticeQueue<T> : Queue<T>, INoticeList<T>
    {
        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            base.Enqueue(item);
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <returns></returns>
        public T Take()
        {
            return base.Dequeue();
        }
    }
}
