using IotHubIngestion.Infra.Data.UoW;
using IotHubIngestion.Infra.Log;
using IoTHubIngestion.Domain.Interfaces.UoW;
using IoTHubIngestion.Domain.Models;
using Logzio.DotNet.NLog;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;

[assembly: FunctionsStartup(typeof(Infra.IoC.Startup))]

namespace Infra.IoC
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.GetContext().Configuration;

            SetLogConfiguration();

            builder.Services.AddSingleton<ILoggerManager>((s) =>
            {
                return new LoggerManager();
            }
            );
            builder.Services.AddSingleton<IUnitOfWork>((s) => {
                return new UnitOfWork(config);
            });

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
