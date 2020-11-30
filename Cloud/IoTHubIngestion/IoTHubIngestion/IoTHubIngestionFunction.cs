using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventHubs;
using System.Text;
using Microsoft.Extensions.Logging;
using IoTHubIngestion.Domain.Interfaces.UoW;
using System.Threading.Tasks;
using System;
using IoTHubIngestion.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

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
        public async Task Run([IoTHubTrigger("messages/events", Connection = "IotHubConnectionString")]EventData[] messages, ILogger log, ExecutionContext context)
        {

            //FunctionConfig.ConnectionString = _connectionStringsOptions.Value.SQLConnectionString;
            FunctionConfig.ConnectionString = Environment.GetEnvironmentVariable("SQLConnectionString");
            //string tempMessage = msg.ToString();
            var messageList = messages.Select(message => Encoding.UTF8.GetString(message.Body.Array)).ToList();

            
            //List<string> messageList = messageList.Add(tempMessage);
            List<string> sqls = GetSqlInBatches(messageList); // ADD PARAMETER DUE TO WHAT WILL ENTER IN THE LINES ABOVE
            //log.LogInformation($"C# IoT Hub trigger function processed a message: {msg}");
            //_logger.LogInfo($"C# IoT Hub trigger function processed a message: {msg}");


            _logger.LogInfo($"C# IoT Hub trigger function processed a message teste: {messages.Length}");
            log.LogInformation($"C# IoT Hub trigger function processed a message teste: {messages.Length}");

            using (var uow = _context.Create() )
            {

                //string sql = $"INSERT INTO smart_header.IoTMessage (CodMessage, CodDevice, MessageDevice) VALUES ({new Random().Next(1, 10000)}, 1, '{msg}')";
                //int executeQuery = await uow.ExecuteAsync(sql: sql);

                foreach (var sql in sqls)
                {
                    await uow.ExecuteAsync(sql);
                };

                var res = await uow.QueryAsync<IoTMessage>("SELECT * FROM IoTMessage", null);
            }

            _logger.LogInfo($"Finished execution");

        }

        public List<string> GetSqlInBatches(List<string> IoTMessage)
        {
            var insertQuery = $"INSERT INTO smart_header.IoTMessage (CodMessage, CodDevice, MessageDevice) VALUES ";
            var batchSize = 10;

            var queriesToExecute = new List<string>();
            var numberofBatches = (int)Math.Ceiling((double)IoTMessage.Count() / batchSize);

            for (int i = 0; i < numberofBatches; i++)
            {
                var messageToInsert = IoTMessage.Skip(i * batchSize).Take(batchSize);
                var valuesToInsert = IoTMessage.Select(x => $"({new Random().Next(1, 10000)}, 1, '{x}')");
                queriesToExecute.Add(insertQuery + string.Join(',', valuesToInsert));
            }


            //IoTMessage.ForEach(x =>
            //{
            //    IoTMessage.Skip(IoTMessage.Count * batchSize).Take(batchSize)
            //};);
            //IoTMessage.ForEach(x => queriesToExecute.Add(insertQuery + string.Join(',', x)));

            return queriesToExecute;

        }


        public async Task RunTestNewDevice([IoTHubTrigger("messages/events", Connection = "IoTNewDeviceConnection")] EventData[] messages, ILogger log, ExecutionContext context)
        {

            //FunctionConfig.ConnectionString = _connectionStringsOptions.Value.SQLConnectionString;
            FunctionConfig.ConnectionString = Environment.GetEnvironmentVariable("SQLConnectionString");
            //string tempMessage = msg.ToString();
            var messageList = messages.Select(message => Encoding.UTF8.GetString(message.Body.Array)).ToList();


            //List<string> messageList = messageList.Add(tempMessage);
            List<string> sqls = GetSqlInBatches(messageList); // ADD PARAMETER DUE TO WHAT WILL ENTER IN THE LINES ABOVE
            //log.LogInformation($"C# IoT Hub trigger function processed a message: {msg}");
            //_logger.LogInfo($"C# IoT Hub trigger function processed a message: {msg}");


            _logger.LogInfo($"C# IoT Hub trigger function processed a message teste de novo dispositivo: {messages.Length}");
            log.LogInformation($"C# IoT Hub trigger function processed a message teste de novo dispositivo: {messages.Length}");

            using (var uow = _context.Create())
            {

                //string sql = $"INSERT INTO smart_header.IoTMessage (CodMessage, CodDevice, MessageDevice) VALUES ({new Random().Next(1, 10000)}, 1, '{msg}')";
                //int executeQuery = await uow.ExecuteAsync(sql: sql);

                foreach (var sql in sqls)
                {
                    await uow.ExecuteAsync(sql);
                };

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

