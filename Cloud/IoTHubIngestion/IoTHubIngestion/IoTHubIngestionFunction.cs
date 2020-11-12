using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using IoTHubIngestion.Domain.Interfaces.UoW;
using System.Threading.Tasks;

namespace IoTHubIngestion
{
    public class IoTHubIngestionFunction
    {
        private IUnitOfWorkFactory _context;

        public IoTHubIngestionFunction(IUnitOfWorkFactory context)
        {
            _context = context;
        }

        [FunctionName("IoTHubIngestionFunction")]
        public async Task Run([IoTHubTrigger("messages/events", Connection = "IotHubConnectionString")]EventData message, ILogger log)
        {
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");
            using (var uow = _context.Create() )
            {
                var res = await uow.QueryAsync<dynamic>("SELECT * FROM Table", null);
            }
            
        }
    }
}