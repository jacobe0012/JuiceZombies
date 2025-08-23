using System.Collections.Concurrent;
using System.Net.WebSockets;
using JuiceZombies.Server.Datas;
using HotFix_UI;
using MessagePack;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JuiceZombies.Server.Handlers;

public class QueryGameShopHandler : HandleBase, ICommandHandler
{
    public QueryGameShopHandler(IConnectionMultiplexer redis,
        ConcurrentDictionary<WebSocket, string> connections) :
        base(redis, connections)
    {
    }

    public async Task<Context> HandleAsync(MyMessage message, WebSocket webSocket)
    {
        Console.WriteLine($"message1 {message.ToString()}");
        GameShop gameShop;
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
        gameShop = new GameShop
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