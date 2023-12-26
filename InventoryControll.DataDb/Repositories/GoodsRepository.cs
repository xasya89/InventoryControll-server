using InventoryControll.Domain;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace InventoryControll.DataDb.Repositories;

public class GoodsRepository
{
    private readonly MySqlConnection _connection;
    public GoodsRepository(MySqlConnection conn) => _connection = conn;

    public async Task<IEnumerable<Good>> Get(int? take=100, int? skip=0)
    {
        var goods = await _connection.QueryAsync<Good>("SELECT * FROM goods ORDER BY Name LIMIT @Skip, @Take",
            new { Skip = skip, Take = take });
        var barcodes = await _connection.QueryAsync<Barcode>("SELECT * FROM barcodes");
        foreach(var good in goods)
            good.Barcodes = barcodes.Where(x=>x.GoodId== good.Id);
        return goods;
    }

    public async Task<int> Count() => await _connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM goods");
}
