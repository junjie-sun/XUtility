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
    /// 订阅者
    /// </summary>
    public class Subscriber : ISubscriber
    {
        private Semaphore semaphore;

        private TimeSpan waitTimeout;

        private CancellationTokenSource cancellationTokenSource;

        private SubscriberStatus status;

        #region 属性

        /// <summary>
        /// 等待通知用的信号量
        /// </summary>
        protected Semaphore Semaphore { get { return semaphore; } }

        /// <summary>
        /// 等待超时时间
        /// </summary>
        protected TimeSpan WaitTimeout { get { return waitTimeout; } }

        /// <summary>
        /// 用于取消订阅
        /// </summary>
        protected CancellationTokenSource CancellationTokenSource { get { return cancellationTokenSource; } }

        /// <summary>
        /// 订阅者名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 发布者名称
        /// </summary>
        public string PublisherName { get; }

        /// <summary>
        /// 订阅者状态
        /// </summary>
        public SubscriberStatus Status { get { return status; } protected set { status = value; } }

        #endregion

        #region 公共方法

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">订阅者名称</param>
        /// <param name="publisherName">发布者名称</param>
        /// <param name="waitTimeout">等待超时时间，单位：分钟</param>
        public Subscriber(string name, string publisherName, int waitTimeout = 15) : this(name, publisherName, TimeSpan.FromMinutes(waitTimeout))
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">订阅者名称</param>
        /// <param name="publisherName">发布者名称</param>
        /// <param name="waitTimeout">等待超时时间，单位：分钟</param>
        public Subscriber(string name, string publisherName, TimeSpan waitTimeout)
        {
            Name = name;
            PublisherName = publisherName;
            this.Status = SubscriberStatus.Init;
            this.waitTimeout = waitTimeout;
            this.cancellationTokenSource = new CancellationTokenSource();

            Init();
        }

        /// <summary>
        /// 等待通知
        /// </summary>
        /// <param name="callback">收到通知后的回调</param>
        /// <param name="completedCallback"></param>
        /// <returns></returns>
        public virtual void StartWaitNotice(Action callback, Action<SubscribeCompletedInfo> completedCallback = null)
        {
            if (callback == null)
            {
                this.Status = SubscriberStatus.Error;
                throw new ArgumentException("callback不能为null");
            }

            if (this.Status != SubscriberStatus.Ready)
            {
                this.Status = SubscriberStatus.Error;
                throw new InvalidOperationException("只有状态为Ready时才能启动等待通知");
            }

            var task = new Task(() =>
            {
                while (!CancellationTokenSource.IsCancellationRequested)
                {
                    this.Status = SubscriberStatus.Waitting;

                    var hasSemaphore = Semaphore.WaitOne(WaitTimeout);

                    if (hasSemaphore)
                    {
                        this.Status = SubscriberStatus.Processing;

                        callback();
                    }
                }
            }, CancellationTokenSource.Token);

            task.ContinueWith(t =>
            {
                this.Status = t.Exception == null ? SubscriberStatus.Exit : SubscriberStatus.Error;
                completedCallback?.Invoke(new SubscribeCompletedInfo(this, t.Exception?.InnerException));
            });

            task.Start();
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        public virtual void UnSubscriber()
        {
            if (this.Status == SubscriberStatus.Exit || this.Status == SubscriberStatus.Error || this.Status == SubscriberStatus.UnSubscribe)
            {
                return;
            }

            this.Status = SubscriberStatus.UnSubscribe;
            CancellationTokenSource.Cancel();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            UnSubscriber();
            Semaphore.Dispose();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            this.semaphore = new Semaphore(0, 1, PublisherName, out bool createNew);

            if (createNew)
            {
                this.Status = SubscriberStatus.Error;
                Semaphore.Dispose();
                throw new InvalidOperationException("Publisher不存在");
            }

            this.Status = SubscriberStatus.Ready;
        }

        #endregion
    }
}
