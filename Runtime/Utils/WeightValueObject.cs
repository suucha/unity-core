namespace SuuchaStudio.Unity.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeightValueObject<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeightValueObject{T}"/> class.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="value">The value.</param>
        public WeightValueObject(T obj, int value)
        {
            Object = obj;
            Value = value;
        }
        /// <summary>
        /// Gets or sets the object.
        /// </summary>
        /// <value>
        /// The object.
        /// </value>
        public T Object { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public int Value { get; set; }
    }
}
