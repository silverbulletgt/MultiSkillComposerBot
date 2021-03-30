using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Skills
{
    public class SkillManifestOptions
    {
        public const string SkillManifest = "SkillManifest";
        public string schema { get; set; }
        public string publisherName { get; set; }
        public string iconUrl { get; set; }
        public string EndpointDefaultProtocol { get; set; }

        public List<SkillManifestEndpointSettings> Endpoints { get; set; }
    }

    public class SkillManifestEndpointSettings
    {
        public string EndpointName { get; set; }
        public string EndpointDescription { get; set; }
        public string EndpointMsAppId { get; set; }
        public string EndpointUrlBase { get; set; }
    }
}
