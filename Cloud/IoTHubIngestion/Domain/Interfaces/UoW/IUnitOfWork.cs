using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace IoTHubIngestion.Domain.Interfaces.UoW
{
    public interface IUnitOfWork : IDisposable
    {

        Task<int> ExecuteAsync(string sql, object param = null);

        Task<IEnumerable<T>> QueryAsync<T>(string sql, object args);
        IEnumerable<T> Query<T>(string sql, object args);
        int Execute(string sql, object param = null);
        void Commit();
        void BeginTransaction(IsolationLevel level = IsolationLevel.ReadCommitted);
        IDbConnection Connection();
        Task<long> CountAsync(string sql, object args);
        long Count(string sql, object args);
    }
}
