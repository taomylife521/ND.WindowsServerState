using System.ServiceModel;

namespace WindowsServerState
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IService
    {
        /// <summary>
        /// 模式设置
        /// </summary>
        /// <param name="mode">auto：自动 normal：手动</param>
        /// <returns></returns>
        [OperationContract]
        string modeSelection(string mode);

        /// <summary>
        /// 获取当前模式
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string getMode();

        /// <summary>
        ///     重启服务
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns></returns>
        [OperationContract]
        string restartService(string serviceName);

        /// <summary>
        ///     启动服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        [OperationContract]
        string startService(string serviceName);

        /// <summary>
        ///     停止服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        [OperationContract]
        string stopService(string serviceName);

        /// <summary>
        ///     获取托管服务状态
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string getServiceStatas();
    }
}