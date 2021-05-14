using Bot.Skills.Dialogs.Common;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Skills.Dialogs
{
    public class LoopingPromptSkill1Manifest : SkillManifestBase
    {
        public LoopingPromptSkill1Manifest(IOptions<SkillManifestOptions> options) : base(options)
        {
            SetRelatedDialogType(typeof(LoopingPromptDialog));
        }

        public override string name => "Test.LoopingPrompt";

        public override string description => "Shows a prompt with 1 selectable value.  If that value is not selected then the prompt will loop.";

        public override string version => "1.0";
    }
    public class LoopingPromptDialog : WaterfallDialogBase<LoopingPromptDialog>
    {
        public LoopingPromptDialog() : base()
        {
        }

        protected override WaterfallStep[] WaterfallSteps => new WaterfallStep[]
        {
            PromptStepAsync,
            HandlePromptStepAsync
        };

        protected override void AddAdditionalDialogs()
        {
            AddDialog(new ChoicePrompt($"{dialogBaseName}.choiceSelection"));
        }

        protected virtual async Task<DialogTurnResult> PromptStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync($"{dialogBaseName}.choiceSelection",
                new PromptOptions
                {
                    Style = ListStyle.HeroCard,
                    Prompt = MessageFactory.Text("Push 'Handle Prompt' below to continue execution of this skill which uses a waterfall dialog"),
                    Choices = ChoiceFactory.ToChoices(new List<string>() { "Handle Prompt" })
                }, cancellationToken);
        }

        protected virtual async Task<DialogTurnResult> HandlePromptStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you for pushing 'Handle Prompt' you may now continue without being prompted each time."));

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }
    }
}
