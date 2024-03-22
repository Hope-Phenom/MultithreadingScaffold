# MultithreadingScaffold

Multi-threading work scaffolding, used to quickly create multi-threaded work code. 



## QuickStart

```c#
using MultithreadingScaffold;
//Normal Mode
var arr = new string[] { "A", "B", "C" };
MTScaffold mTScaffold = new MTScaffold();
mTScaffold.Worker = (i) =>
{
    Console.WriteLine(arr[i]);
};
mTScaffold.Final = () =>
{
    Console.WriteLine("It's Over.");
};
mTScaffold.Workload = arr.Count;
mTScaffold.Start();
```

```c#
using MultithreadingScaffold;
//Plan Mode
var arr = new string[] { "A", "B", "C" };
MTScaffold mTScaffold = new MTScaffold();
mTScaffold.Worker = (i) =>
{
    Console.WriteLine(arr[i]);
};
mTScaffold.Final = () =>
{
    Console.WriteLine("It's Over.");
};
mTScaffold.IsPlanMode = true;
mTScaffold.Workload = arr.Count;
mTScaffold.Start();
```


## Param

- IsPlanMode, bool, Optional —— In this mode, MTScaffold will slice tasks and start fixed num threads to handle section. Dramatically improve performance by reducing context switching. 在计划模式下，MTScaffold 会对整体任务进行切分，并以固定数量的线程来对任务进行处理。通过减少线程的上下文切换，可以大幅的提升性能。
- Workload, int, Required —— Used to determine how many threads are counted as the end of the total task after work. 工作量，用于判断多少个线程工作完毕后算作总任务结束。
- Worker, ThreadWorker, Required  —— The multi-threading delegate used. 用于多线程工作执行的委托。
- Final, ThreadFinal, Optional —— Action after multi-threading work end. 工作结束时调用。
- InNewThread, bool, Optional —— If use a new thread to start work, or block the current thread. 是否开启新线程作为调度线程，若为false则会阻塞当前线程。- Locker, object, Optional —— Specify an object as a lock, if not specified, an Object will be automatically created as a lock. 指定作为锁的一个对象，若不指定则会自动创建一个Object作为锁。
- WriteConsole, bool, Optional —— Whether to print information related to thread work, closed by default. 是否打印和线程工作相关的信息，默认关闭。
- ThreadLimit, int, Optional —— Maximum number of threads, if not specified or specified as 0, it will be automatically adjusted according to the number of CPU cores in the system. 最大线程数，若不指定或指定为0则会根据系统CPU核心数自动调整。
- SleepTime, int, Optional —— The sleep time of the startup thread, which affects the interval of starting a new thread each time, defaults to 100. 启动线程的睡眠时间，影响每次启动新线程的间隔，默认100。
- TTL, int, Optional —— The survival time of the entire MTScaffold object, beyond this time will stop all threads, in seconds, the default value is -1 that does not open. 整个MTScaffold对象的存活时间，超出这个时间将停止所有线程，单位秒，默认值为-1即不开启。

