using System.Collections.Concurrent;
using System.Net.WebSockets;
using AutoMapper;
using cfg.config;
using JuiceZombies.Server.Datas;
using HotFix_UI;
using JuiceZombies.Server.Datas.Config.Scripts;
using JuiceZombies.Server.Utility;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using UnityEngine;

namespace JuiceZombies.Server.Handlers;

public class GachaHandler : HandleBase, ICommandHandler
{
    public GachaHandler(IMapper mapper, MyPostgresDbContext context
    ) : base(mapper, context)
    {
    }

    Vector2 PreGetOneGachaId(Gacha tbGacha, int totalPower)
    {
        Vector2 getId = default;

        int randomValue = Random.Shared.Next(0, totalPower);
        int weightSum = 0;
        foreach (var item in tbGacha.power)
        {
            weightSum += (int)item.z;
            // 检查随机值是否在当前物品的权重范围内
            if (randomValue <= weightSum)
            {
                getId = new Vector2(item.x, item.y);
                break;
            }
        }

        return getId;
    }

    Vector2 GetOneGachaId(Gacha tbGacha, ref int counter, int totalPower)
    {
        Vector2 id;
        if (tbGacha.ensureCount <= counter)
        {
            counter = 0;
            id = PreGetOneGachaId(tbGacha, totalPower);
            if ((int)id.x == 4)
            {
                var heroList = MyConfig.Tables.TbItem.DataList.Where(a => a.type == 4).ToList();
                heroList = heroList.OrderBy(x => Random.Shared.NextInt64()).ToList();
                id = new Vector2(heroList[0].id, 1);
            }
        }
        else
        {
            var heroList = MyConfig.Tables.TbItem.DataList.Where(a => a.type == 4).ToList();
            heroList = heroList.OrderBy(x => Random.Shared.NextInt64()).ToList();
            id = new Vector2(heroList[0].id, 1);
        }

        return id;
    }

    public async Task<OutputContext> HandleAsync(Context context)
    {
        var message = context.Message;
        var request = MessagePackSerializer.Deserialize<C2S_GachaRequest>(message.Content, options);
        var tbGacha = MyConfig.Tables.TbGacha.Get(request.BoxId);

        GachaData gachaData = await _dataBase.GachaDatas
            .FirstOrDefaultAsync(u => u.UserId == context.UserId);

        S2C_RewardsData? S2C_RewardsData = new S2C_RewardsData
        {
            Result = new List<Vector2>()
        };

        if (gachaData == null)
        {
            gachaData = new GachaData
            {
                UserId = context.UserId,
                Pity_IdCounter = new ConcurrentDictionary<int, int>()
            };
        }

        gachaData.Pity_IdCounter.TryAdd(request.BoxId, 0);
        int counter = gachaData.Pity_IdCounter[request.BoxId];

        int totalPower = 0;
        foreach (var power in tbGacha.power)
        {
            totalPower += (int)power.y;
        }


        switch (request.Type)
        {
            case 1:
                counter++;

                var id = GetOneGachaId(tbGacha, ref counter, totalPower);
                S2C_RewardsData.Result.Add(id);

                //TODO:入库
                break;
            case 2:

                List<Vector2> result = new List<Vector2>(10);
                for (int i = 0; i < 10; i++)
                {
                    counter++;
                    var id0 = GetOneGachaId(tbGacha, ref counter, totalPower);
                    result.Add(id0);
                }

                S2C_RewardsData.Result.AddRange(result);

                //TODO:入库
                break;
        }

        gachaData.Pity_IdCounter[request.BoxId] = counter;
        _dataBase.GachaDatas.Update(gachaData);


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