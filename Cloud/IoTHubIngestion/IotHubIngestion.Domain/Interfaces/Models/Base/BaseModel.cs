using System;

namespace IoTHubIngestion.Domain.Models {
    public abstract class BaseModel {

        public Guid Id {get;set;}

        public BaseModel() {
            //Id = Guid.NewGuid();
        }
    }
}