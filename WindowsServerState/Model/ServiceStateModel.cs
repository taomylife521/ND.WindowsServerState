namespace WindowsServerState.Model
{
    /// <summary>
    /// 服务状态名称
    /// </summary>
    public class ServiceStateModel
    {
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 服务状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
