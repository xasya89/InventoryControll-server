using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using InventoryControll.DataDb.Repositories;
using Microsoft.Extensions.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace InventoryControll.DataDb
{
    public class ShopUnitOfWork: IDisposable
    {
        public GoodsRepository Goods { get => new GoodsRepository(Connection); }
        public StocktakingRepository Stocktakings { get => new StocktakingRepository(Connection); }

        private readonly MySqlConnection _connection;
        private MySqlTransaction _transaction;
        public MySqlConnection Connection { get
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                    _connection.Open();
                return _connection;
            } }

        public ShopUnitOfWork(ITenantService tenant, IConfiguration configuration)
        {
            var name = tenant.Tenant;
            string conStr = configuration.GetConnectionString(name);
            _connection = new MySqlConnection(conStr);
        }

        public async Task StartTransaction()
        {
            _transaction = await _connection.BeginTransactionAsync();
        }
        public async Task Commit()
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }
        public async Task Rollback()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }


        public void Dispose()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
                _connection.Close();
        }
    }
}
