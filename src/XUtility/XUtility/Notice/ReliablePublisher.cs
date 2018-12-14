// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XUtility.Notice
{
    /// <summary>
    /// 发布者，通知失败会尝试再次通知，直到所有通知都发布成功
    /// </summary>
    public class ReliablePublisher : Publisher
    {
        private TimeSpan maxTryNoticeAgainTime;

        private int hasNoticeAgainTask = 0;

        private TimeSpan TryNoticeAgainTimeIncrease = TimeSpan.FromSeconds(10);     //再次通知失败时，下次通知时间间隔增幅

        private CancellationTokenSource noticeAgainTimeCts = new CancellationTokenSource();

        /// <summary>
        /// 当通知失败，再次尝试通知初始时间间隔
        /// </summary>
        protected TimeSpan initTryNoticeAgainTime = TimeSpan.FromSeconds(10);

        /// <summary>
        /// 当通知失败，再次尝试通知当前时间间隔
        /// </summary>
        protected TimeSpan currentTryNoticeAgainTime;

        /// <summary>
        /// 需要重试通知的总数
        /// </summary>
        protected int tryNoticeAgainCount = 0;

        #region 属性

        /// <summary>
        /// 当通知失败，再次尝试通知最大时间间隔
        /// </summary>
        protected TimeSpan MaxTryNoticeAgainTime { get { return maxTryNoticeAgainTime; } }

        #endregion

        #region 公共方法

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">发布者名称</param>
        /// <param name="maxConcurrent">最大并发数</param>
        /// <param name="maxTryNoticeAgainTime">当通知失败，再次尝试通知最大时间间隔，单位：分钟</param>
        public ReliablePublisher(string name, int maxConcurrent, int maxTryNoticeAgainTime = 1) : this(name, maxConcurrent, TimeSpan.FromMinutes(maxTryNoticeAgainTime))
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">发布者名称</param>
        /// <param name="maxConcurrent">最大并发数</param>
        /// <param name="maxTryNoticeAgainTime">当通知失败，再次尝试通知最大时间间隔</param>
        public ReliablePublisher(string name, int maxConcurrent, TimeSpan maxTryNoticeAgainTime) : base(name, maxConcurrent)
        {
            this.maxTryNoticeAgainTime = maxTryNoticeAgainTime;
            this.currentTryNoticeAgainTime = this.initTryNoticeAgainTime;
        }

        /// <summary>
        /// 通知
        /// </summary>
        public override void Notice()
        {
            try
            {
                base.Notice();
            }
            catch(SemaphoreFullException)
            {
                AddNoticeAgain();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            noticeAgainTimeCts.Cancel();
            base.Dispose();
        }

        #endregion

        #region 保护方法

        /// <summary>
        /// 再次通知
        /// </summary>
        protected virtual void AddNoticeAgain()
        {
            Interlocked.Increment(ref tryNoticeAgainCount);

            if (Interlocked.CompareExchange(ref hasNoticeAgainTask, 1, 0) == 0)
            {
                Task.Run(() =>
                {
                    do
                    {
                        try
                        {
                            base.Notice();
                            Interlocked.Decrement(ref tryNoticeAgainCount);
                            currentTryNoticeAgainTime = initTryNoticeAgainTime;
                        }
                        catch (SemaphoreFullException)
                        {
                            Task.Delay(currentTryNoticeAgainTime).Wait();
                            if (currentTryNoticeAgainTime + TryNoticeAgainTimeIncrease < MaxTryNoticeAgainTime)
                            {
                                currentTryNoticeAgainTime += TryNoticeAgainTimeIncrease;
                            }
                            else
                            {
                                currentTryNoticeAgainTime = MaxTryNoticeAgainTime;
                            }
                        }
                    } while (tryNoticeAgainCount > 0 && !noticeAgainTimeCts.IsCancellationRequested);
                    Interlocked.Exchange(ref hasNoticeAgainTask, 0);
                });
            }
        }

        #endregion
    }
}
