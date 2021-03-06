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
using System.Configuration;

namespace IoTHubIngestion
{
    public class IoTHubIngestionFunction
    {
        private IUnitOfWorkFactory _context;
        private ILoggerManager _logger;
        private IConfiguration _config;
        private readonly IOptions<ApplicationSettings> _applicationSettingsOptions;
        private readonly IOptions<ConnectionStrings> _connectionStringsOptions;


        public IoTHubIngestionFunction(IUnitOfWorkFactory context, ILoggerManager logger, IOptions<ApplicationSettings> applicationSettingsOptions, IOptions<ConnectionStrings> connectionStringsOptions)
        {
            _context = context;
            _logger = logger;
            _applicationSettingsOptions = applicationSettingsOptions;
            _connectionStringsOptions = connectionStringsOptions;
        }

        [FunctionName("IoTHubIngestionFunction")]
        public async Task Run([IoTHubTrigger("messages/events", Connection = "IotHubConnectionString")]EventData message, ILogger log, ExecutionContext context)
        {

            //FunctionConfig.ConnectionString = _connectionStringsOptions.Value.SQLConnectionString;
            FunctionConfig.ConnectionString = Environment.GetEnvironmentVariable("SQLConnectionString");
            var msg = Encoding.UTF8.GetString(message.Body.Array);
            string[] msgArray= { msg };
            log.LogInformation($"C# IoT Hub trigger function processed a message: {msg}");
            _logger.LogInfo($"C# IoT Hub trigger function processed a message: {msg}");

            using (var uow = _context.Create() )
            {
                //int v = await uow.ExecuteAsync(sql: $"INSERT INTO smart_header.IoTMessage (CodMessage, CodDevice, MessageDevice) VALUES ({new Random().Next(1, 10000)}, 1, '{msg}')");
                int v = await uow.ExecuteAsync(sql: $"BULK INSERT smart_header.IoTMessage FROM {msgArray}");
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

