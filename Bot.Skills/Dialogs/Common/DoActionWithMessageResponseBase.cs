using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Skills.Dialogs.Common
{
    public abstract class DoActionWithMessageResponseBase<T, TReturn> : DoActionDialogBase<T, TReturn> where T : ComponentDialog
    {
        public DoActionWithMessageResponseBase() : base()
        {
        }

        /// <summary>
        /// Use this method to perform the action which this dialog is meant to perform
        /// After the action completes the actionResultText will be shown as part of the conversation back to the user
        /// And the success value will be returned to the parent conversation
        /// </summary>
        /// <param name="actionResultText">The text which will be displayed to the user indicating success or failure of the action</param>
        /// <returns>Indicates success or failure of the request</returns>
        protected abstract TReturn DoAction(out string actionResultText);

        protected override TReturn DoAction(WaterfallStepContext stepContext, out IActivity actionResultActivity)
        {
            string actionResultText;
            var result = DoAction(out actionResultText);

            actionResultActivity = MessageFactory.Text(actionResultText, actionResultText, InputHints.IgnoringInput);

            return result;
        }
    }
}
