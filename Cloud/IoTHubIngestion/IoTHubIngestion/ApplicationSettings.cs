using System;
using System.Collections.Generic;
using System.Text;

namespace IoTHubIngestion
{
    //my edition from now on

    public class ApplicationSettings
    {
        public string AzureWebJobsStorage { get; set; }
        public string IotHubConnectionString { get; set; }
    }

    public class ConnectionStrings
    {
        public string SQLConnectionString { get; set; }
    }

}
