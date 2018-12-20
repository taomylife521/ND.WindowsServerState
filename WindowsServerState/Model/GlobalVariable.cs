using System.Collections.Generic;
using System.Threading;

namespace WindowsServerState.Model
{
    /// <summary>
    ///     公共通讯接口
    /// </summary>
    public class GlobalVariable
    {
        /// <summary>
        ///     托管服务状态读写锁
        /// </summary>
        public static readonly ReaderWriterLockSlim StateLock = new ReaderWriterLockSlim();

        /// <summary>
        ///     模式读写锁
        /// </summary>
        public static readonly ReaderWriterLockSlim modeLock = new ReaderWriterLockSlim();

        /// <summary>
        ///     模式设置 auto：自动 normal：手动
        /// </summary>
        private static string _mode = "auto";

        public static string mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        /// <summary>
        ///     托管服务状态
        /// </summary>
        public static List<ServiceStateModel> SericeStates { get; set; }
    }
}