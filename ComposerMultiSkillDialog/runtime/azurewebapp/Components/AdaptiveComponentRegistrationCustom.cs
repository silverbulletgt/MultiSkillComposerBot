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
    /// <summary>
    /// <see cref="ComponentRegistration"/> implementation for adaptive components.
    /// </summary>
    [Obsolete("Use `AdaptiveBotComponent` instead.")]
    public class AdaptiveComponentRegistrationCustom : DeclarativeComponentRegistrationBridge<AdaptiveBotComponentCustom>
    {
    }
}
