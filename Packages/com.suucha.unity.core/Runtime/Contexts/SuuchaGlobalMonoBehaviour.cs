using Cysharp.Threading.Tasks;
using SuuchaStudio.Unity.Core.Ioc;
using SuuchaStudio.Unity.Core.Ioc.Zenject;
using SuuchaStudio.Unity.Core.LogEvents;
using SuuchaStudio.Unity.Core.Logging;
using SuuchaStudio.Unity.Core.Storages;
using System;
using System.Collections.Generic;
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
                        instance.Execute();
                    }
                }
            }
            LogEventManager.LogEvent(SuuchaEventNames.AppStart).GetAwaiter().GetResult();
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitIoc()
        {
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
    }
}
