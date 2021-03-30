using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Skills
{
    public class StartupHelpers
    {
        public static List<Type> GetDialogsToDependencyInject()
        {
            return ReflectionHelpers.GetNonAbstractImplementationsOfType(typeof(ComponentDialog), new List<string>() { "Bot.Skills" }, "Bot.Skills");
        }

        public static List<Type> GetSkillManifestToDependencyInject()
        {
            return ReflectionHelpers.GetNonAbstractImplementationsOfType(typeof(SkillManifestBase), new List<string>() { "Bot.Skills" }, "Bot.Skills");
        }

        public static List<SkillManifestBase> GetSkillManifestInstances(IServiceProvider serviceProvider)
        {
            List<SkillManifestBase> skillManifestBases = new List<SkillManifestBase>();

            foreach (var manifest in GetSkillManifestToDependencyInject())
            {
                skillManifestBases.Add((SkillManifestBase)serviceProvider.GetService(manifest));
            }

            return skillManifestBases;
        }
    }
}
