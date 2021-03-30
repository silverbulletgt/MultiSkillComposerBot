using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Skills.Controllers
{
    [ApiController]
    public class ManifestController : ControllerBase
    {
        private IServiceProvider _serviceProvider;
        public ManifestController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [Route("api/manifest/{skillName}")]
        [HttpGet]
        public string Get(string skillName)
        {
            var manifests = StartupHelpers.GetSkillManifestInstances(_serviceProvider);
            var foundManifest = manifests.Find(x => x.name.Equals(skillName));
            return JsonConvert.SerializeObject(foundManifest);
        }
    }
}
