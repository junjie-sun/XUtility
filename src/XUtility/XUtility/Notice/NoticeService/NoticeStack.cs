// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace XUtility.Notice
{
    /// <summary>
    /// 通知栈
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NoticeStack<T> : Stack<T>, INoticeList<T>
    {
        /// <summary>
        /// 入栈
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            base.Push(item);
        }

        /// <summary>
        /// 出栈
        /// </summary>
        /// <returns></returns>
        public T Take()
        {
            return base.Pop();
        }
    }
}
