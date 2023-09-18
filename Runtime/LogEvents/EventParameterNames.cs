namespace SuuchaStudio.Unity.Core.LogEvents
{
    public class EventParameterNames
    {
        public static string PurchaseType { get; set; } = "purchase_type";
        #region ad播放相关
        public static string AdUnitId { get; set; } = "ad_unit_id";
        public static string AdType { get; set; } = "ad_type";
        public static string AdPlacement { get; set; } = "ad_placement";
        public static string AdLabel { get; set; } = "ad_label";
        #endregion
        #region 归因相关
        public static readonly string IsOrganic = "is_organic";
        public static readonly string MediaSource = "media_source";
        public static readonly string AdCampaign = "ad_campaign";
        public static readonly string AdSiteId = "ad_site_id";
        public static readonly string AdCreative = "ad_creative";
        public static readonly string AdCpi = "ad_cpi";
        public static readonly string InvitationCode = "invitation_code";
        public static readonly string UpperInvitationCode = "upper_invitation_code";
        #endregion
        #region 支付相关
        public static readonly string EventCumulativePurchase = "event_cumulative_purchase";
        public static readonly string EventPurchase = "event_purchase";
        #endregion
        #region 事件累计次数
        public static readonly string EventCumulativeCount = "event_cumulative_count";
        public static readonly string EventCount = "event_count";
        #endregion
        #region app运行时间相关
        public static readonly string AppRunDuration = "app_run_duration";
        #endregion
    }
}
