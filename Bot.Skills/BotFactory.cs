using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Skills
{
    public class BotFactory
    {
        public static IBot CreateBot(string skillName, IServiceProvider serviceProvider)
        {
            var manifests = StartupHelpers.GetSkillManifestInstances(serviceProvider);
            var foundManifest = manifests.Find(x => x.name.Equals(skillName));

            if (foundManifest == null)
            {
                throw new ArgumentException($"Manifest details for skill: {skillName} could not be found out of ${manifests.Count} available manifests");
            }

            if (foundManifest.RelatedDialogType == null)
            {
                throw new ArgumentException($"Manifest details for skill: {skillName} were found but RelatedDialogType was null");
            }

            var dialog = (Dialog)serviceProvider.GetService(foundManifest.RelatedDialogType);
            var skillBotBase = (SkillBotBase)serviceProvider.GetService(typeof(SkillBotBase));
            skillBotBase.SetDialog(dialog);
            return skillBotBase;
        }
    }
}
