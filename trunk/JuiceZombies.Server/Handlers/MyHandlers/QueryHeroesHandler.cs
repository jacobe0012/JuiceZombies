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

public class QueryHeroesHandler : HandleBase, ICommandHandler
{
    public QueryHeroesHandler(IMapper mapper, MyPostgresDbContext context
    ) : base(mapper, context)
    {
    }

    public async Task<OutputContext> HandleAsync(Context context)
    {
        var message = context.Message;
        //var request = MessagePackSerializer.Deserialize<C2S_QueryShop>(message.Content, options);
        // _context.GameShops.FindAsync(1);
        // Console.WriteLine($"message1 {message.ToString()}");

        var heroList = await _dataBase.HeroDatas
            .Where(u => u.UserId == context.UserId)
            .ToListAsync();

        var s2cHeroList = _mapper.Map<List<S2C_HeroItemData>>(heroList);
        
        message.Content =
            MessagePackSerializer.Serialize(s2cHeroList, options);

        var contextStr = MyHelper.GetInputOutPutStr(null, s2cHeroList);
        var output = new OutputContext
        {
            message = message,
            inputContentStr = contextStr.Item1,
            outputContentStr = contextStr.Item2
        };
        return output;
    }
}