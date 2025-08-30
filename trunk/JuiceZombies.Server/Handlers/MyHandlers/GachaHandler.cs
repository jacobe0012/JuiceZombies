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

    ItemData Reward2ItemData(Vector2 reward, uint UserId = 0)
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

                break;
            case ItemType.HeroItem:

                itemData = new HeroItemData();
                var heroItemData = itemData as HeroItemData;
                heroItemData.Level = 1;


                break;
        }

        itemData.ItemType = ensureItemType;
        itemData.ConfigId = configId;
        itemData.Count = count;
        itemData.UserId = UserId;


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
            if (poolId == tbGacha.ensurePoolId)
                counter = 0;

            reward = GetOneFromPool(poolId);
        }

        return reward;
    }

    public List<ItemData> MergeItems(List<ItemData> sourceItems)
    {
        var mergedItems = new List<ItemData>();

        var groupedItems = sourceItems.GroupBy(item => item.ConfigId);

        foreach (var group in groupedItems)
        {
            var firstItem = group.First();
            var tbItem = MyConfig.Tables.TbItem.Get(firstItem.ConfigId);
            if (tbItem.pileYn == 1)
            {
                var totalCount = group.Sum(item => item.Count);

                firstItem.ConfigId = firstItem.ConfigId;
                firstItem.Count = totalCount;

                mergedItems.Add(firstItem);
            }
            else
            {
                foreach (var item in group)
                {
                    mergedItems.Add(item);
                }
            }
        }

        return mergedItems;
    }

    public async Task<OutputContext> HandleAsync(Context context)
    {
        var message = context.Message;
        var request = MessagePackSerializer.Deserialize<C2S_GachaRequest>(message.Content, options);
        var tbGacha = MyConfig.Tables.TbGacha.Get(request.BoxId);

        GachaPityCounterData gachaData = await _dataBase.GachaPityCounterDatas
            .FirstOrDefaultAsync(u => u.UserId == context.UserId);

        List<ItemData> itemDatas = new List<ItemData>();

        if (gachaData == null)
        {
            gachaData = new GachaPityCounterData
            {
                UserId = context.UserId,
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
                var itemData1 = Reward2ItemData(reward1, context.UserId);
                itemDatas.Add(itemData1);

                break;
            case 2:


                for (int i = 0; i < 10; i++)
                {
                    counter++;
                    var reward2 = GetOneGachaId(tbGacha, ref counter);
                    var itemData2 = Reward2ItemData(reward2, context.UserId);
                    itemDatas.Add(itemData2);
                }

                break;
        }

        var S2C_ItemDatas = _mapper.Map<List<S2C_ItemData>>(itemDatas);

        tempIdCounter[request.BoxId] = counter;
        
        gachaData.MyPity_IdCounter = tempIdCounter;
        _dataBase.GachaPityCounterDatas.Update(gachaData);

        var confirmedItemDatas = MergeItems(itemDatas);
        foreach (var item in confirmedItemDatas)
        {
            var itemData = await _dataBase.ItemDatas.FirstOrDefaultAsync(a =>
                a.ConfigId == item.ConfigId && a.UserId == item.UserId);
            if (itemData == null)
            {
                _dataBase.ItemDatas.Add(item);
            }
            else
            {
                itemData.Count += item.Count;
            }
        }

        await _dataBase.SaveChangesAsync();
        //await _dataBase.SaveChangesAsync();

        message.Content =
            MessagePackSerializer.Serialize(S2C_ItemDatas, options);

        var a = JsonConvert.SerializeObject(gachaData);
        Console.WriteLine($"gachaData:{a}");

        var contextStr = MyHelper.GetInputOutPutStr(request, S2C_ItemDatas);
        var output = new OutputContext
        {
            message = message,
            inputContentStr = contextStr.Item1,
            outputContentStr = contextStr.Item2
        };
        return output;
    }
}