using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MultithreadingScaffold
{
    /// <summary>
    /// 多线程工作脚手架，用于快速的创建多线程工作的代码
    /// </summary>
    public class MTScaffold
    {
        #region 参数设置

        /// <summary>
        /// 用于多线程工作执行的委托
        /// </summary>
        public delegate void ThreadWorker(int counter);
        /// <summary>
        /// 用于多线程工作完毕后的最终操作，超时重试结束时也会触发此函数
        /// </summary>
        public delegate void ThreadFinal();

        /// <summary>
        /// 工作的线程
        /// </summary>
        public ThreadWorker Worker = null;
        /// <summary>
        /// 工作结束时调用
        /// </summary>
        public ThreadFinal Final = null;
        /// <summary>
        /// 工作量，用于判断多少个线程工作完毕后算作总任务结束
        /// </summary>
        public int Workload = 0;

        /// <summary>
        /// 是否开启新线程作为调度线程，若为false则会阻塞当前线程
        /// </summary>
        public bool InNewThread = false;
        /// <summary>
        /// 指定作为锁的一个对象，若不指定则会自动创建一个Object作为锁
        /// </summary>
        public object Locker = null;
        /// <summary>
        /// 是否打印和线程工作相关的信息，默认关闭
        /// </summary>
        public bool WriteConsole = false;
        /// <summary>
        /// 最大线程数，若不指定或指定为0则会根据系统CPU核心数自动调整
        /// </summary>
        public int ThreadLimit = 0;
        /// <summary>
        /// 启动线程的睡眠时间，影响每次启动新线程的间隔，默认100
        /// </summary>
        public int SleepTime = 100;
        /// <summary>
        /// 整个MTScaffold对象的存活时间，超出这个时间将停止所有线程，单位秒，默认值为-1即不开启
        /// </summary>
        public int TTL = -1;

        /// <summary>
        /// 线程计数器，用于判断是否可以启动新线程
        /// </summary>
        private int ThreadCount = 0;
        /// <summary>
        /// 已启动线程计数器，用于判断是否可以结束全部任务
        /// </summary>
        public int Counter = 0;
        /// <summary>
        /// 整个MTScaffold对象的启动时间
        /// </summary>
        private int StartTime = 0;
        /// <summary>
        /// 存储所有线程对象的List
        /// </summary>
        private List<Thread> ls_thread;

        #endregion

        /// <summary>
        /// 实际工作线程
        /// </summary>
        private void ThreadWorking()
        {
            if (Locker == null)
                Locker = new object();

            if (ThreadLimit == 0)
                ThreadLimit = Environment.ProcessorCount;

            if (TTL != -1)
            {
                StartTime = DateTime.Now.Second;
                ls_thread = new List<Thread>();
            }

            while (Counter < Workload || ThreadCount > 0)
            {
                if (Counter >= Workload)
                    continue;

                if (TTL != -1)
                    if (DateTime.Now.Second - StartTime >= TTL)
                        return;

                if (ThreadCount < ThreadLimit)
                {
                    Thread thread = new Thread(() =>
                    {
                        if (WriteConsole)
                            LogOut($"启动工作线程，当前线程：{ThreadCount}个，第 {Counter + 1} / {Workload} 个任务");

                        var index = 0;

                        lock (Locker)
                            index = ++Counter;

                        try
                        {
                            Worker(index - 1);
                        }
                        catch (ThreadInterruptedException)
                        {

                        }

                        ThreadCount--;
                    });

                    if (TTL != -1)
                        ls_thread.Add(thread);

                    thread.Start();
                    ThreadCount++;
                }

                Thread.Sleep(SleepTime);
            }

            Final?.Invoke();
        }

        /// <summary>
        /// 调用线程
        /// </summary>
        private void CallThreadWorking()
        {
            if (InNewThread)
                Task.Run(new Action(() => { ThreadWorking(); }));
            else
                ThreadWorking();
        }

        /// <summary>
        /// 调用线程，启动所有的多线程工作
        /// </summary>
        public void StartThreadWorking()
        {
            if (Workload == 0)
                throw new Exception("工作量不能为0，请指定！");

            if (TTL != -1)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(TTL * 1000);

                    if (ls_thread != null)
                        foreach (var t in ls_thread)
                            t.Interrupt();
                });
            }

            CallThreadWorking();
        }

        /// <summary>
        /// 输出LOG
        /// </summary>
        /// <param name="str"></param>
        private void LogOut(string str)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($@"{time} || {str}");
        }
    }
}
