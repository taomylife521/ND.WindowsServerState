using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServerState.MsMqMonitor.Alarm
{
    /// <summary>
    /// 预警接口
    /// </summary>
   public interface IArarm
    {
        bool Alarm(AlarmOption option);
    }
}
