using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Skills.Controllers
{
    [Route("api/skill")]
    [ApiController]
    public class SkillConsumerController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IServiceProvider _serviceProvider;

        public SkillConsumerController(IBotFrameworkHttpAdapter httpAdapter, IServiceProvider serviceProvider)
        {
            _adapter = httpAdapter;
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        [HttpGet]
        [Route("{skillName}")]
        public async Task PostAsync(string skillName)
        {
            await _adapter.ProcessAsync(Request, Response, BotFactory.CreateBot(skillName, _serviceProvider));
        }
    }
}
