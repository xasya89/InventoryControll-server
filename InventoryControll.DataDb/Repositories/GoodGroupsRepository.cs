using Dapper;
using InventoryControll.Domain;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.DataDb.Repositories;

public class GoodGroupsRepository
{
    private readonly MySqlConnection _connection;
    public GoodGroupsRepository(MySqlConnection conn) => _connection = conn;

    public async Task<IEnumerable<GoodGroup>> Get() =>
        await _connection.QueryAsync<GoodGroup>("SELECT * FROM goodgroups ORDER BY name");
}
