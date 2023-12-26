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
    public class ShopUnitOfWork
    {
        public GoodsRepository Goods { get => new GoodsRepository(_connection); }

        private readonly MySqlConnection _connection;
        public MySqlConnection Connection { get => _connection; }
        public ShopUnitOfWork(ITenantService tenant, IConfiguration configuration)
        {
            var name = tenant.Tenant;
            string conStr = configuration.GetConnectionString(name);
            _connection = new MySqlConnection(conStr);
        }
    }
}
