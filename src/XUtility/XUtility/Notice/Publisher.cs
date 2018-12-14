// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using XUtility.Threading;

namespace XUtility.Notice
{
    /// <summary>
    /// 发布者
    /// </summary>
    public class Publisher : IPublisher
    {
        private Semaphore semaphore;

        private int maxConcurrent;

        #region 属性

        /// <summary>
        /// 发布通知用的信号量
        /// </summary>
        protected Semaphore Semaphore { get { return semaphore; } }

        /// <summary>
        /// 信号量最大并发数
        /// </summary>
        protected int MaxConcurrent { get { return maxConcurrent; } }

        /// <summary>
        /// 发布者名称
        /// </summary>
        public string Name { get; }

        #endregion

        #region 公共方法

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">发布者名称</param>
        /// <param name="maxConcurrent">最大并发数</param>
        public Publisher(string name, int maxConcurrent)
        {
            Name = name;
            this.maxConcurrent = maxConcurrent;

            Init();
        }

        /// <summary>
        /// 发布通知
        /// </summary>
        public virtual void Notice()
        {
            Semaphore.Release();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            Semaphore.Dispose();
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            this.semaphore = new Semaphore(0, this.MaxConcurrent, Name, out bool createNew);

            if (!createNew)
            {
                this.semaphore.Dispose();
                throw new InvalidOperationException("PublisherName已存在");
            }
        }

        #endregion
    }
}
