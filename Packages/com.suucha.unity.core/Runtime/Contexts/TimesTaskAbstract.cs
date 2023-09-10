
using Cysharp.Threading.Tasks;
using System;

namespace SuuchaStudio.Unity.Core
{
    public abstract class TimesTaskAbstract
    {
        private int currentTimes;
        private DateTime nextTime;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="delay">Delay time for task execution, unit: millisecond</param>
        /// <param name="interval">The duration of the task interval, in milliseconds. 
        /// If it is 0, it means that it is executed only once, and maxTimes is meaningless</param>
        /// <param name="maxTimes">Maximum number of executions of the task. If it is 0, it means there is no limit</param>
        public TimesTaskAbstract(int delay, int interval, int maxTimes)
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.Now.AddMilliseconds(delay);
            Interval = interval;
            MaxTimes = maxTimes;
            if (interval == 0)
            {
                MaxTimes = 1;
            }
            currentTimes = 0;
            nextTime = StartTime;
            IsPausing = false;
        }
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Interval { get; set; }
        public int MaxTimes { get; set; }
        public bool IsPausing { get; private set; }

        public bool IsFinished
        {
            get
            {
                if (MaxTimes == 0)
                {
                    return false;
                }
                return currentTimes >= MaxTimes;
            }
        }
        public UniTask Execute()
        {
            if (IsFinished || IsPausing)
            {
                return UniTask.CompletedTask;
            }
            UniTask task = UniTask.CompletedTask;
            var now = DateTime.Now;
            if (now >= nextTime)
            {
                task = ExecuteInternal();
                currentTimes++;
                nextTime = now.AddMilliseconds(Interval);
            }
            return task;
        }
        public void Pause()
        {
            IsPausing = true;
        }
        public void Resume()
        {
            nextTime = DateTime.Now.AddMilliseconds(Interval);
            IsPausing = false;
        }
        protected abstract UniTask ExecuteInternal();

    }
}