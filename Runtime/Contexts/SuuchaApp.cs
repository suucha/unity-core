using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SuuchaStudio.Unity.Core.Ioc;
using SuuchaStudio.Unity.Core.Logging;
using System;
using System.IO;
using UnityEngine;

namespace SuuchaStudio.Unity.Core
{
    public delegate void CustomerUserIdChangedHandler(string oldId, string newId);
    public partial class Suucha : SuuchaBase
    {
        private readonly string suuchaDataDbFileName = "suuchaData.db";
        private readonly string suuchaDataDbFile;
        private Func<long> getRemoteUtcTimestamp;
        private readonly SuuchaData suuchaData;
        private string customerUserId;
        public Suucha()
        {
            suuchaDataDbFile = Path.Combine(UnityEngine.Application.persistentDataPath, suuchaDataDbFileName);
            suuchaData = GetSuuchaDataFromDb();
        }
        private SuuchaData GetSuuchaDataFromDb()
        {
            if (!File.Exists(suuchaDataDbFile))
            {
                File.WriteAllText(suuchaDataDbFile, "{}");
            }
            var content = File.ReadAllText(suuchaDataDbFile);
            SuuchaData oldData;
            try
            {
                oldData = JsonConvert.DeserializeObject<SuuchaData>(content);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Get Suucha data from file {suuchaDataDbFile} error: {ex.Message}");
                return new SuuchaData();
            }
            return oldData;
        }
        private void SaveSuuchaDataToDb()
        {
            try
            {
                File.WriteAllText(suuchaDataDbFile, JsonConvert.SerializeObject(suuchaData));
            }
            catch (Exception ex)
            {
                Logger.LogError($"Save Suucha data to file {suuchaDataDbFile} error: {ex.Message}");
            }
        }
        public string CustomerUserId
        {
            get
            {
                return customerUserId;
            }
        }
        private event CustomerUserIdChangedHandler CustomerUserIdChanged;
        public event CustomerUserIdChangedHandler OnCustomerUserIdChanged
        {
            add
            {
                CustomerUserIdChanged += value;
            }
            remove
            {
                CustomerUserIdChanged -= value;
            }
        }
        public void SetCustomerUserId(string userId)
        {
            if (customerUserId != userId)
            {
                var oldId = customerUserId;
                customerUserId = userId;
                CustomerUserIdChanged?.Invoke(oldId, customerUserId);
            }
        }
        public void SetGetRemoteUtcTimestampFunc(Func<long> getRemoteUtcTimestamp)
        {
            this.getRemoteUtcTimestamp = getRemoteUtcTimestamp;
        }
        private static Suucha app;
        public static Suucha App
        {
            get
            {
                if (app == null)
                {
                    app = new Suucha();
                    SuuchaGlobalMonoBehaviour.Global.Suucha = app;
                }
                return app;
            }
        }
        public long GetRemoteUtcTimestamp()
        {
            if (getRemoteUtcTimestamp == null)
            {
                return GetLocalUtcTimestamp();
            }
            return getRemoteUtcTimestamp();
        }
        public long GetLocalUtcTimestamp()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }
        private string deviceId;
        public UniTask<string> GetDeviceId()
        {
            var tcs = new UniTaskCompletionSource<string>();
            if (!string.IsNullOrEmpty(deviceId))
            {
                tcs.TrySetResult(deviceId);
                return tcs.Task;
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass client = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
                AndroidJavaObject adInfo = client.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", currentActivity);
                deviceId = adInfo.Call<string>("getId").ToString();
                if(Guid.Empty.ToString() == deviceId)
                {
                    deviceId = SystemInfo.deviceUniqueIdentifier;
                }
                tcs.TrySetResult(deviceId);
            }
            catch (Exception ex)
            {
                deviceId = SystemInfo.deviceUniqueIdentifier;
                tcs.TrySetResult(deviceId);
            }
#elif UNITY_IOS && !UNITY_EDITOR
            deviceId = UnityEngine.iOS.Device.vendorIdentifier;
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = SystemInfo.deviceUniqueIdentifier;
            }
            tcs.TrySetResult(deviceId);
#else
            deviceId = SystemInfo.deviceUniqueIdentifier;
            tcs.TrySetResult(deviceId);
#endif
            return tcs.Task;
        }
        public string PlatfromName
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        return PlatformNames.Android;
                    case RuntimePlatform.IPhonePlayer:
                        return PlatformNames.iOS;
                    case RuntimePlatform.WindowsPlayer:
                        return PlatformNames.Windows;
                    case RuntimePlatform.OSXPlayer:
                        return PlatformNames.Mac;
                }
                return PlatformNames.Others;
            }
        }
        public Suucha SetLogLevel(LogLevel logLevel)
        {
            IEnableLogLevelContainer enableLogLevelContainer;
            if (IocContainer.TryResolve<IEnableLogLevelContainer>(out enableLogLevelContainer))
            {
                enableLogLevelContainer.LogLevel = logLevel;
            }
            else
            {
                enableLogLevelContainer = new EnableLogLevelContainer(logLevel);
                IocContainer.RegisterInstance<IEnableLogLevelContainer, EnableLogLevelContainer>(enableLogLevelContainer as EnableLogLevelContainer);
            }
            return this;
        }
        public Guid AddTask(int delay, int interval, int maxTimes, Func<UniTask> func)
        {
            return TimesTaskManager.AddTask(delay, interval, maxTimes, func);
        }
        public Guid AddTask<T1>(int delay, int interval, int maxTimes,
            Func<T1, UniTask> func, T1 arg)
        {
            return TimesTaskManager.AddTask<T1>(delay, interval, maxTimes, func, arg);
        }
        public Guid AddTask<T1, T2>(int delay, int interval, int maxTimes,
            Func<T1, T2, UniTask> func, T1 arg1, T2 arg2)
        {
            return TimesTaskManager.AddTask<T1, T2>(delay, interval, maxTimes, func, arg1, arg2);
        }
        public Guid AddTask<T1, T2, T3>(int delay, int interval, int maxTimes,
            Func<T1, T2, T3, UniTask> func, T1 arg1, T2 arg2, T3 arg3)
        {
            return TimesTaskManager.AddTask<T1, T2, T3>(delay, interval, maxTimes, func, arg1, arg2, arg3);
        }
        public Guid AddTask<T1, T2, T3, T4>(int delay, int interval, int maxTimes,
            Func<T1, T2, T3, T4, UniTask> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return TimesTaskManager.AddTask<T1, T2, T3, T4>(delay, interval, maxTimes, func, arg1, arg2, arg3, arg4);
        }
        public void RemoveTask(Guid taskId)
        {
            TimesTaskManager.Remove(taskId);
        }
        public T AddComponent<T>()
            where T : Component
        {
            return SuuchaGlobalMonoBehaviour.Global.AddComponent<T>();
        }
        public void QueueOnMainThread(Action action , float time = 0f)
        {
            SuuchaGlobalMonoBehaviour.Global.QueueOnMainThread(action, time);
        }
    }
    internal class SuuchaData
    {
        public int AppLaunches { get; set; }
        public string CustomerUserId { get; set; }

    }
}
