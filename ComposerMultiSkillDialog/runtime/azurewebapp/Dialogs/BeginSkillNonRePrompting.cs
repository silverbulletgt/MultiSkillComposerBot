using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotFramework.Composer.WebApp.Dialogs
{
    public class BeginSkillNonRePrompting : BeginSkillCustom
    {
        private readonly string _dialogOptionsStateKey = $"{typeof(BeginSkillCustom).FullName}.DialogOptionsData";

        public BeginSkillNonRePrompting([CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            : base(callerPath, callerLine)
        {
        }

        /// <summary>
        /// This method is overridden to deal with an issue where if 2 skills are called back to back then the
        /// "EndOfConversation" activity which gets returned from the 1st skill is passed to the 2nd skill.
        /// This prevents the 2nd skill from running successfully because there is logic on the skill side which will
        /// CancelAllDialogs when an EndOfConversation activitiy is passed
        /// 
        /// In this overridden version we will detect if an "EndOfConversation" is passed in & then pass in a blank activity.
        /// I'm not sure if this will cause any other issues. I will need to watch out for this as we continue to build more complex dialogs.
        /// </summary>
        /// <param name="dc">The <see cref="DialogContext"/> for the current turn of conversation.</param>
        /// <param name="options">Optional, initial information to pass to the dialog.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default)
        {
            if (Disabled != null && Disabled.GetValue(dc.State))
            {
                return await dc.EndDialogAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            // Update the dialog options with the runtime settings.
            DialogOptions.BotId = BotId.GetValue(dc.State);
            DialogOptions.SkillHostEndpoint = new Uri(SkillHostEndpoint.GetValue(dc.State));
            DialogOptions.ConversationIdFactory = dc.Context.TurnState.Get<SkillConversationIdFactoryBase>() ?? throw new NullReferenceException("Unable to locate SkillConversationIdFactoryBase in HostContext");
            DialogOptions.SkillClient = dc.Context.TurnState.Get<BotFrameworkClient>() ?? throw new NullReferenceException("Unable to locate BotFrameworkClient in HostContext");
            DialogOptions.ConversationState = dc.Context.TurnState.Get<ConversationState>() ?? throw new NullReferenceException($"Unable to get an instance of {nameof(ConversationState)} from TurnState.");
            DialogOptions.ConnectionName = ConnectionName.GetValue(dc.State);

            // Set the skill to call
            DialogOptions.Skill.Id = DialogOptions.Skill.AppId = SkillAppId.GetValue(dc.State);
            DialogOptions.Skill.SkillEndpoint = new Uri(SkillEndpoint.GetValue(dc.State));

            // Store the initialized DialogOptions in state so we can restore these values when the dialog is resumed.
            dc.ActiveDialog.State[_dialogOptionsStateKey] = DialogOptions;

            // Get the activity to send to the skill.
            Activity activity = null;
            if (Activity != null && ActivityProcessed.GetValue(dc.State))
            {
                // The parent consumed the activity in context, use the Activity property to start the skill.
                activity = await Activity.BindAsync(dc, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else if (dc.Context.Activity.Type.Equals(ActivityTypes.EndOfConversation))
            {
                //if an EndOfConversation activity is passed then we are assuming that this was becasue another skill
                //was called immediately before this one & the EndOfConversation from that has been passed to this skill
                //we need to prevent this issue otherwise this skill will not be called
                activity = new Activity() { Type = ActivityTypes.Event };
            }

            // Call the base to invoke the skill
            // (If the activity has not been processed, send the turn context activity to the skill (pass through)). 
            return await base.BeginDialogAsync(dc, new BeginSkillDialogOptions { Activity = activity ?? dc.Context.Activity }, cancellationToken).ConfigureAwait(false);
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
