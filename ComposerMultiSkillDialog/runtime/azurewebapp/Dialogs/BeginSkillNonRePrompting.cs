using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotFramework.Composer.WebApp.Dialogs
{
    public class BeginSkillNonRePrompting : BeginSkill
    {
        private readonly string _dialogOptionsStateKey = $"{typeof(BeginSkill).FullName}.DialogOptionsData";

        public BeginSkillNonRePrompting([CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            : base(callerPath, callerLine)
        {
        }

        public override Task RepromptDialogAsync(ITurnContext turnContext, DialogInstance instance, CancellationToken cancellationToken = default)
        {
            //get the skill endpoint - this contains the skill name from configuration - we need this to get the related skill:{SkillName}:disableReprompt value
            var skillEndpoint = ((SkillDialogOptions)instance.State["Microsoft.Bot.Builder.Dialogs.Adaptive.Actions.BeginSkill.DialogOptionsData"]).Skill.SkillEndpoint;

            //get IConfiguration so that we can use it to determine if this skill should be reprompting or not
            var config = turnContext.TurnState.Get<IConfiguration>() ?? throw new NullReferenceException("Unable to locate IConfiguration in HostContext");

            //the id looks like this:
            //BeginSkillNonRePrompting['=settings.skill['testLoopingPrompt'].msAppId','']
            //parse out the skill name
            var startingSearchValue = "=settings.skill['";
            var startOfSkillName = instance.Id.IndexOf(startingSearchValue) + startingSearchValue.Length;
            var endingOfSkillName = instance.Id.Substring(startOfSkillName).IndexOf("']");
            var skillName = instance.Id.Substring(startOfSkillName, endingOfSkillName);

            //if we do not want to reprompt call EndDialogAsync instead of RepromptDialogAsync
            if (Convert.ToBoolean(config[$"skill:{skillName}:disableReprompt"]))
            {
                //this does not actually appear to remove the dialog from the stack
                //if I call "Call Skill 1" again then this line is still hit
                //so it seems like the dialog hangs around but shouldn't actually show to the user
                //not sure how to resolve this but it's not really an issue as far as I can tell
                return EndDialogAsync(turnContext, instance, DialogReason.EndCalled, cancellationToken);
            }
            else
            {
                LoadDialogOptions(turnContext, instance);
                return base.RepromptDialogAsync(turnContext, instance, cancellationToken);
            }
        }

        private void LoadDialogOptions(ITurnContext context, DialogInstance instance)
        {
            var dialogOptions = (SkillDialogOptions)instance.State[_dialogOptionsStateKey];

            DialogOptions.BotId = dialogOptions.BotId;
            DialogOptions.SkillHostEndpoint = dialogOptions.SkillHostEndpoint;
            DialogOptions.ConversationIdFactory = context.TurnState.Get<SkillConversationIdFactoryBase>() ?? throw new NullReferenceException("Unable to locate SkillConversationIdFactoryBase in HostContext");
            DialogOptions.SkillClient = context.TurnState.Get<BotFrameworkClient>() ?? throw new NullReferenceException("Unable to locate BotFrameworkClient in HostContext");
            DialogOptions.ConversationState = context.TurnState.Get<ConversationState>() ?? throw new NullReferenceException($"Unable to get an instance of {nameof(ConversationState)} from TurnState.");
            DialogOptions.ConnectionName = dialogOptions.ConnectionName;

            // Set the skill to call
            DialogOptions.Skill = dialogOptions.Skill;
        }
    }
}
