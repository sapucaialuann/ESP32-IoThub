using Infra.Data.UoW;
using IotHubIngestion.Infra.Data.UoW;
using IotHubIngestion.Infra.Log;
using IoTHubIngestion.Domain.Interfaces.UoW;
using IoTHubIngestion.Domain.Models;
using Logzio.DotNet.NLog;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using System;
using static IoTHubIngestion.IoTHubIngestionFunction;

[assembly: FunctionsStartup(typeof(IoTHubIngestion.Startup))]

namespace IoTHubIngestion
{
    public class Startup : FunctionsStartup
    {

        public override void Configure(IFunctionsHostBuilder builder)
        {
            SetLogConfiguration();

            builder.Services.AddSingleton<ILoggerManager>((s) =>
            {
                return new LoggerManager();
            }
            );

            builder.Services.AddSingleton<IUnitOfWorkFactory>((s) =>
                {
                    return new UnitOfWorkFactory(new FunctionConfig());
                });

            //my edition
            builder.Services.AddOptions<ApplicationSettings>().Configure<IConfiguration>((settings, configuration) => { configuration.Bind(settings); });

            builder.Services.AddOptions<ConnectionStrings>().Configure<IConfiguration>((settings, configuration) => { configuration.Bind("ConnectionStrings", settings); });

        }

        public void SetLogConfiguration()
        {
            var logConfig = new LoggingConfiguration();
            var logzioTarget = new LogzioTarget
            {
                Token = "RKKdMqotTSWMwMBsnbaDLAndkHIVibXc",
            };

            logConfig.AddTarget("Logzio", logzioTarget);

          
            logConfig.AddRule(LogLevel.Info, LogLevel.Fatal, "Logzio", "*");
            

            LogManager.Configuration = logConfig;

        }

    }
}
