// Copyright (c) junjie sun. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XUtility.Threading
{
    /// <summary>
    /// 提供reader-writer语义的异步锁
    /// </summary>
    public sealed class AsyncOneManyLock
    {
        #region 锁的代码 

        private SpinLock m_lock = new SpinLock(true);   // 自旋锁不要用readonly

        private void Lock() { Boolean taken = false; m_lock.Enter(ref taken); }

        private void Unlock() { m_lock.Exit(); }

        #endregion

        #region 锁的状态和辅助方法

        private Int32 m_state = 0;

        private Boolean IsFree { get { return m_state == 0; } }

        private Boolean IsOwnedByWriter { get { return m_state == -1; } }

        private Boolean IsOwnedByReaders { get { return m_state > 0; } }

        private Int32 AddReaders(Int32 count) { return m_state += count; }

        private Int32 SubtractReader() { return --m_state; }

        private void MakeWriter() { m_state = -1; }

        private void MakeFree() { m_state = 0; }

        #endregion

        // 目的是在非竞态条件时增强性能和减少内存消耗
        private readonly Task m_noContentionAccessGranter;

        // 每个等待的writer都通过它们在这里排队的TaskCompletionSource来唤醒
        private readonly Queue<TaskCompletionSource<Object>> m_qWaitingWriters = new Queue<TaskCompletionSource<Object>>();

        // 一个TaskCompletionSource收到信号，所有等待的reader都唤醒
        private TaskCompletionSource<Object> m_waitingReadersSignal = new TaskCompletionSource<Object>();

        private Int32 m_numWaitingReaders = 0;


        /// <summary>
        /// 构造函数
        /// </summary>
        public AsyncOneManyLock()
        {
            m_noContentionAccessGranter = Task.FromResult<Object>(null);
        }

        /// <summary>
        /// 等待
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public Task WaitAsync(OneManyMode mode)
        {
            Task accressGranter = m_noContentionAccessGranter; // 假定无竞争

            Lock();

            switch (mode)
            {
                case OneManyMode.Exclusive:
                    if (IsFree)
                    {
                        MakeWriter();  // 无竞争
                    }
                    else
                    {
                        // 有竞争：新的writer任务进入队列，并返回它使writer等待
                        var tcs = new TaskCompletionSource<Object>();
                        m_qWaitingWriters.Enqueue(tcs);
                        accressGranter = tcs.Task;
                    }
                    break;
                case OneManyMode.Shared:
                    if (IsFree || (IsOwnedByReaders && m_qWaitingWriters.Count == 0))
                    {
                        AddReaders(1); // 无竞争 
                    }
                    else
                    { // 有竞争
                      // 竞争：递增等待的reader数量，并返回reader任务使reader等待 
                        m_numWaitingReaders++;
                        accressGranter = m_waitingReadersSignal.Task.ContinueWith(t => t.Result);
                    }
                    break;
            }

            Unlock();

            return accressGranter;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Release()
        {
            TaskCompletionSource<Object> accessGranter = null;   // 假定没有代码被释放

            Lock();

            if (IsOwnedByWriter) MakeFree(); // 一个writer离开
            else SubtractReader();           // 一个reader离开

            if (IsFree)
            {
                // 如果自由，唤醒1个等待的writer或所有等待的readers
                if (m_qWaitingWriters.Count > 0)
                {
                    MakeWriter();
                    accessGranter = m_qWaitingWriters.Dequeue();
                }
                else if (m_numWaitingReaders > 0)
                {
                    AddReaders(m_numWaitingReaders);
                    m_numWaitingReaders = 0;
                    accessGranter = m_waitingReadersSignal;

                    // 为将来需要等待的readers创建一个新的TCS
                    m_waitingReadersSignal = new TaskCompletionSource<Object>();
                }
            }
            Unlock();

            // 唤醒锁外面的writer/reader，减少竞争机率以提高性能 
            if (accessGranter != null) accessGranter.SetResult(null);
        }
    }

    /// <summary>
    /// 锁模式
    /// </summary>
    public enum OneManyMode
    {
        /// <summary>
        /// 独占模式
        /// </summary>
        Exclusive,

        /// <summary>
        /// 共享模式
        /// </summary>
        Shared
    }
}
