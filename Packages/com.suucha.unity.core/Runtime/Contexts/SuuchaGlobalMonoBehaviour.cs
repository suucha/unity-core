using Cysharp.Threading.Tasks;
using SuuchaStudio.Unity.Core.Ioc;
using SuuchaStudio.Unity.Core.Ioc.Zenject;
using SuuchaStudio.Unity.Core.LogEvents;
using SuuchaStudio.Unity.Core.Logging;
using SuuchaStudio.Unity.Core.Storages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SuuchaStudio.Unity.Core
{
    internal class SuuchaGlobalMonoBehaviour : SuuchaMonoBehaviourBase
    {
        internal static SuuchaGlobalMonoBehaviour Global { get; set; }
        internal Suucha Suucha { get; set; }
        internal static GameObject GameObject { get; set; }
        bool isInTick = false;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Init()
        {
            GameObject = new GameObject("SuuchaGlobalMonoBehaviour");
            DontDestroyOnLoad(GameObject);
            Global = GameObject.AddComponent<SuuchaGlobalMonoBehaviour>();
            //InitIoc();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (typeof(IAfterSuuchaInit).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        var instance = Activator.CreateInstance(type) as IAfterSuuchaInit;
                        try
                        {
                            instance.Execute();
                        }
                        catch (Exception ex)
                        {
                            Global.Logger.LogError($"Execute IAfterSuuchaInit(type name: {type.FullName}) error: {ex.Message}");
                        }
                    }
                }
            }
            Global.Logger.LogDebug($"App started.");
            LogEventManager.LogEvent(SuuchaEventNames.AppStart).GetAwaiter().GetResult();
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitIoc()
        {
            UnityEngine.Debug.Log("[SuuchaGlobalMonoBehaviour] DEBUG InitIoc started.");
            var zenjectContainer = new ZenjectContainer();
            zenjectContainer.Build();
            IocContainer.SetContainer(zenjectContainer);
            IocContainer.Register<SuuchaStudio.Unity.Core.Logging.ILogger, UnityLogger>(life: LifeStyle.Transient);
            IocContainer.Register<ILocalStorage, UnityLocalStorage>();
        }
        internal T AddComponent<T>()
            where T : Component
        {
            return GameObject.AddComponent<T>();
        }
        void Start()
        {
            InvokeRepeating("Tick", 0, 0.02f);
        }
        // Update is called once per frame
        void Update()
        {
            LoomUpdate();
        }
        void OnApplicationQuit()
        {

        }
        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                Logger.LogWarning("Into background.");
                if (Suucha != null)
                {
                    //Suucha.ApplicationPauseHandler?.Invoke();
                }
            }
        }
        async UniTask Tick()
        {
            if (isInTick)
            {
                return;
            }
            isInTick = true;
            await TimesTaskManager.Tick();
            isInTick = false;
        }
        #region loom
        internal struct DelayedQueueItem
        {
            public float time;
            public Action action;
        }
        private readonly List<Action> actions = new List<Action>();
        private readonly List<DelayedQueueItem> delayed = new List<DelayedQueueItem>();
        readonly List<DelayedQueueItem> currentDelayed = new List<DelayedQueueItem>();
        internal void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }
        internal void QueueOnMainThread(Action action, float time)
        {
            Logger.LogDebug($"QueueOnMainThread entered.");
            if (time != 0)
            {
                lock (delayed)
                {
                    delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                }
            }
            else
            {
                lock (actions)
                {
                    actions.Add(action);
                }
            }
        }
        readonly List<Action> currentActions = new List<Action>();

        // Update is called once per frame
        void LoomUpdate()
        {
            lock (actions)
            {
                currentActions.Clear();
                currentActions.AddRange(actions);
                actions.Clear();
            }
            foreach (var a in currentActions)
            {
                a();
            }
            lock (delayed)
            {
                currentDelayed.Clear();
                currentDelayed.AddRange(delayed.Where(d => d.time <= Time.time));
                foreach (var item in currentDelayed)
                    delayed.Remove(item);
            }
            foreach (var delayed in currentDelayed)
            {
                delayed.action();
            }
        }
        #endregion
    }
}
