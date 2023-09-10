namespace SuuchaStudio.Unity.Core.Storages
{
    /// <summary>
    /// Interface of local storage
    /// </summary>
    public interface ILocalStorage
    {
        /// <summary>
        /// Sets the int.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        void SetInt(string key, int value);
        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        int GetInt(string key);
        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        int GetInt(string key, int defaultValue);
        /// <summary>
        /// Sets the long.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        void SetLong(string key, long value);
        /// <summary>
        /// Gets the long.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        long GetLong(string key);

        /// <summary>Gets the long.</summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        long GetLong(string key, long defaultValue);
        /// <summary>
        /// Sets the float.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        void SetFloat(string key, float value);
        /// <summary>
        /// Gets the float.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        float GetFloat(string key);
        /// <summary>
        /// Gets the float.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        float GetFloat(string key, float defaultValue);
        /// <summary>
        /// Sets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        void SetString(string key, string value);
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        string GetString(string key);
        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        string GetString(string key, string defaultValue);
        /// <summary>
        /// save data to local storage
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">data</param>
        /// <returns></returns>
        void Set<T>(string key, T value);
        /// <summary>
        /// get data from local storage
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="key">key</param>
        /// <returns></returns>
        T Get<T>(string key);
        /// <summary>
        /// Save all data to the persistent device.
        /// </summary>
        /// <returns></returns>
        void Save();
    }
}
