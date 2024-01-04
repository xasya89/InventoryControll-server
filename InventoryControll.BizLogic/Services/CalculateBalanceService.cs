using Dapper;
using InventoryControll.BizLogic.BizLogic;
using InventoryControll.DataDb;
using InventoryControll.Domain.NoEntityModels;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryControll.BizLogic.Services;


public interface ICalculateBalance
{
    Task<IEnumerable<BalanceItem>> Calculate(DateTime? inventoryDateBefore = null);
}
public class CalculateBalanceService: ICalculateBalance
{
    private MySqlConnection _connection;
    public CalculateBalanceService(ShopUnitOfWork context) => _connection = context.Connection;

    public async Task<IEnumerable<BalanceItem>> Calculate(DateTime? inventoryDateBefore = null) =>
        await new GetCalculateBalance(_connection).GetBalance(inventoryDateBefore);
}
