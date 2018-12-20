using System;
using System.ServiceProcess;
using System.Threading;
using WindowsServerState.Model;
using Newtonsoft.Json;

namespace WindowsServerState
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    public class Service : IService
    {
        /// <summary>
        /// 模式设置
        /// </summary>
        /// <param name="mode">auto：自动 normal：手动</param>
        /// <returns></returns>
        public string modeSelection(string mode)
        {
            try
            {
                GlobalVariable.modeLock.EnterWriteLock();
                GlobalVariable.mode = mode;
            }
            finally
            {
                if (GlobalVariable.modeLock.IsWriteLockHeld)
                    GlobalVariable.modeLock.ExitWriteLock();
            }
            return "ok";
        }

        /// <summary>
        /// 获取当前模式
        /// </summary>
        /// <returns></returns>
        public string getMode()
        {
            string result;
            try
            {
                GlobalVariable.modeLock.EnterReadLock();
                result = GlobalVariable.mode;
            }
            finally
            {
                GlobalVariable.modeLock.ExitReadLock();
            }
            return result;
        }
        /// <summary>
        /// 重启服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public string restartService(string serviceName)
        {
            var service = new ServiceController(serviceName);
            try
            {
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
                return "ok";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            finally
            {
                service.Close();
            }
        }

        /// <summary>
        ///     启动服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public string startService(string serviceName)
        {
            var service = new ServiceController(serviceName);
            try
            {
                if (service.Status != ServiceControllerStatus.Running)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                }
                return "ok";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            finally
            {
                service.Close();
            }
        }

        /// <summary>
        ///     停止服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public string stopService(string serviceName)
        {
            var service = new ServiceController(serviceName);
            try
            {
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                return "ok";
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
            finally
            {
                service.Close();
            }
        }

        /// <summary>
        /// 获取托管服务状态
        /// </summary>
        /// <returns></returns>
        public string getServiceStatas()
        {
            string result;
            try
            {
                GlobalVariable.StateLock.EnterReadLock();
                result = JsonConvert.SerializeObject(GlobalVariable.SericeStates);
            }
            finally
            {
                GlobalVariable.StateLock.ExitReadLock();
            }
            return result;
        }
    }
}
