using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.BaoXian.Reconciliation.Helper.Log
{
    /// <summary>
    /// 当日志被写入之后执行的回调方法
    /// </summary>
    public delegate void LoggedCallbackDelegate(LoggedCallbackInfo info);
}
