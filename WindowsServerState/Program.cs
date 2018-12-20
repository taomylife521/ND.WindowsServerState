using System;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace WindowsServerState
{
    class Program
    {
        static void Main(string[] args)
        {
            //加载日志配置文件
            var logPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4Config.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(logPath));

            TheadHelper.Initialization();

            //Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            GlobalVariable.StateLock.TryEnterReadLock(100);
            //            Console.WriteLine("读取成功!");
            //        }
            //        catch (Exception exception)
            //        {
            //            Console.WriteLine("读取超时.");
            //            Console.WriteLine(exception.Message);
            //        }
            //        finally
            //        {
            //            if (GlobalVariable.StateLock.IsReadLockHeld)
            //                GlobalVariable.StateLock.ExitReadLock();
            //        }
            //        Thread.Sleep(300);
            //    }
            //});
            HostFactory.Run(c =>
            {
                c.RunAsLocalSystem();
                //服务名称
                c.SetServiceName("WindowsServerState");
                //服务显示名称
                c.SetDisplayName("WindowsServerState");
                //服务描述
                c.SetDescription("服务监控平台-服务状态监控服务");

                c.Service<ServerMain>(s =>
                {
                    s.ConstructUsing(b => new ServerMain());
                    s.WhenStarted(o => o.Start());
                    s.WhenStopped(o => o.Stop());
                });
            });
        }
    }
}
