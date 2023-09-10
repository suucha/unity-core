using System;
using System.Collections.Generic;
using System.Linq;

namespace SuuchaStudio.Unity.Core.Utils
{
    public class WeightUtil
    {
        private static readonly Random random;
        /// <summary>
        /// Initializes the <see cref="WeightUtil"/> class.
        /// </summary>
        static WeightUtil()
        {
            random = new Random();
        }
        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <param name="weights">The weights.</param>
        /// <returns></returns>
        public static IWeighable GetObject(IEnumerable<IWeighable> weights)
        {
            var count = weights.Count();
            if (count == 0)
            {
                return null;
            }
            if (count == 1)
            {
                var theFirst = weights.First();
                if (theFirst.Weight <= 0)
                {
                    return null;
                }
                return theFirst;
            }
            var totalWeight = weights.Sum(w => w.Weight);
            var value = random.Next(1, totalWeight + 1);
            var minValue = 1;
            var maxValue = 1;
            IWeighable result = null;
            foreach (var weight in weights)
            {
                minValue = maxValue;
                maxValue += weight.Weight;
                if (minValue <= value && value < maxValue)
                {
                    result = weight;
                    break;
                }
            }
            return result;
        }
    }
}
