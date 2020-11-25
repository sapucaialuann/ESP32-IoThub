using IotHubIngestion.Infra.Data.UoW;
using IoTHubIngestion.Domain.Interfaces.UoW;
using Microsoft.Extensions.Configuration;

namespace Infra.Data.UoW
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private IConfiguration _config;
        private string _connectionString;

        public UnitOfWorkFactory(IConfiguration config)
        {

            this._config = config;
        }

        public UnitOfWorkFactory(string connectionString)
        {
            this._connectionString = connectionString;
        }
        public IUnitOfWork Create()
        {
            if (this._connectionString != "" && this._connectionString != null)
            {
                return new UnitOfWork(this._connectionString);
            }
            return new UnitOfWork(this._config);
        }
    }
}