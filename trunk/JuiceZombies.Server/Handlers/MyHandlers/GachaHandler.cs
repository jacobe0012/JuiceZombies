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

    Vector2 GetOneFromPool(int poolId)
    {
        var tbPool = MyConfig.Tables.TbPool.Get(poolId);
        Vector2 getId = default;
        int totalPower = 0;
        foreach (var power in tbPool.power)
        {
            totalPower += (int)power.z;
        }

        int randomValue = Random.Shared.Next(0, totalPower);
        int weightSum = 0;
        foreach (var item in tbPool.power)
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

    Vector2 GetOneFromPool(List<Vector3> poolList)
    {
        Vector2 getId = default;
        int totalPower = 0;
        foreach (var power in poolList)
        {
            totalPower += (int)power.z;
        }

        int randomValue = Random.Shared.Next(0, totalPower);
        int weightSum = 0;
        foreach (var item in poolList)
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

    Vector2 GetOneFromPool(List<Vector2> poolList)
    {
        Vector2 getId = default;
        int totalPower = 0;
        foreach (var power in poolList)
        {
            totalPower += (int)power.y;
        }

        int randomValue = Random.Shared.Next(0, totalPower);
        int weightSum = 0;
        foreach (var item in poolList)
        {
            weightSum += (int)item.y;
            // 检查随机值是否在当前物品的权重范围内
            if (randomValue <= weightSum)
            {
                getId = new Vector2(item.x, item.y);
                break;
            }
        }

        return getId;
    }

    async Task<ItemData> StrogeReward(Vector2 reward, MyPostgresDbContext database, bool isStroge = true,
        bool isStrogeImmediately = false)
    {
        ItemData itemData = null;
        int configId = (int)reward.x;
        int count = (int)reward.y;
        var item = MyConfig.Tables.TbItem.Get(configId);
        var ensureItemType = (ItemType)item.type;
        switch (ensureItemType)
        {
            case ItemType.BagItem:
                itemData = new BagItemData();
                var bagItemData = itemData as BagItemData;
                bagItemData.ConfigId = configId;
                bagItemData.Count = count;
                if (isStroge)
                {
                    database.ItemDatas.Add(bagItemData);
                    if (isStrogeImmediately)
                    {
                        await _dataBase.SaveChangesAsync();
                    }
                }

                break;
            case ItemType.HeroItem:

                itemData = new HeroItemData();
                var heroItemData = itemData as HeroItemData;
                heroItemData.ConfigId = configId;
                heroItemData.Count = count;
                heroItemData.Level = 1;
                if (isStroge)
                {
                    database.ItemDatas.Add(heroItemData);
                    if (isStrogeImmediately)
                    {
                        await _dataBase.SaveChangesAsync();
                    }
                }

                break;
        }

        return itemData;
    }

    Vector2 GetOneGachaId(Gacha tbGacha, ref int counter)
    {
        //ItemData itemData = null;
        Vector2 reward = default;
        if (tbGacha.ensureCount <= counter)
        {
            counter = 0;
            reward = GetOneFromPool(tbGacha.ensurePoolId);
        }
        else
        {
            var pool = GetOneFromPool(tbGacha.pool);
            int poolId = (int)pool.x;
            reward = GetOneFromPool(poolId);
        }

        return reward;
    }

    public async Task<OutputContext> HandleAsync(Context context)
    {
        var message = context.Message;
        var request = MessagePackSerializer.Deserialize<C2S_GachaRequest>(message.Content, options);
        var tbGacha = MyConfig.Tables.TbGacha.Get(request.BoxId);

        GachaPityCounterData gachaData = await _dataBase.GachaPityCounterDatas
            .FirstOrDefaultAsync(u => u.UserId == context.UserId);

        List<S2C_ItemData> S2C_RewardsData = new List<S2C_ItemData>();

        if (gachaData == null)
        {
            gachaData = new GachaPityCounterData
            {
                UserId = context.UserId,
                //Pity_IdCounter = new ConcurrentDictionary<int, int>()
            };
        }

        var tempIdCounter = gachaData.MyPity_IdCounter;


        tempIdCounter.TryAdd(request.BoxId, 0);
        int counter = tempIdCounter[request.BoxId];


        switch (request.Type)
        {
            case 1:
                counter++;

                var reward1 = GetOneGachaId(tbGacha, ref counter);
                var itemData1 = await StrogeReward(reward1, _dataBase);
                var S2C_ItemData1 = _mapper.Map<S2C_ItemData>(itemData1);
                S2C_RewardsData.Add(S2C_ItemData1);

                //TODO:入库
                break;
            case 2:


                for (int i = 0; i < 10; i++)
                {
                    counter++;
                    var reward2 = GetOneGachaId(tbGacha, ref counter);
                    var itemData2 = await StrogeReward(reward2, _dataBase);
                    var S2C_ItemData2 = _mapper.Map<S2C_ItemData>(itemData2);
                    S2C_RewardsData.Add(S2C_ItemData2);
                }

                break;
        }

        tempIdCounter[request.BoxId] = counter;
        gachaData.MyPity_IdCounter = tempIdCounter;

        var a = JsonConvert.SerializeObject(gachaData);
        Console.WriteLine($"gachaData:{a}");
        _dataBase.GachaPityCounterDatas.Update(gachaData);

        await _dataBase.SaveChangesAsync();

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