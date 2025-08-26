using System.Collections.Concurrent;
using System.Net.WebSockets;
using JuiceZombies.Server.Datas;
using HotFix_UI;
using MessagePack;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JuiceZombies.Server.Handlers;

public class QueryGameShopHandler : HandleBase, ICommandHandler<C2S_QueryShop>, ICommandHandler
{
    public QueryGameShopHandler(MyPostgresDbContext context, IConnectionMultiplexer redis
    ) : base(context, redis)
    {
    }

    public Task<Context> HandleAsync(C2S_QueryShop command)
    {
        Console.WriteLine($"QueryGameShopHandler");
        return default;
    }

    public Task<Context> HandleAsync(object command)
    {
        Console.WriteLine($"QueryGameShopHandler1");
        return HandleAsync((C2S_QueryShop)command);
    }

    public async Task<Context> HandleAsync(MyMessage message, WebSocket webSocket)
    {
        // _context.GameShops.FindAsync(1);
        // Console.WriteLine($"message1 {message.ToString()}");
        S2C_ShopData gameShop;
        // if (!_connections.TryGetValue(webSocket, out var openId))
        // {
        //     //Console.WriteLine($"webSocket:{webSocket.} not found");
        //     //用户未登记
        //     message.ErrorCode = 1001;
        // }
        //
        // var db = _redis.GetDatabase();
        //var rv = await db.StringGetAsync(GetRedisDBStr(1, openId));
        //gameShop = JsonConvert.DeserializeObject<GameShop>(rv);
        gameShop = new S2C_ShopData
        {
            isBuyADCard = false,
            isBuyMonthCard = false,
            buyedMonthCardms = 21312123123123
        };


        message.Content =
            MessagePackSerializer.Serialize(gameShop, options);

        var outputContentStr = gameShop.ToString();

        var context = new Context
        {
            message = message,
            inputContentStr = "",
            outputContentStr = outputContentStr
        };
        return context;
    }
}