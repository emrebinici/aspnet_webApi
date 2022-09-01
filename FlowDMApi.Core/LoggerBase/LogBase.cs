using FlowDMApi.Core.Enums;

namespace FlowDMApi.Core.LoggerBase
{
    public abstract class LogBase
    {
        protected readonly object LockObj = new object();
        public abstract void Log(Level level,string message,string path = "");
    }
}
