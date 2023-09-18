# Unity核心类库 - 广告播放API

## 启用广告播放
通过下面的代码启用广告播放：
``` csharp
Suucha.App.EnableAdPlaying(adManager, adPlayingStrategy);
```
> 其中的adManager是需要实现IAdManager接口的类实例，针对不同的广告播放平台实现各自不同的adManager，从而方便地替换不同的广告平台，而不用修改广告播放的相关代码。
> adPlayingStrategy是实现IAdPlayingStrategy接口的类实例，此接口定义了获取广告的策略
## 设置广告回调
通过SuuchaAdPlayingCallbacks可设置广告的各种回调，此设置是全局的
``` csharp
//设置激励视频广告点击后的回调
SuuchaAdPlayingCallbacks.OnRewardedVideoClicked += RewardedVideoClicked;
//设置激励视频广告关闭后的回调
SuuchaAdPlayingCallbacks.OnRewardedVideoClosed += RewardedVideoClosed;
...

//设置插屏广告加载成功后的回调
SuuchaAdPlayingCallbacks.OnInterstitialVideoLoaded += InterstitialVideoLoaded;
//设置插屏广告开始显示的回调
SuuchaAdPlayingCallbacks.OnInterstitialVideoShown += InterstitialVideoShown;
...

```
## 广告播放
首先判断是否有广告可以播放，激励视频使用HasRewardedVideo（插屏使用HasInterstitialVideo）来判断是否有可以播放：
``` csharp
var adRequestResults = Suucha.App.HasRewardedVideo("placement");
// var adRequestResults = SuuchaApp.Instance.HasInterstitialVideo("placement");
```
其中返回结果类为AdRequestResults，定义如下：
``` csharp
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
        SdkNotInitialized
        /// <summary>
        /// 其他
        /// </summary>
        Other = 99
    }
```
HasRewardedVideo（HasInterstitialVideo）会根据adPlayingStrategy定义的策略来做出判断，逻辑如下：
* 如果此时有缓存的广告并且策略允许播放广告则返回AdRequestResults.Successful
* 如果此时没有缓存的广告并且策略允许播放广告则返回AdRequestResults.NotReady，并且同时会请求新的广告
* 不管有没有缓存的广告只要策略不允许播放广告则返回AdRequestResults.MaxTimes表示以达到广告播放的最大次数或者AdRequestResults.Other
* 如果策略定义了对应的广告位，调用时给出了错误的广告位则会返回AdRequestResults.InvalidPlacement

通过调用RequestRewardedVideo(RequestInterstitialVideo)来请求广告：
``` csharp
Suucha.App.RequestRewardedVideo("placement");
// SuuchaApp.Instance.RequestInterstitialVideo("placement");
```

通过调用ShowRewardedVideo(ShowInterstitialVideo)来播放广告：
``` csharp
var result = Suucha.App.ShowRewardedVideo("placement");
//var result = SuuchaApp.Instance.ShowInterstitialVideo("placement");
```
播放逻辑如下：
* 如果此时有缓存的广告并且策略允许播放广告则开始播放广告，并且返回AdRequestResults.Successful
* 如果此时没有缓存的广告并且策略允许播放广告则开始播放广告，则请求新广告并且返回AdRequestResults.NotReady
* 不管有没有缓存的广告只要策略不允许播放广告则返回AdRequestResults.MaxTimes表示以达到广告播放的最大次数或者AdRequestResults.Other
* 如果策略定义了对应的广告位，调用时给出了错误的广告位则会返回AdRequestResults.InvalidPlacement

关于Banner广告，可以用下面的代码来展示或隐藏Banner广告：
``` csharp
//展示banner广告
Suucha.App.ShowBanner("placement");
//隐藏banner广告
Suucha.App.HideBanner("placement");
```

