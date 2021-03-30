using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Skills
{
    public class SkillBotBase : ActivityHandler
    {
        public Dialog Dialog { get; private set; }

        protected readonly ILogger _logger;
        private readonly ConversationState _conversationState;


        public SkillBotBase(IServiceProvider serviceProvider, ILogger<SkillBotBase> logger, ConversationState conversationState)
        {
            _logger = logger;
            _conversationState = conversationState;
        }

        public void SetDialog(Dialog dialog)
        {
            Dialog = dialog;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await Dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>($"DialogState_{Dialog.Id}"), cancellationToken);

            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    }
}
