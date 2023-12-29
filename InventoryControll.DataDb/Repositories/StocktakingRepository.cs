using Dapper;
using InventoryControll.Domain;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.DataDb.Repositories
{
    public class StocktakingRepository
    {
        private readonly MySqlConnection _connection;
        public StocktakingRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

        public async Task<Stocktaking> GetByGroup(int id)
        {
            var stocktaking = await _connection.QueryFirstAsync<Stocktaking>("SELECT * FROM stocktakings WHERE id=@Id", new { Id = id });
            var groups = await _connection.QueryAsync<StockTakingGroup>("SELECT * FROM stocktakinggroups WHERE StocktakingId=@Id", new { Id = id });
            stocktaking.Groups = groups;
            var ids = groups.Select(s => s.Id).ToList();
            var goods = await _connection.QueryAsync<StocktakingGood>("SELECT * FROM stocktakinggoods WHERE StockTakingGroupId IN @GroupsId",
                new { GroupsId = ids });
            foreach(var group in groups)
                group.Goods= goods.Where(x=>x.StockTakingGroupId==group.Id).ToList();
            return stocktaking;
        }

        public async Task<Stocktaking> GetWithSummary(int id)
        {
            var stocktaking = await _connection.QueryFirstAsync<Stocktaking>("SELECT * FROM stocktakings WHERE id=@Id", new { Id = id });
            var summary = await _connection.QueryAsync<StocktakingSummary>("SELECT * FROM stocktakingsummarygoods WHERE StocktakingId=@Id", new { Id = id });
            stocktaking.Summary = summary;
            return stocktaking;
        }

        public async Task<int> Add(Stocktaking stocktaking)
        {
            int lastNum = await _connection.QueryFirstOrDefaultAsync<int>("SELECT MAX(Num) FROM stocktakings");
            lastNum++;
            var id = await _connection.QueryFirstAsync<int>(@"INSERT INTO stocktakings (Num, Uuid, Start, ShopId, isSuccess, Status, CountDb, CountFact, SumDb, SumFact, CashMoneyDb, CashMoneyFact)
                VALUES (@Num, @Uuid, @Start, 1, 1, 2, @CountDb, @CountFact, @SumDb, @SumFact, @CashMoneyDb, @CashMoneyFact);
                SELECT LAST_INSERT_ID();"
            ,
                new
                {
                    Num = lastNum,
                    Create = stocktaking.Create,
                    Uuid = stocktaking.Uuid,
                    Start = stocktaking.Start,
                    CountDb = stocktaking.CountDb,
                    CountFact = stocktaking.CountFact,
                    SumDb = stocktaking.SumDb,
                    SumFact = stocktaking.SumFact,
                    CashMoneyDb = stocktaking.CashMoneyDb,
                    CashMoneyFact = stocktaking.CashMoneyFact
                });
            stocktaking.Id = id;
            return id;
        }


        public async Task<int> Add(StockTakingGroup group)
        {
            var id = await _connection.QueryFirstAsync<int>(@"INSERT INTO stocktakinggroups (name, StocktakingId) VALUES (@Name, @StocktakingId);
                SELECT LAST_INSERT_ID();",
                new { Name = group.Name, StocktakingId = group.StocktakingId });
            group.StocktakingId = id;
            return id;
        }

        public async Task<int> Add(StocktakingGood position)
        {
            int id = await _connection.QueryFirstAsync<int>(@"INSERT INTO stocktakinggoods (GoodId, StockTakingGroupId, CountFact, Price, CountDB, Count)
                VALUES (@GoodId, @StockTakingGroupId, @CountFact, @Price, 0, 0);
                SELECT LAST_INSERT_ID();",
                new { GoodId = position.GoodId, StockTakingGroupId = position.StockTakingGroupId, CountFact = position.CountFact, Price = position.Price });
            position.Id= id;
            return id;
        }

        public async Task<int> Add(StocktakingSummary summary)
        {
            int id = await _connection.QueryFirstAsync<int>(@"INSERT INTO stocktakingsummarygoods (StocktakingId, GoodId, CountDb, CountFact. Price) VALUES 
                VALUES (@StocktakingId, @GoodId, @CountDb, @CountFact, @Price);
                SELECT LAST_INSERT_ID();",
                new { StocktakingId=summary.StocktakingId, GoodId=summary.GoodId, CountDb=summary.CountDb, CountFact=summary.CountFact, Price=summary.Price });
            summary.Id= id;
            return id;
        }

        public async Task Update(Stocktaking stocktaking) =>
            await _connection.ExecuteAsync("UPDATE stocktakings s SET s.Num=@Num, s.Create=@Create WHERE s.id=@Id",
                new { Id = stocktaking.Id, Num = stocktaking.Num, Create = stocktaking.Create });
    }
}

