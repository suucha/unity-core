using SuuchaStudio.Unity.Core.Ioc;
using SuuchaStudio.Unity.Core.Logging;
using SuuchaStudio.Unity.Core.Storages;

namespace SuuchaStudio.Unity.Core
{
    public class SuuchaBase
    {
        private ILocalStorage localStorage;
        protected ILocalStorage LocalStorage
        {
            get
            {
                if (localStorage != null)
                {
                    return localStorage;
                }
                if (!IocContainer.TryResolve<ILocalStorage>(out localStorage))
                {
                    return EmptyLocalStorage.Instance;
                }
                return localStorage;
            }
        }
        private ILogger logger;
        public ILogger Logger
        {
            get
            {
                if (logger != null)
                {
                    return logger;
                }
                if (!IocContainer.TryResolve<ILogger>(out logger))
                {
                    return EmptyLogger.Instance;
                }
                if (logger is LoggerAbstract)
                {
                    (logger as LoggerAbstract).Tag = this.GetType().Name;
                }
                return logger;
            }
        }
    }
}
