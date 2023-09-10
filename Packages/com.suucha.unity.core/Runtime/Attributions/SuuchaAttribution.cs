using Cysharp.Threading.Tasks;
using SuuchaStudio.Unity.Core.Attributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuuchaStudio.Unity.Core
{
    public delegate void AttributionChangedHandler(AttributionData oldData, AttributionData newData);
    public partial class Suucha
    {
        public AttributionData Attribution { get; set; }
        private event AttributionChangedHandler AttributionChanged;
        public event AttributionChangedHandler OnAttributionChanged
        {
            add
            {
                AttributionChanged += value;
            }
            remove
            {
                AttributionChanged -= value;
            }
        }
        public void BeginGetAttribution()
        {
            AddTask(30000, 30000, 1, GetAttributionTimeout);
        }
        private UniTask GetAttributionTimeout()
        {
            Logger.LogWarning($"Get attribution timeout.");
            if (Attribution.LastModifiedTime == 0L)
            {
                //AddCommonEventParameter("isOrganic", "true");
                //AddCommonEventParameter("mediaSource", "organic");
                //AddCommonEventParameter("adCampaign", "");
                //AddCommonEventParameter("adCampaignId", "");
                //AddCommonEventParameter("adSiteId", "");
                //AddCommonEventParameter("adSet", "");
                //AddCommonEventParameter("adCreative", "");
                //AddCommonEventParameter("adCpi", "0");
            }
            return UniTask.CompletedTask;
        }
        public UniTask SetAttribution(bool organic, string mediaSource, string campaign, string adSet, string siteId, string campaignId, string cpi)
        {
            var oldAttribution = new AttributionData
            {
                AdCampaign = Attribution.AdCampaign,
                AdCampaignId = Attribution.AdCampaignId,
                AdCpi = Attribution.AdCpi,
                AdSet = Attribution.AdSet,
                InvitationCode = Attribution.InvitationCode,
                IsOrganic = Attribution.IsOrganic,
                AdSiteId = Attribution.AdSiteId,
                MediaSource = Attribution.MediaSource,
                LastModifiedTime = Attribution.LastModifiedTime,
                UpperInvitationCode = Attribution.UpperInvitationCode
            };
            if (Attribution.LastModifiedTime == 0L)
            {
                Attribution.LastModifiedTime = GetRemoteUtcTimestamp();
                Attribution.IsOrganic = organic;
            }
            Attribution.AdCampaign = campaign;
            if (string.IsNullOrEmpty(campaign))
            {
                Attribution.AdCampaign = "";
            }
            Attribution.AdSet = adSet;
            if (string.IsNullOrEmpty(adSet))
            {
                Attribution.AdSet = "";
            }
            Attribution.MediaSource = mediaSource;
            if (string.IsNullOrEmpty(mediaSource))
            {
                Attribution.MediaSource = "organic";
            }
            Attribution.AdSiteId = siteId;
            if (string.IsNullOrEmpty(siteId))
            {
                Attribution.AdSiteId = "";
            }
            Attribution.AdCampaignId = campaignId;
            if (string.IsNullOrEmpty(campaignId))
            {
                Attribution.AdCampaignId = "";
            }
            Attribution.AdCpi = cpi;
            if (string.IsNullOrEmpty(cpi))
            {
                Attribution.AdCpi = "0";
            }

            SaveSuuchaDataToDb();
            AttributionChanged?.Invoke(oldAttribution, Attribution);
            return UniTask.CompletedTask;
        }

    }
}
