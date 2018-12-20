using System.Configuration;

namespace WindowsServerState
{
    public class ServerName : ConfigurationSection // 所有配置节点都要选择这个基类
    {
        private static readonly ConfigurationProperty s_property
            = new ConfigurationProperty(string.Empty, typeof(kvCollection), null,
                ConfigurationPropertyOptions.IsDefaultCollection);

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public kvCollection KeyValues
        {
            get { return (kvCollection)base[s_property]; }
        }
    }
}
