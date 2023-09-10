using System.Collections.Generic;
using System;
using SuuchaStudio.Unity.Core.Logging;
using SuuchaStudio.Unity.Core.Ioc;
using Cysharp.Threading.Tasks;

namespace SuuchaStudio.Unity.Core
{
    public class TimesTaskManager
    {
        private static readonly Queue<TimesTaskAbstract> taskQueue;
        private static readonly Queue<TimesTaskAbstract> taskQueue1;
        private static readonly List<Guid> removeTaskIds;
        private static ILogger logger;
        private static bool isInTick = false;
        private static readonly object queueLock = new object();
        public static ILogger Logger
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
                return logger;
            }
        }
        private TimesTaskManager()
        {

        }
        static TimesTaskManager()
        {
            taskQueue = new Queue<TimesTaskAbstract>();
            taskQueue1 = new Queue<TimesTaskAbstract>();
            removeTaskIds = new List<Guid>();
        }
        public async static UniTask Tick()
        {
            if (isInTick)
            {
                return;
            }
            isInTick = true;
            while (taskQueue.Count != 0)
            {
                TimesTaskAbstract task;
                var removeIt = true;
                lock (queueLock)
                {
                    task = taskQueue.Dequeue();
                    if (!removeTaskIds.Contains(task.Id))
                    {
                        taskQueue1.Enqueue(task);
                        removeIt = false;
                    }
                }
                if (!removeIt)
                {
                    await task.Execute();
                }
            }
            while (taskQueue1.Count != 0)
            {
                lock (queueLock)
                {
                    var task1 = taskQueue1.Dequeue();
                    if (!task1.IsFinished)
                    {
                        taskQueue.Enqueue(task1);
                    }
                }
            }
            isInTick = false;
        }
        public static void Remove(Guid taskId)
        {
            Logger.LogDebug($"Remove task, id:{taskId}");
            lock (queueLock)
            {
                removeTaskIds.Add(taskId);
            }
        }
        private static Guid Enqueue(TimesTaskAbstract task)
        {
            lock (queueLock)
            {
                taskQueue.Enqueue(task);
            }
            return task.Id;
        }
        public static Guid AddTask(int delay, int interval, int maxTimes,
            Func<UniTask> func)
        {
            var task = new TimesTask(delay, interval, maxTimes, func);
            return Enqueue(task);
        }
        public static Guid AddTask<T1>(int delay, int interval, int maxTimes,
            Func<T1, UniTask> func, T1 arg)
        {
            var task = new TimesTask<T1>(delay, interval, maxTimes, func, arg);
            return Enqueue(task);
        }
        public static Guid AddTask<T1, T2>(int delay, int interval, int maxTimes,
            Func<T1, T2, UniTask> func, T1 arg1, T2 arg2)
        {
            var task = new TimesTask<T1, T2>(delay, interval, maxTimes, func, arg1, arg2);
            return Enqueue(task);
        }
        public static Guid AddTask<T1, T2, T3>(int delay, int interval, int maxTimes,
            Func<T1, T2, T3, UniTask> func, T1 arg1, T2 arg2, T3 arg3)
        {
            var task = new TimesTask<T1, T2, T3>(delay, interval, maxTimes, func, arg1, arg2, arg3);
            return Enqueue(task);
        }
        public static Guid AddTask<T1, T2, T3, T4>(int delay, int interval, int maxTimes,
            Func<T1, T2, T3, T4, UniTask> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var task = new TimesTask<T1, T2, T3, T4>(delay, interval, maxTimes, func, arg1, arg2, arg3, arg4);
            return Enqueue(task);
        }
    }
}