using System.Threading;

namespace WindowsServerState
{
    /// <summary>
    ///     线程池初始化 中心 培培原创
    /// </summary>
    public class TheadHelper
    {
        /// <summary>
        ///     辅助线程数
        /// </summary>
        public static int maxWorkerThreads;

        /// <summary>
        ///     异步I/O线程数
        /// </summary>
        public static int maxIOThreads;

        /// <summary>
        ///     线程池初始化
        /// </summary>
        public static void Initialization()
        {
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxIOThreads);
            //Console.WriteLine("系统最大支持:");
            //Console.WriteLine("辅助线程数:{0},异步I/O线程数:{1}", maxWorkerThreads, maxIOThreads);
            //Console.WriteLine();
            //使用系统的最大配置,这要求对线程有一个控制,否则会产生竞争性死锁
            ThreadPool.SetMinThreads(maxWorkerThreads, maxIOThreads);
        }
    }
}
