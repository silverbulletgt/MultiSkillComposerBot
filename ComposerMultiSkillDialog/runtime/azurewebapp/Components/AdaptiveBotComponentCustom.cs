using System;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Dialogs.Declarative.Obsolete;
using Microsoft.BotFramework.Composer.WebApp.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.BotFramework.Composer.WebApp.Components
{
    public class AdaptiveBotComponentCustom : AdaptiveBotComponent
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            base.ConfigureServices(services, configuration);

            //this is the replace the DeclarativeType BeginSkill so that we can handle the reprompting situation differently
            //in some cases we don't want to reprompt when there has been an interruption
            //this will be configured in appSettings.json under skill:{SkillName}:disableReprompt
            var beginSkillDI = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(DeclarativeType<BeginSkill>));
            if (beginSkillDI != null)
            {
                services.Remove(beginSkillDI);
            }

            //adding the cust class which will deal with use the configuration setting skill:{SkillName}:disableReprompt
            services.AddSingleton<DeclarativeType>(sp => new DeclarativeType<BeginSkillNonRePrompting>(BeginSkill.Kind));
        }
    }
}
