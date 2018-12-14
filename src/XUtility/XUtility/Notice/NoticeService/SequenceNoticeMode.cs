// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace XUtility.Notice
{
    /// <summary>
    /// 顺序通知模式
    /// 按订阅者添加顺序依次处理通知
    /// 一个通知所有订阅都处理完毕后再处理下一个通知
    /// </summary>
    public sealed class SequenceNoticeMode<T> : INoticeMode<T>
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
                if (subscriber != null && subscriber.Callback != null)
                {
                    try
                    {
                        subscriber.Callback(notice);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            subscriber.ExceptionCallback?.Invoke(notice, ex);
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
