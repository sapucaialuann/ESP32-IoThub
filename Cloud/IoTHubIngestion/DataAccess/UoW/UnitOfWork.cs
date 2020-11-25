using Dapper;
using IoTHubIngestion.Domain.Interfaces.UoW;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace IotHubIngestion.Infra.Data.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        private bool _disposed;
        private readonly string _connectionString;


        public UnitOfWork(IConfiguration config)
        {
            _connectionString = config["Data:DefaultConnection:ConnectionString"];
            _connection = new SqlConnection(_connectionString);
            _connection.Open();

        }

        public UnitOfWork(string connectionString)
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }

        public IDbConnection Connection()
        {
            return _connection;
        }

        public void BeginTransaction(IsolationLevel level = IsolationLevel.ReadCommitted)
        {
            _transaction = _connection.BeginTransaction(level);
        }

        public IDbTransaction Transaction()
        {
            return _transaction;
        }
        public void Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
            }
        }

        public int Execute(string sql, object param = null)
        {
            return _connection.Execute(sql, param, _transaction);
        }

        public Task<int> ExecuteAsync(string sql, object param = null)
        {
            return _connection.ExecuteAsync(sql, param, _transaction);
        }

        public IEnumerable<T> Query<T>(string sql, object args)
        {
            return _connection.Query<T>(sql, args, _transaction);
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object args)
        {
            return _connection.QueryAsync<T>(sql, args, _transaction);
        }

        public async Task<long> CountAsync(string sql, object args)
        {
            return (await _connection.QueryAsync<long>($"select count(*) as total from ({sql}) as query", args, _transaction)).AsList().First();
        }

        public long Count(string sql, object args)
        {
            return ( _connection.Query<long>($"select count(*) as total from ({sql}) as query", args, _transaction)).AsList().First();
        }

        public void Rollback()
        {

            _transaction.Rollback();
            _transaction.Dispose();

        }

        public void AsyncCommit(Task task)
        {
            if (task.Exception != null)
            {
                Rollback();
                throw task.Exception;
            }
            else
            {
                Commit();
            }
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        private void dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }
                    if (_connection != null)
                    {
                        _connection.Dispose();
                        _connection = null;
                    }
                }
                _disposed = true;
            }
        }

        ~UnitOfWork()
        {
            dispose(false);
        }
    }
}