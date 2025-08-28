using System.Collections.Concurrent;
using System.Net.WebSockets;
using AutoMapper;
using JuiceZombies.Server.Datas;
using HotFix_UI;
using JuiceZombies.Server.Datas.Config.Scripts;
using JuiceZombies.Server.Utility;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JuiceZombies.Server.Handlers;

public class GachaHandler : HandleBase, ICommandHandler
{
    public GachaHandler(IMapper mapper, MyPostgresDbContext context
    ) : base(mapper, context)
    {
    }

    public async Task<OutputContext> HandleAsync(Context context)
    {
        var message = context.Message;
        var request = MessagePackSerializer.Deserialize<C2S_GachaRequest>(message.Content, options);
        var heroList = MyConfig.Tables.TbItem.DataList.Where(a => a.type == 4).ToList();
        S2C_RewardsData? S2C_RewardsData = null;

        message.Content =
            MessagePackSerializer.Serialize(S2C_RewardsData, options);

        var contextStr = MyHelper.GetInputOutPutStr(request, S2C_RewardsData);
        var output = new OutputContext
        {
            message = message,
            inputContentStr = contextStr.Item1,
            outputContentStr = contextStr.Item2
        };
        return output;
    }
}