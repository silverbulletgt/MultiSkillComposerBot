using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Skills
{
    public abstract class SkillManifestBase
    {
        public SkillManifestBase(IOptions<SkillManifestOptions> options)
        {
            schema = options.Value.schema;
            publisherName = options.Value.publisherName;
            iconUrl = options.Value.iconUrl;
            endpointDefaultprotocol = options.Value.EndpointDefaultProtocol;

            Endpoints = new List<SkillManifestEndpointSettings>();
            options.Value.Endpoints.ForEach(endpoint =>
            {
                Endpoints.Add(endpoint);
            });

            AddDefaultEndpoints();
        }

        protected void AddDefaultEndpoints()
        {
            Endpoints.ForEach(endpoint =>
            {
                endpointsList.Add(endpoint.EndpointName, new Endpoint()
                {
                    name = endpoint.EndpointName,
                    protocol = endpointDefaultprotocol,
                    endpointUrl = $"{endpoint.EndpointUrlBase}{name}",
                    msAppId = endpoint.EndpointMsAppId,
                    description = endpoint.EndpointDescription,
                });
            });
        }

        [JsonIgnore]
        public Type RelatedDialogType { get; private set; }

        public void SetRelatedDialogType(Type type)
        {
            if (!typeof(Dialog).IsAssignableFrom(type))
            {
                throw new ArgumentException("Type provided must be assignable from Microsoft.Bot.Builder.Dialogs.Dialog");
            }
            RelatedDialogType = type;
        }

        [JsonProperty("$schema", Order = 1)]
        public string schema { get; private set; }
        [JsonProperty("$id", Order = 2)]
        public string id => name;
        [JsonProperty(Order = 3)]
        public abstract string name { get; }
        [JsonProperty(Order = 4)]
        public abstract string description { get; }
        [JsonProperty(Order = 5)]
        public string publisherName { get; private set; }
        [JsonProperty(Order = 6)]
        public abstract string version { get; }
        [JsonProperty(Order = 7)]
        public string iconUrl { get; private set; }

        private List<string> tagList = new List<string>();
        [JsonProperty(Order = 8)]
        public string[] tags
        {
            get
            {
                return tagList.ToArray();
            }
            set
            {
                tagList = value.ToList();
            }
        }

        private Dictionary<string, Endpoint> endpointsList = new Dictionary<string, Endpoint>();
        [JsonProperty(Order = 9)]
        public Endpoint[] endpoints
        {
            get
            {
                return endpointsList.Select(x => x.Value).ToArray();
            }
            set
            {
                endpointsList = value.ToDictionary(x => x.name);
            }
        }


        [JsonIgnore]
        public string endpointDefaultprotocol { get; private set; }

        [JsonIgnore]
        public List<SkillManifestEndpointSettings> Endpoints { get; set; }
    }

    public class Endpoint
    {
        public string name { get; set; }
        public string protocol { get; set; }
        public string description { get; set; }
        public string endpointUrl { get; set; }
        public string msAppId { get; set; }
    }
}
