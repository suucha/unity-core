using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuuchaStudio.Unity.Core.Attributions
{
    public class AttributionData
    {
        public bool IsOrganic { get; set; }
        public string MediaSource { get; set; }
        public string Channel { get; set; }
        public string AdCampaignId { get; set; }
        public string AdCampaign { get; set; }
        public string AdSet { get; set; }
        public string AdSiteId { get; set; }

        public string AdCpi { get; set; }
        public string InvitationCode { get; set; }
        public string UpperInvitationCode { get; set; }
        public long LastModifiedTime { get; set; }
    }
}
