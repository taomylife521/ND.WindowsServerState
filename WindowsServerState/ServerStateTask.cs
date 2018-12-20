using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using WindowsServerState.Log;
using WindowsServerState.Model;


namespace WindowsServerState
{
    public static class ServerStateTask
    {
        public static LogFileFolder Log = new LogFileFolder("State");

        public static ServerName ListenConf = ConfHlep.listenConfig.GetSectionBlock<ServerName>("ServerName");

        public static Dictionary<string, string> Confs = ListenConf.KeyValues.Cast<kvSetting>().ToDictionary(k => k.Key, v => v.Value);

        public static void DoTask(CancellationTokenSource tokenSource)
        {
            Task.Factory.StartNew(() =>
            {
                while (!tokenSource.IsCancellationRequested)
                {
                    while (true)
                    {
                        try
                        {
                            GlobalVariable.modeLock.EnterReadLock();
                            if (GlobalVariable.mode == "auto")
                            {
                                break;
                            }
                        }
                        finally
                        {
                            GlobalVariable.modeLock.ExitReadLock();
                        }
                        Thread.Sleep(1000 * 5);
                    }
                    //获得服务集合
                    var serviceControllers = ServiceController.GetServices();
                    var stopAll = serviceControllers.Where(x => x.Status == ServiceControllerStatus.Stopped);
                    Parallel.ForEach(stopAll, x =>
                    {
                        if (!Confs.ContainsKey(x.ServiceName)) return;
                        try
                        {
                            Log.InfoFormat("检测到服务：{0} 描述：{1} 已停止，正在重启", x.ServiceName, Confs[x.ServiceName]);
                            var st = Stopwatch.StartNew();
                            x.Start();
                            x.WaitForStatus(ServiceControllerStatus.Running);
                            st.Stop();
                            Log.InfoFormat("服务：{0} 描述：{1} 重启成功,耗时：{2}", x.ServiceName, Confs[x.ServiceName], st.ElapsedMilliseconds);
                        }
                        catch (InvalidOperationException exception)
                        {
                            Log.Fatal(string.Format("已知错误 服务：{0} 重启失败！", x.ServiceName), exception);
                        }
                        catch (Exception exception)
                        {
                            Log.Fatal(string.Format("未知错误 服务：{0} 重启失败！", x.ServiceName), exception);
                        }
                    });
                    Thread.Sleep(new TimeSpan(0, 1, 0));
                }
            });
        }
        public static void DoStateTask(CancellationTokenSource tokenSource)
        {
            Task.Factory.StartNew(() =>
            {
                while (!tokenSource.IsCancellationRequested)
                {
                    try
                    {
                        GlobalVariable.StateLock.EnterWriteLock();

                        //获得服务集合
                        var serviceControllers = ServiceController.GetServices();

                        //重置上次的状态
                        GlobalVariable.SericeStates = new List<ServiceStateModel>();

                        foreach (var x in serviceControllers)
                        {
                            if (!Confs.ContainsKey(x.ServiceName)) continue;
                            try
                            {
                                string serviceName = x.ServiceName;
                                string objPath = string.Format("Win32_Service.Name='{0}'", serviceName);
                                using (var service = new ManagementObject(new ManagementPath(objPath)))
                                {
                                    GlobalVariable.SericeStates.Add(new ServiceStateModel
                                    {
                                        ServiceName = x.ServiceName,
                                        Status = ((ServiceStatus)Convert.ToInt32(x.Status)).ToString(),
                                        DisplayName = x.DisplayName,
                                        Description = service["Description"].ToString()
                                    });
                                }
                            }
                            catch (Exception exception)
                            {
                                Log.Fatal(string.Format("未知错误：{0}", x.ServiceName), exception);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Fatal(string.Format("未知错误：{0}", exception.Message), exception);
                    }
                    finally
                    {
                        GlobalVariable.StateLock.ExitWriteLock();
                    }
                    Thread.Sleep(new TimeSpan(0, 0, 10));
                }
            });
        }
    }
    //
    // 摘要:
    //     指示该服务的当前状态。
    public enum ServiceStatus
    {
        //
        // 摘要:
        //     服务未运行。 这对应于 Win32 SERVICE_STOPPED 常量，它定义为 0x00000001。
        服务未运行 = 1,
        //
        // 摘要:
        //     服务正在启动。 这对应于 Win32 SERVICE_START_PENDING 常量，它定义为 0x00000002。
        服务正在启动 = 2,
        //
        // 摘要:
        //     服务正在停止。 这对应于 Win32 SERVICE_STOP_PENDING 常量，它定义为 0x00000003。
        服务正在停止 = 3,
        //
        // 摘要:
        //     该服务正在运行。 这对应于 Win32 SERVICE_RUNNING 常量，它定义为 0x00000004。
        服务正在运行 = 4,
        //
        // 摘要:
        //     服务继续处于挂起状态。 这对应于 Win32 SERVICE_CONTINUE_PENDING 常量，它定义为 0x00000005。
        服务继续处于挂起状态 = 5,
        //
        // 摘要:
        //     服务暂停处于挂起状态。 这对应于 Win32 SERVICE_PAUSE_PENDING 常量，它定义为 0x00000006。
        服务暂停处于挂起状态 = 6,
        //
        // 摘要:
        //     服务已暂停。 这对应于 Win32 SERVICE_PAUSED 常量，它定义为 0x00000007。
        服务已暂停 = 7
    }
}
