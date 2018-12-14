// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace XUtility.Notice
{
    /// <summary>
    /// 通知管理器
    /// 提供消息分发与订阅功能
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class NoticeManager<T> : IDisposable
    {
        private readonly Object noticeLockObj = new Object();

        private CancellationTokenSource noticeCts = null;

        private TaskCompletionSource<object> noticeStopTcs = null;

        private INoticeList<T> noticeList;

        private INoticeMode<T> noticeMode;

        private SubscribeNoticeStatus status = SubscribeNoticeStatus.Stop;

        private IDictionary<string, NoticeSubscriber<T>> subscriberList = new Dictionary<string, NoticeSubscriber<T>>();

        #region 公共属性

        /// <summary>
        /// 通知管理器状态
        /// </summary>
        public SubscribeNoticeStatus Status { get { return status; } }

        /// <summary>
        /// 未处理通知列表
        /// </summary>
        public INoticeList<T> NoticeList { get { return noticeList; } }

        #endregion

        #region 公共方法

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="noticeMode">通知处理模式，默认为ParallelNoticeMode</param>
        /// <param name="noticeList">存放通知的列表，默认为NoticeQueue</param>
        public NoticeManager(INoticeMode<T> noticeMode = null, INoticeList < T> noticeList = null)
        {
            if (noticeMode == null)
            {
                noticeMode = new ParallelNoticeMode<T>();
            }

            if (noticeList == null)
            {
                noticeList = new NoticeQueue<T>();
            }

            this.noticeMode = noticeMode;
            this.noticeList = noticeList;
        }

        /// <summary>
        /// 发布通知
        /// </summary>
        /// <param name="item"></param>
        public void Publish(T item)
        {
            Monitor.Enter(noticeLockObj);

            noticeList.Add(item);

            Monitor.PulseAll(noticeLockObj);

            Monitor.Exit(noticeLockObj);
        }

        /// <summary>
        /// 启动订阅通知
        /// </summary>
        public void StartSubscribeNotice()
        {
            if (status != SubscribeNoticeStatus.Stop)
            {
                throw new InvalidOperationException("订阅通知任务只有在Stop状态下启动");
            }
            status = SubscribeNoticeStatus.Starting;
            noticeCts = new CancellationTokenSource();
            noticeStopTcs = new TaskCompletionSource<object>();
            StartSubscribeNoticeTask();
            status = SubscribeNoticeStatus.Running;
        }

        /// <summary>
        /// 停止订阅通知
        /// </summary>
        public Task StopSubscribeNotice()
        {
            if (status != SubscribeNoticeStatus.Running)
            {
                throw new InvalidOperationException("订阅通知任务未启动");
            }
            status = SubscribeNoticeStatus.RequestStop;
            noticeCts.Cancel();
            Monitor.Enter(noticeLockObj);
            Monitor.PulseAll(noticeLockObj);
            Monitor.Exit(noticeLockObj);
            return noticeStopTcs.Task.ContinueWith(task => status = SubscribeNoticeStatus.Stop);
        }

        /// <summary>
        /// 清除未处理通知列表
        /// </summary>
        public void ClearNoticeList()
        {
            if (status != SubscribeNoticeStatus.Stop)
            {
                throw new InvalidOperationException("清除通知列表只有在Stop状态下执行");
            }
            Monitor.Enter(noticeLockObj);

            noticeList.Clear();

            Monitor.PulseAll(noticeLockObj);

            Monitor.Exit(noticeLockObj);
        }

        /// <summary>
        /// 订阅通知
        /// </summary>
        /// <param name="name">订阅名称</param>
        /// <param name="callback">通知回调</param>
        /// <param name="exceptionCallback"></param>
        public void AddSubscribe(string name, Action<T> callback, Action<T, Exception> exceptionCallback = null)
        {
            if (status != SubscribeNoticeStatus.Stop)
            {
                throw new InvalidOperationException("订阅通知任务只有在Stop状态下才能增加订阅");
            }
            subscriberList.Add(name, new NoticeSubscriber<T>() { Name = name, Callback = callback, ExceptionCallback = exceptionCallback });
        }

        /// <summary>
        /// 移除订阅通知
        /// </summary>
        /// <param name="name">订阅名称</param>
        public void RemoveSubscribe(string name)
        {
            if (status != SubscribeNoticeStatus.Stop)
            {
                throw new InvalidOperationException("订阅通知任务只有在Stop状态下才能移除订阅");
            }
            if (subscriberList.ContainsKey(name))
            {
                subscriberList.Remove(name);
            }
        }

        /// <summary>
        /// 移除所有订阅通知
        /// </summary>
        public void ClearSubscribe()
        {
            if (status != SubscribeNoticeStatus.Stop)
            {
                throw new InvalidOperationException("订阅通知任务只有在Stop状态下才能移除订阅");
            }
            subscriberList.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (status == SubscribeNoticeStatus.Running)
            {
                StopSubscribeNotice();
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 启动订阅通知任务
        /// </summary>
        private void StartSubscribeNoticeTask()
        {
            Task.Run(() =>
            {
                while (!noticeCts.IsCancellationRequested)
                {
                    Monitor.Enter(noticeLockObj);

                    //如果没有通知则继续等待
                    while (noticeList.Count == 0 && !noticeCts.IsCancellationRequested)
                    {
                        Monitor.Wait(noticeLockObj);
                    }

                    var isCancel = noticeCts.IsCancellationRequested;

                    T notice = default(T);
                    if (!isCancel)
                    {
                        //从待处理列表中取出通知
                        notice = noticeList.Take();
                    }
                    
                    Monitor.Exit(noticeLockObj);

                    if (!isCancel)
                    {
                        noticeMode.Notice(notice, subscriberList.Values.ToList());
                    }
                }
            }, noticeCts.Token).ContinueWith(task =>
            {
                noticeStopTcs.SetResult(null);
            });
        }

        #endregion
    }

    /// <summary>
    /// 订阅通知状态
    /// </summary>
    public enum SubscribeNoticeStatus
    {
        /// <summary>
        /// 关闭
        /// </summary>
        Stop,

        /// <summary>
        /// 正在启动
        /// </summary>
        Starting,

        /// <summary>
        /// 正在运行
        /// </summary>
        Running,

        /// <summary>
        /// 请求关闭
        /// </summary>
        RequestStop
    }
}
