namespace SuuchaStudio.Unity.Core.Storages
{
    public class EmptyLocalStorage : ILocalStorage
    {
        private static ILocalStorage instance;
        public static ILocalStorage Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }
                instance = new EmptyLocalStorage();
                return instance;
            }
        }
        public EmptyLocalStorage()
        {

        }
        public T Get<T>(string key)
        {
            return default;
        }

        public float GetFloat(string key)
        {
            return 0f;
        }

        public float GetFloat(string key, float defaultValue)
        {
            return 0f;
        }

        public int GetInt(string key)
        {
            return 0;
        }

        public int GetInt(string key, int defaultValue)
        {
            return defaultValue;
        }

        public long GetLong(string key)
        {
            return 0L;
        }

        public long GetLong(string key, long defaultValue)
        {
            return defaultValue;
        }

        public string GetString(string key)
        {
            return "";
        }

        public string GetString(string key, string defaultValue)
        {
            return defaultValue;
        }

        public void Save()
        {
            
        }

        public void Set<T>(string key, T value)
        {
        }

        public void SetFloat(string key, float value)
        {
        }

        public void SetInt(string key, int value)
        {
        }

        public void SetLong(string key, long value)
        {
        }

        public void SetString(string key, string value)
        {
        }
    }
}
