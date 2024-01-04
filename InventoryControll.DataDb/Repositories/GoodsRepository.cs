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

    public async Task<IEnumerable<Good>> GetAll()
    {
        var goods = await _connection.QueryAsync<Good>("SELECT * FROM goods ORDER BY Name");
        var barcodes = await _connection.QueryAsync<Barcode>("SELECT * FROM barcodes");
        foreach (var good in goods)
            good.Barcodes = barcodes.Where(x => x.GoodId == good.Id);
        return goods;
    }
    public async Task<IEnumerable<Good>> Get(int? take=100, int? skip=0)
    {
        var goods = await _connection.QueryAsync<Good>("SELECT * FROM goods ORDER BY Name LIMIT @Skip, @Take",
            new { Skip = skip, Take = take });
        var barcodes = await _connection.QueryAsync<Barcode>("SELECT * FROM barcodes");
        foreach(var good in goods)
            good.Barcodes = barcodes.Where(x=>x.GoodId== good.Id);
        return goods;
    }

    public async Task<Good?> Get(Guid uuid)
    {
        var good = await _connection.QueryFirstOrDefaultAsync<Good>("SELECT * FROM goods WHERE uuid=@Uuid",
            new { Uuid = uuid });
        if(good==null) return null;
        var barcodes = await _connection.QueryAsync<Barcode>("SELECT * FROM barcodes WHERE GoodId=@GoodId",
            new { GoodId = good?.Id });
        good.Barcodes = barcodes;
        return good;
    }

    public async Task<Good?> Get(int id)
    {
        var good = await _connection.QueryFirstOrDefaultAsync<Good>("SELECT * FROM goods WHERE id=@Id",
            new { Id = id });
        if (good == null) return null;
        var barcodes = await _connection.QueryAsync<Barcode>("SELECT * FROM barcodes WHERE GoodId=@GoodId",
            new { GoodId = good?.Id });
        good.Barcodes = barcodes;
        return good;
    }

    public async Task<int> Count() => await _connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM goods");

    public async Task<int> CountWithoutDeleted() => await _connection.QuerySingleAsync<int>("SELECT COUNT(*) FROM goods WHERE IsDeleted=0");
}
