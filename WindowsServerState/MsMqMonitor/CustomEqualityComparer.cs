using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor
{
    /// <summary>
    /// 自定义对比器
    /// </summary>
    public class CustomEqualityComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (x.Contains(y))
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
