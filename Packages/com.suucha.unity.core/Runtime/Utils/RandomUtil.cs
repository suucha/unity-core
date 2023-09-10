using System;
using System.Collections.Generic;
using System.Linq;

namespace SuuchaStudio.Unity.Core.Utils
{
    public class RandomUtil
    {
        private static readonly Random random = new Random();
        /// <summary>Randoms the specified sources.</summary>
        /// <param name="sources">The sources.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static int Random(List<int> sources)
        {
            var position = random.Next(0, sources.Count);
            return sources[position];
        }
        /// <summary>Randoms the specified sources.</summary>
        /// <param name="sources">The sources.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static string Random(List<string> sources)
        {
            var position = random.Next(0, sources.Count);
            return sources[position];
        }
        /// <summary>Randoms the specified sources.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sources">The sources.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static T Random<T>(List<T> sources)
        {
            var position = random.Next(0, sources.Count);
            return sources[position];
        }
        /// <summary>Randoms the specified sources.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sources">The sources.</param>
        /// <param name="excludes">The excludes.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static T Random<T>(List<T> sources, List<T> excludes)
        {
            if (excludes != null)
            {
                if (!sources.Any(s => !excludes.Contains(s)))
                {
                    return default;
                }
            }
            var position = random.Next(0, sources.Count);
            var value = sources[position];
            while (excludes != null && excludes.Contains(value))
            {
                position = random.Next(0, sources.Count);
                value = sources[position];
            }
            return value;
        }
        /// <summary>Randoms the specified minimum.</summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static int Random(int min, int max)
        {
            return random.Next(min, max + 1);
        }
        /// <summary>
        /// Randoms the specified minimum.
        /// </summary>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="excludes">The excludes.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">no random</exception>
        public static int Random(int min, int max, List<int> excludes)
        {
            var count = max - min + 1;
            if (excludes.Count >= count)
            {
                throw new InvalidOperationException("no random");
            }
            var result = random.Next(min, max + 1);
            while (excludes.Contains(result))
            {
                result = random.Next(min, max + 1);
            }
            return result;
        }
        /// <summary>
        /// Randoms the specified objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects">The objects.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns></returns>
        public static List<T> Random<T>(List<WeightValueObject<T>> objects, int minValue, int maxValue)
        {
            var selectedObjects = new List<T>();
            if (objects == null || objects.Count == 0)
            {
                return selectedObjects;
            }
            var currentCount = 0;
            var currentMaxCount = maxValue;
            while (true)
            {
                var selectableCounts = objects.Where(cc => cc.Value <= currentMaxCount).ToList();
                if (selectableCounts.Count == 0)
                {
                    break;
                }
                var selectableCount = selectableCounts[0];
                if (selectableCounts.Count > 1)
                {
                    var position = random.Next(0, selectableCounts.Count);
                    selectableCount = selectableCounts[position];
                }
                selectedObjects.Add(selectableCount.Object);
                currentCount += selectableCount.Value;
                currentMaxCount -= selectableCount.Value;
                if (currentMaxCount >= minValue)
                {
                    var minRandom = random.Next(1, 101);
                    if (minRandom < 25)
                    {
                        break;
                    }
                }
            }
            return selectedObjects;
        }
    }
}
