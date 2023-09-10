namespace SuuchaStudio.Unity.Core.AdPlaying
{
    /// <summary>
    /// 激励视频点击回调
    /// </summary>
    /// <param name="adUnitId">广告单元ID</param>
    /// <param name="placement">广告位置</param>
    public delegate void RewardedVideoClickedHandler(string adUnitId, AdCallbackInfo adCallbackInfo);
    /// <summary>
    /// 激励视频关闭回调
    /// </summary>
    /// <param name="adUnitId">广告单元ID</param>
    /// <param name="placement">广告位置</param>
    public delegate void RewardedVideoClosedHandler(string adUnitId, AdCallbackInfo adCallbackInfo);
    /// <summary>
    /// 激励视频关闭回调
    /// </summary>
    /// <param name="adUnitId">广告单元ID</param>
    /// <param name="placement">广告位置</param>
    public delegate void RewardedVideoCompletedHandler(string adUnitId, AdCallbackInfo adCallbackInfo);
    /// <summary>
    /// 激励视频展示回调
    /// </summary>
    /// <param name="adUnitId">广告单元ID</param>
    /// <param name="placement">广告位置</param>
    public delegate void RewardedVideoShownHandler(string adUnitId, AdCallbackInfo adCallbackInfo);
    /// <summary>
    /// 激励视频展示失败回调
    /// </summary>
    /// <param name="adUnitId">广告单元ID</param>
    /// <param name="error">错误信息</param>
    /// <param name="placement">广告位置</param>
    public delegate void RewardedVideoShowFailedHandler(string adUnitId, string error, string placement);
    /// <summary>
    /// 激励视频加载成功回调
    /// </summary>
    /// <param name="adUnitId">广告单元ID</param>
    /// <param name="placement">广告位置</param>
    public delegate void RewardedVideoLoadedHandler(string adUnitId, AdCallbackInfo adCallbackInfo);
    /// <summary>
    /// 激励视频加载失败回调
    /// </summary>
    /// <param name="adUnitId">广告单元ID</param>
    /// <param name="error">错误信息</param>
    public delegate void RewardedVideoLoadFailedHandler(string adUnitId, string error);
    /// <summary>
    /// 激励视频收益回调
    /// </summary>
    /// <param name="adUnitId">广告单元ID</param>
    /// <param name="label">广告收益标签</param>
    /// <param name="value">广告收益</param>
    /// <param name="placement">广告位置</param>
    public delegate void RewardedVideoRevenuePaidHandler(string adUnitId, AdCallbackInfo adCallbackInfo);

    /// <summary>
    /// Interface of rewarded video player
    /// </summary>
    public interface IRewardedVideoPlayer
    {
        /// <summary>
        /// Occurs when [on clicked].
        /// </summary>
        event RewardedVideoClickedHandler OnClicked;
        /// <summary>
        /// Occurs when [on closed].
        /// </summary>
        event RewardedVideoClosedHandler OnClosed;
        /// <summary>
        /// Occurs when [on shown].
        /// </summary>
        event RewardedVideoShownHandler OnShown;
        /// <summary>
        /// Occurs when [on loaded].
        /// </summary>
        event RewardedVideoLoadedHandler OnLoaded;
        /// <summary>
        /// Occurs when [on failed].
        /// </summary>
        event RewardedVideoLoadFailedHandler OnLoadFailed;
        /// <summary>
        /// Occurs when [on failed to play].
        /// </summary>
        event RewardedVideoShowFailedHandler OnShowFailed;
        /// <summary>
        /// Occurs when [on revenue to pay].
        /// </summary>
        event RewardedVideoRevenuePaidHandler OnRevenuePaid;
        event RewardedVideoCompletedHandler OnCompleted;
        /// <summary>
        /// Requests the rewarded video.
        /// </summary>
        /// <param name="adUnitId">The ad unit identifier.</param>
        /// <returns></returns>
        void RequestRewardedVideo(string adUnitId);
        /// <summary>
        /// Determines whether [has rewarded video] [the specified ad unit identifier].
        /// </summary>
        /// <param name="adUnitId">The ad unit identifier.</param>
        /// <returns></returns>
        bool HasRewardedVideo(string adUnitId);
        /// <summary>
        /// Shows the rewarded video.
        /// </summary>
        /// <param name="adUnitId">The ad unit identifier.</param>
        /// <returns></returns>
        void ShowRewardedVideo(string adUnitId, string placement);
    }
}
