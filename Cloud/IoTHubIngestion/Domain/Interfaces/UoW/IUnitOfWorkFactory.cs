using System.Data;

namespace IoTHubIngestion.Domain.Interfaces.UoW {
    public interface IUnitOfWorkFactory {
        IUnitOfWork Create();
    }
}