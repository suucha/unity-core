namespace SuuchaStudio.Unity.Core.AdPlaying
{
    /// <summary>
    /// 
    /// </summary>
    public enum AdRequestResults
    {
        /// <summary>
        /// 已准备好
        /// </summary>
        Successful,
        /// <summary>
        /// 未准备好
        /// </summary>
        NotReady,
        /// <summary>
        /// 已达最大次数
        /// </summary>
        MaxTimes,
        /// <summary>
        /// 无效的广告位
        /// </summary>
        InvalidPlacement,
        /// <summary>
        /// SDK未初始化
        /// </summary>
        SdkNotInitialized,
        /// <summary>
        /// 其他
        /// </summary>
        Other = 99
    }
}
