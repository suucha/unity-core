using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace SuuchaStudio.Unity.Core
{
    public class TimesTask : TimesTaskAbstract
    {
        private readonly Func<UniTask> func;
        internal TimesTask(int delay, int interval, int maxTimes, Func<UniTask> func) :
            base(delay, interval, maxTimes)
        {
            if (func == null)
            {
                Debug.LogError("The parameter func cannot be null.");
                throw new InvalidOperationException("The parameter func cannot be null.");
            }
            this.func = func;
        }
        protected override UniTask ExecuteInternal()
        {
            return func.Invoke();
        }
    }

    public class TimesTask<T> : TimesTaskAbstract
    {
        private readonly Func<T, UniTask> func;
        private readonly T arg;
        internal TimesTask(int delay, int interval, int maxTimes, Func<T, UniTask> func, T arg) :
            base(delay, interval, maxTimes)
        {
            if (func == null)
            {
                Debug.LogError("The parameter func cannot be null.");
                throw new InvalidOperationException("The parameter func cannot be null.");
            }
            this.arg = arg;
            this.func = func;
        }
        protected override UniTask ExecuteInternal()
        {
            return func(arg);
        }
    }

    public class TimesTask<T1, T2> : TimesTaskAbstract
    {
        private readonly Func<T1, T2, UniTask> func;
        private readonly T1 arg1;
        private readonly T2 arg2;

        internal TimesTask(int delay, int interval, int maxTimes, Func<T1, T2, UniTask> func, T1 arg1, T2 arg2)
            : base(delay, interval, maxTimes)
        {
            if (func == null)
            {
                Debug.LogError("The parameter func cannot be null.");
                throw new InvalidOperationException("The parameter func cannot be null.");
            }
            this.arg1 = arg1;
            this.arg2 = arg2;
            this.func = func;
        }
        protected override UniTask ExecuteInternal()
        {
            return func(arg1, arg2);
        }
    }
    public class TimesTask<T1, T2, T3> : TimesTaskAbstract
    {
        private readonly Func<T1, T2, T3, UniTask> func;
        private readonly T1 arg1;
        private readonly T2 arg2;
        private readonly T3 arg3;
        internal TimesTask(int delay, int interval, int maxTimes, Func<T1, T2, T3, UniTask> func, T1 arg1, T2 arg2, T3 arg3)
            : base(delay, interval, maxTimes)
        {
            if (func == null)
            {
                Debug.LogError("The parameter func cannot be null.");
                throw new InvalidOperationException("The parameter func cannot be null.");
            }
            this.arg1 = arg1;
            this.arg2 = arg2;
            this.arg3 = arg3;
            this.func = func;
        }
        protected override UniTask ExecuteInternal()
        {
            return func(arg1, arg2, arg3);
        }
    }

    public class TimesTask<T1, T2, T3, T4> : TimesTaskAbstract
    {
        private readonly Func<T1, T2, T3, T4, UniTask> func;
        private readonly T1 arg1;
        private readonly T2 arg2;
        private readonly T3 arg3;
        private readonly T4 arg4;
        internal TimesTask(int delay, int interval, int maxTimes,
            Func<T1, T2, T3, T4, UniTask> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
            : base(delay, interval, maxTimes)
        {
            if (func == null)
            {
                Debug.LogError("The parameter func cannot be null.");
                throw new InvalidOperationException("The parameter func cannot be null.");
            }
            this.arg1 = arg1;
            this.arg2 = arg2;
            this.arg3 = arg3;
            this.arg4 = arg4;
            this.func = func;
        }
        protected override UniTask ExecuteInternal()
        {
            return func(arg1, arg2, arg3, arg4);
        }
    }
}