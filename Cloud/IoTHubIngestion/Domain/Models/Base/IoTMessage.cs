using System;

namespace IoTHubIngestion.Domain.Models {
    public class IoTMessage {

        public int CodMessage {get;set;}
        public int CodDevice {get;set;}
        public string MessageDevice {get;set;}

    }
}