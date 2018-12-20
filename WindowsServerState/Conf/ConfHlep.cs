using System;
using System.Configuration;
using System.IO;

namespace WindowsServerState
{
    public static class ConfHlep
    {
        /// <summary>
        ///     服务状态侦听配置
        /// </summary>
        public static Configuration listenConfig
        {
            get
            {
                var path = string.Format("{0}/listen.config", Environment.CurrentDirectory);
                if (!File.Exists(path))
                {
                    throw new Exception("找不到配置文件“listen.config”");
                }
                var pconf = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = path
                };
                var result = ConfigurationManager.OpenMappedExeConfiguration(pconf, ConfigurationUserLevel.None);
                return result;
            }
        }

        /// <summary>
        ///     服务重启侦听配置
        /// </summary>
        public static Configuration restartConfig
        {
            get
            {
                var path = string.Format("{0}/restart.config", Environment.CurrentDirectory);
                if (!File.Exists(path))
                {
                    throw new Exception("找不到配置文件“restart.config”");
                }
                var pconf = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = path
                };
                var result = ConfigurationManager.OpenMappedExeConfiguration(pconf, ConfigurationUserLevel.None);
                return result;
            }
        }

        /// <summary>
        ///     获取配置节点
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conf"></param>
        /// <param name="sectionname"></param>
        /// <returns></returns>
        public static T GetSectionBlock<T>(this Configuration conf, string sectionname) where T : class
        {
            return conf.GetSection(sectionname) as T;
        }
    }
}