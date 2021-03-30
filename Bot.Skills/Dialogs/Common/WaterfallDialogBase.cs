using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Skills.Dialogs.Common
{
    public abstract class WaterfallDialogBase<T> : ComponentDialog where T : ComponentDialog
    {
        public WaterfallDialogBase() : base(nameof(T))
        {
            InitializeWaterfallDialog();
        }

        public WaterfallStepContext StepContext { get; private set; }

        protected abstract WaterfallStep[] WaterfallSteps { get; }

        protected string dialogBaseName = $"{typeof(T).Name}";

        protected virtual void InitializeWaterfallDialog()
        {
            AddDialog(new WaterfallDialog($"{dialogBaseName}.mainFlow", WaterfallSteps));
            AddAdditionalDialogs();

            InitialDialogId = $"{dialogBaseName}.mainFlow";
        }

        /// <summary>
        /// Override this method if you need to add additional dialogs to the dialog
        /// </summary>
        protected virtual void AddAdditionalDialogs()
        {
        }
    }
}
