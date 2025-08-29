using System.Collections.Concurrent;
using System.Net.WebSockets;
using AutoMapper;
using JuiceZombies.Server.Datas;
using HotFix_UI;
using JuiceZombies.Server.Utility;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JuiceZombies.Server.Handlers;

public class QueryShopDataHandler : HandleBase, ICommandHandler
{
    public QueryShopDataHandler(IMapper mapper, MyPostgresDbContext context
    ) : base(mapper, context)
    {
    }

    public async Task<OutputContext> HandleAsync(Context context)
    {
        var message = context.Message;
        //var request = MessagePackSerializer.Deserialize<C2S_QueryShop>(message.Content, options);
        // _context.GameShops.FindAsync(1);
        // Console.WriteLine($"message1 {message.ToString()}");
        ShopData? shopData;

        shopData = await _dataBase.ShopDatas
            .FirstOrDefaultAsync(u => u.UserId == context.UserId);

        // 如果没有找到，直接返回
        if (shopData == null)
        {
            shopData = new ShopData
            {
                UserId = context.UserId,
                IsBuyADCard = false,
                IsBuyMonthCard = false,
                BuyedMonthCardms = 0
            };
            _dataBase.ShopDatas.Add(shopData);
            await _dataBase.SaveChangesAsync();
        }

        var S2C_ShopData = _mapper.Map<S2C_ShopData>(shopData);

        message.Content =
            MessagePackSerializer.Serialize(S2C_ShopData, options);
        
        var contextStr = MyHelper.GetInputOutPutStr(null, S2C_ShopData);
        var output = new OutputContext
        {
            message = message,
            inputContentStr = contextStr.Item1,
            outputContentStr = contextStr.Item2
        };
        return output;
    }
}