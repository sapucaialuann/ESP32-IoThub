using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using IoTHubIngestion.Domain.Interfaces.UoW;
using System.Threading.Tasks;
using System;
using IoTHubIngestion.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace IoTHubIngestion
{
    public class IoTHubIngestionFunction
    {
        private IUnitOfWorkFactory _context;
        private ILoggerManager _logger;
        private IConfiguration _config;
        private readonly IOptions<ApplicationSettings> _applicationSettingsOptions;
        private readonly IOptions<ConnectionStrings> _connectionStringsOptions;


        public IoTHubIngestionFunction(IUnitOfWorkFactory context, ILoggerManager logger, IConfiguration configuration, IOptions<ApplicationSettings> applicationSettingsOptions, IOptions<ConnectionStrings> connectionStringsOptions)
        {
            _context = context;
            _logger = logger;
            _config = configuration;
            _applicationSettingsOptions = applicationSettingsOptions;
            _connectionStringsOptions = connectionStringsOptions;
        }

        [FunctionName("IoTHubIngestionFunction")]
        public async Task Run([IoTHubTrigger("messages/events", Connection = "IotHubConnectionString")]EventData message, ILogger log, ExecutionContext context)
        {

            FunctionConfig.ConnectionString = _config["SQLConnectionString"];
            var msg = Encoding.UTF8.GetString(message.Body.Array);
            log.LogInformation($"C# IoT Hub trigger function processed a message: {msg}");
            _logger.LogInfo($"C# IoT Hub trigger function processed a message: {msg}");

            using (var uow = _context.Create() )
            {
                await uow.ExecuteAsync($"INSERT (CodMessage, CodDevice, MessageDevice) INTO smart_header.IoTMessage ({new Random().Next(1, 10000)}, 1, '{msg}')");
                var res = await uow.QueryAsync<IoTMessage>("SELECT * FROM IoTMessage", null);

            }

            _logger.LogInfo($"Finished execution");

        }

        private IConfigurationRoot GetAppConfiguration(ExecutionContext context)
        {
            return new ConfigurationBuilder()
                             .SetBasePath(context.FunctionAppDirectory)
                             .AddEnvironmentVariables()
                             .Build();
        }
    }
}