## 上报广告播放埋点事件
使用
``` csharp
//启用广告播放埋点事件上报
Suucha.App.EnableAdPlayingEvent();
//启用广告播放埋点事件上报，并且上报带placement的RewardedVideoClicked和RewardedVideoClosed事件
Suucha.App.EnableAdPlayingEvent(AdPlayingEnablePlacementEvents.RewardedVideoClicked, AdPlayingEnablePlacementEvents.RewardedVideoClosed);
//如上代码启用广告播放埋点事件后，播放广告会自动上报广告相关的埋点事件，同时还会增加带placement的埋点事件
Suucha.App.ShowRewardedVideo("double");
//以上代码除了上报ad_rewarded_closed和ad_rewarded_clicked埋点事件，同时还会上报ad_rewarded_closed_double和ad_rewarded_clicked_double埋点事件
```
> 埋点事件预设的事件名可以通过AdPlayingEventNames中定义的静态属性来更改，Suucha预设的名称如下：
> ``` csharp
>     public class AdPlayingEventNames
>   {
>       /// <summary>
>       /// 广告收益事件参数名称，默认值为"ad_revenue"
>       /// </summary>
>       /// <remarks>
>       /// 不能为空，如果为空则使用默认值
>       /// </remarks>
>       public static string RevenuePaidEventParameterName { get; set; } = "ad_revenue";
>       #region Rewarded video
>       /// <summary>
>       /// 激励视频广告加载成功事件名称，默认值为"ad_rewarded_Loaded"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string RewardedVideoLoaded { get; set; } = "ad_rewarded_loaded";
>       /// <summary>
>       /// 激励视频广告点击事件名称，默认值为"ad_rewarded_clicked"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string RewardedVideoClicked { get; set; } = "ad_rewarded_clicked";
>       /// <summary>
>       /// 激励视频广告展示完成事件名称，默认值为"ad_rewarded_shown"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string RewardedVideoShown { get; set; } = "ad_rewarded_shown";
>       /// <summary>
>       /// 激励视频广告关闭事件名称，默认值为"ad_rewarded_closed"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string RewardedVideoClosed { get; set; } = "ad_rewarded_closed";
>       /// <summary>
>       /// 激励视频广告加载失败事件名称，默认值为"ad_rewarded_load_failed"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string RewardedVideoLoadFailed { get; set; } = "ad_rewarded_load_failed";
>       /// <summary>
>       /// 激励视频广告展示失败事件名称，默认值为"ad_rewarded_show_failed"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string RewardedVideoShowFailed { get; set; } = "ad_rewarded_show_failed";
>       /// <summary>
>       /// 激励视频广告收益事件名称，默认值为"ad_purchase"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string RewardedVideoRevenuePaid { get; set; } = "ad_purchase";
>       /// <summary>
>       /// 激励视频广告完成事件名称，默认值为"ad_rewarded_completed"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string RewardedVideoCompleted { get; set; } = "ad_rewarded_completed";
>
>       #endregion
>
>       #region InterstitialVideo
>       /// <summary>
>       /// 插屏广告加载完成事件名称，默认值为"ad_interstitial_loaded"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string InterstitialVideoLoaded { get; set; } = "ad_interstitial_loaded";
>       /// <summary>
>       /// 插屏广告加载失败事件名称，默认值为"ad_interstitial_load_failed"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string InterstitialVideoLoadFailed { get; set; } = "ad_interstitial_load_failed";
>       /// <summary>
>       /// 插屏广告展示完成事件名称，默认值为"ad_interstitial_shown"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string InterstitialVideoShown { get; set; } = "ad_interstitial_shown";
>       /// <summary>
>       /// 插屏广告展示失败事件名称，默认值为"ad_interstitial_show_failed"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string InterstitialVideoShowFailed { get; set; } = "ad_interstitial_show_failed";
>       /// <summary>
>       /// 插屏广告收益事件名称，默认值为"ad_purchase"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string InterstitialVideoRevenuePaid { get; set; } = "ad_purchase";
>       /// <summary>
>       /// 插屏广告关闭事件名称，默认值为"ad_interstitial_closed"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string InterstitialVideoDismissed { get; set; } = "ad_interstitial_closed";
>       /// <summary>
>       /// 插屏广告点击事件名称，默认值为"ad_interstitial_clicked"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string InterstitialVideoClicked { get; set; } = "ad_interstitial_clicked";
>       /// <summary>
>       /// 插屏广告完成事件名称，默认值为"ad_interstitial_completed"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string InterstitialVideoCompleted { get; set; } = "ad_interstitial_completed";
>       #endregion
>
>       #region Banner
>       /// <summary>
>       /// Banner广告加载完成事件名称，默认值为"ad_banner_loaded"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string BannerLoaded { get; set; } = "ad_banner_loaded";
>       /// <summary>
>       /// Banner广告加载失败事件名称，默认值为"ad_banner_load_failed"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string BannerLoadFailed { get; set; } = "ad_banner_load_failed";
>       /// <summary>
>       /// Banner广告点击事件名称，默认值为"ad_banner_clicked"
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string BannerClicked { get; set; } = "ad_banner_clicked";
>       /// <summary>
>       /// Banner广告收益事件名称，默认值为""
>       /// </summary>
>       /// <remarks>
>       /// 如果为空，则表示不记录此事件
>       /// </remarks>
>       public static string BannerRevenuePaid { get; set; } = "ad_purchase";
>       #endregion
>   }
> ``` 