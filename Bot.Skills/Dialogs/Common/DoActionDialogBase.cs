using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Skills.Dialogs.Common
{
    public abstract class DoActionDialogBase<T, TReturn> : WaterfallDialogBase<T> where T : ComponentDialog
    {
        public DoActionDialogBase() : base()
        {
        }

        protected override WaterfallStep[] WaterfallSteps => new WaterfallStep[]
                {
                    ActionStepAsync
                };

        /// <summary>
        /// Use this method to perform the action which this dialog is meant to perform
        /// After the action completes the out activity will be reported to the user
        /// And the success value will be returned to the parent conversation
        /// </summary>
        /// <param name="actionResultActivity"></param>
        /// <returns>Indicates success or failure of the request</returns>
        protected abstract TReturn DoAction(WaterfallStepContext stepContext, out IActivity actionResultActivity);

        protected virtual async Task<DialogTurnResult> ActionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            IActivity actionResultActivity;
            var result = DoAction(stepContext, out actionResultActivity);

            if (actionResultActivity != null)
            {
                await stepContext.Context.SendActivityAsync(actionResultActivity, cancellationToken);
            }
            return await stepContext.EndDialogAsync(result);
        }
    }
}
