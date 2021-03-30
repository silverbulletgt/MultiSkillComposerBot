using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.ApplicationInsights;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.ApplicationInsights.Core;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace Bot.Skills
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("cognitivemodels.json", optional: true)
                .AddJsonFile($"cognitivemodels.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //this wasn't in the Skill template or the post about how to integrate the composer with a skill project
            //the lack of this was preventing the settings from ComposerDialogs/settings/appSettings.json from
            //being available though
            //hope that it doesn't cause any issues with anything else
            //https://stackoverflow.com/questions/65363853/bot-framework-v4-value-cannot-be-null-parameter-uristring/65364805#65364805
            services.AddSingleton<IConfiguration>(Configuration);

            // Configure MVC
            //this extension method is not recognized even though it is supposed to be in: Microsoft.Extensions.DependencyInjection
            //trying without
            services.AddControllers();//.AddNewtonsoftJson();

            // Configure server options
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // Configure channel provider
            services.AddSingleton<IChannelProvider, ConfigurationChannelProvider>();

            // Register AuthConfiguration to enable custom claim validation.
            services.AddSingleton(sp => new AuthenticationConfiguration { ClaimsValidator = new AllowedCallersClaimsValidator(sp.GetService<IConfiguration>()) });

            // Configure configuration provider
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Configure telemetry
            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<IBotTelemetryClient, BotTelemetryClient>();
            services.AddSingleton<ITelemetryInitializer, OperationCorrelationTelemetryInitializer>();
            services.AddSingleton<ITelemetryInitializer, TelemetryBotIdInitializer>();
            services.AddSingleton<TelemetryInitializerMiddleware>();
            services.AddSingleton<TelemetryLoggerMiddleware>();

            // Configure adapters
            services.AddSingleton<IBotFrameworkHttpAdapter, DefaultAdapter>();
            services.AddSingleton<BotAdapter, DefaultAdapter>();

            ConfigureState(services);
            ConfigureBotsAndDialogsAndManfest(services);
        }

        public void ConfigureBotsAndDialogsAndManfest(IServiceCollection services)
        {
            var skillDialogs = StartupHelpers.GetDialogsToDependencyInject();

            //register dialogs
            foreach (var dialog in skillDialogs)
            {
                services.AddTransient(dialog);
            }

            services.AddTransient<SkillBotBase>();

            services.AddOptions<SkillManifestOptions>().Bind(Configuration.GetSection(SkillManifestOptions.SkillManifest));

            var skillManifests = StartupHelpers.GetSkillManifestToDependencyInject();

            //register bots
            foreach (var manifest in skillManifests)
            {
                services.AddSingleton(manifest);
            }
        }

        public void ConfigureState(IServiceCollection services)
        {
            services.AddSingleton<IStorage, MemoryStorage>();

            services.AddSingleton<UserState>();
            services.AddSingleton<ConversationState>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());

            // Uncomment this to support HTTPS.
            // app.UseHttpsRedirection();
        }
    }
}
