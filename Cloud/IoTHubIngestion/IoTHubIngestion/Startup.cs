using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using IoTHubIngestion.Domain.Interfaces.UoW;
using IotHubIngestion.DataAccess.UoW;

[assembly: FunctionsStartup(typeof(IoTHubIngestion.Startup))]

namespace IoTHubIngestion
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.GetContext().Configuration;

            builder.Services.AddSingleton<IUnitOfWork>((s) => {
                return new UnitOfWork(config);
            });

        }
    }
}
