// using Main;
// using UnityEngine;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using cfg.blobstruct;
// using Unity.Transforms;
// using System;
// using ProjectDawn.Navigation;
// using System.Linq;
// using Unity.Entities.UniversalDelegates;
//
//
// //[DisableAutoCreation]
// [UpdateInGroup(typeof(InitializationSystemGroup))]
// //[DisableAutoCreation]
// public partial struct AreaAnd0bstacleGenerateSystem : ISystem
// {
//     int currentMapIndex;
//
//
//     [BurstCompile]
//     public void OnCreate(ref SystemState state)
//     {
//         state.Enabled = false;
//     }
//
//     [BurstCompile]
//     public void OnDestroy(ref SystemState state)
//     {
//     }
//
//
//     public void OnUpdate(ref SystemState state)
//     {
//         //state.Enabled = false;
//
//         Debug.Log($"222222222222222  OnUpdateAreaAnd0bstacleGenerateSystem");
//         if (!SystemAPI.TryGetSingleton<GlobalConfigData>(out var configData))
//         {
//             return;
//         }
//
//
//         //拿到刷新id
//         int levelID = SystemAPI.GetSingleton<GameOthersData>().levelId;
//         var queryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<MapElementData>()
//             .WithOptions(EntityQueryOptions.IncludePrefab);
//
//         var elementQuery = state.GetEntityQuery(queryBuilder);
//
//         ref var refreshGroup =
//             ref configData.value.Value.configTblevels.configTblevels[levelID - 10001].mapRefreshId;
//         // var refreshIDs = UtilityClass.BlobArrayToNativeArray<int>(ref refreshGroup);
//
//
//         ref var refreshArray = ref configData.value.Value.configTbmap_refreshs.configTbmap_refreshs;
//         ref var elementsArray = ref configData.value.Value.configTbmap_modules.configTbmap_modules;
//         ref var mapTable = ref configData.value.Value.configTbmap_types.configTbmap_types;
//
//
//         //当前需要对第几张地图进行操作就拿改地图的id(ps 正常情况下 一个关卡所有地图id都是相同的)
//         DynamicBuffer<MapIndexPosBuffer> mapPosBuffer = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>();
//         currentMapIndex = mapPosBuffer[mapPosBuffer.Length - 1].mapIndex;
//
//         var mapGlobalData= SystemAPI.GetSingleton<MapGloablaData>();
//         var mapID = mapGlobalData.mapID;
//
//         //假设独占的所有坐标均不重叠 分配最大空间
//         int maxRectCount = 0;
//         for (int i = 0; i < refreshArray.Length; i++)
//         {
//             if (refreshArray[i].selfYn == 0)
//             {
//                 maxRectCount += refreshArray[i].num;
//             }
//         }
//
//         //记录第j行的刷新rect 用来判断独占
//         NativeList<Rect> outRects =
//             new NativeList<Rect>(maxRectCount,
//                 Allocator.Temp);
//
//         //读取刷新组
//         for (int i = 0; i < refreshGroup.Length; i++)
//         {
//             int id = refreshGroup[i];
//             for (int j = 0; j < refreshArray.Length; j++)
//             {
//                 //刷新组相等的话
//                 if (refreshArray[j].id == id)
//                 {
//                     //Debug.Log($"生成{j + 1}次");
//                     int refreshType;
//
//                     if (refreshArray[j].element.Length > 1)
//                     {
//                         refreshType = 3; //成组刷新
//                     }
//                     else
//                     {
//                         if (JudgeType(refreshArray[j].element[0].x) == 1)
//                             refreshType = 1; //area
//                         else if (JudgeType(refreshArray[j].element[0].x) == 2)
//                             refreshType = 2; //obstacle
//                         else
//                         {
//                             Debug.Log("错误的刷新组分组");
//                             return;
//                         }
//                     }
//
//                     // var mapConfig = SystemAPI.GetSingleton<MapGridConfig>();
//                     //拿到一个entity数组 并添加基础数据
//                     NativeList<Entity> outEntities;
//                     GetEntity(elementQuery, ref refreshArray[j].element, out outEntities,
//                         ref state);
//                
//
//
//
//                     //第j行的规则适用 拿到第j行的刷新位置
//                     NativeList<float2> outPositions;
//                     outPositions = RefreshItem(ref outRects, ref refreshArray[j], refreshType, ref state);
//
//
//                     NativeHashMap<Entity, float3> itemPosDic =
//                         new NativeHashMap<Entity, float3>(outPositions.Length, Allocator.Temp);
//
//                     InitEntities(ref state, outPositions, outEntities, refreshType, ref refreshArray[j],
//                         ref itemPosDic);
//
//                     var mapConfigEntity = SystemAPI.GetSingletonEntity<MapIndexPosBuffer>();
//                     if (!state.EntityManager.HasComponent<ItemPosDicData>(mapConfigEntity))
//                     {
//                         state.EntityManager.AddBuffer<ItemPosDicData>(mapConfigEntity);
//                     }
//
//                     var dic = state.EntityManager.GetBuffer<ItemPosDicData>(mapConfigEntity);
//                     foreach (var item in itemPosDic)
//                     {
//                         dic.Add(new ItemPosDicData { entity = item.Key, pos = item.Value });
//                     }
//                 }
//             }
//         }
//
//         // var initSysGroup =
//         //     World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<InitializationSystemGroup>();
//         // initSysGroup.RemoveSystemFromUpdateList(state.SystemHandle);
//         state.Enabled = false;
//     }
//
//     private void AjustEntityScale(ref SystemState state, ref NativeArray<Entity> outEntities)
//     {
//         //Debug.Log($"AjustEntityScale{outEntities.Length}");
//         var configData = SystemAPI.GetSingleton<GlobalConfigData>();
//         ref var elementArray = ref configData.value.Value.configTbmap_modules.configTbmap_modules;
//         foreach (var item in outEntities)
//         {
//             var id = SystemAPI.GetComponent<MapElementData>(item).elementID;
//             for (int i = 0; i < elementArray.Length; i++)
//             {
//                 if (id == elementArray[i].id)
//                 {
//                     var size = elementArray[i].size[0] > elementArray[i].size[1]
//                         ? elementArray[i].size[0]
//                         : elementArray[i].size[1];
//                     var loc = SystemAPI.GetComponentRW<LocalTransform>(item);
//                     
//                     loc.ValueRW.Scale = size;
//                     //loc.ValueRW.Scale = 10;
//                     // var scale = new PostTransformMatrix
//                     //     { Value = float4x4.Scale(elementArray[i].size[0], elementArray[i].size[1], 1) };
//                     // state.EntityManager.AddComponentData(item, scale);
//                     
//                    
//                     //Debug.LogError($"00:{elementArray[i].size[0]},11:{elementArray[i].size[1]}");
//                     if (SystemAPI.HasComponent<AgentShape>(item))
//                     {
//                         var agent = SystemAPI.GetComponentRW<AgentShape>(item);
//                         agent.ValueRW.Radius = size;
//                     }
//                 }
//             }
//         }
//     }
//
//     public int JudgeType(int id)
//     {
//         id /= 1000;
//         if (id == 1 || id == 2)
//             return id;
//         else return -1;
//     }
//
//
//     public void AddDataToElement(Entity entity, ref SystemState state)
//     {
//         //Debug.LogError("AddDataToElement");
//         var elementID = state.EntityManager.GetComponentData<MapElementData>(entity).elementID;
//         var configData = SystemAPI.GetSingleton<GlobalConfigData>();
//         ref var elementArray = ref configData.value.Value.configTbmap_modules.configTbmap_modules;
//         int hp = 0;
//         ref var eventsTable = ref configData.value.Value.configTbevent_0s.configTbevent_0s;
//         for (int i = 0; i < elementArray.Length; i++)
//         {
//             if (elementID == elementArray[i].id)
//             {
//                 hp = elementArray[i].hp;
//                 ref var events = ref elementArray[i].event0;
//                 if (events.Length <= 0) return;
//                 var target = HandleEventID(ref state, ref events);
//
//                 //Debug.Log($"target:{target}");
//                 for (int j = 0; j < events.Length; j++)
//                 {
//                     int eventID = events[j];
//                     AddEventTo(eventID, entity, ref state, configData);
//                 }
//
//                 break;
//             }
//         }
//         if (JudgeType(elementID) == 2)
//         {
//             if (hp != 0)
//             {
//                 Debug.LogError("add hp");
//                 state.EntityManager.AddComponentData(entity, new ChaStats
//                 {
//                     chaProperty = new ChaProperty
//                     {
//                         maxHp = hp,
//                         hp = hp,
//                     },
//                     chaControlState = default
//                 });
//                 state.EntityManager.AddComponent<DamageInfo>(entity);
//                 state.EntityManager.AddComponent<Buff>(entity);
//             }
//             //给碰撞体增加ChaStats组件 和可承受伤害组件
//
//             state.EntityManager.AddComponent<ObstacleTag>(entity);
//         }
//         else
//         {
//             state.EntityManager.AddComponent<AreaTag>(entity);
//         }
//
//
//         var elementData = new MapElementData { };
//
//
//         ////初始化地形信息
//         elementData.belongsIndex = currentMapIndex;
//         elementData.elementID = elementID;
//         //给area添加基础信息
//         state.EntityManager.AddComponentData(entity, elementData);
//
//         state.EntityManager.RemoveComponent<MapElementData>(entity);
//
//     
//     }
//
//     private int HandleEventID(ref SystemState state, ref BlobArray<int> events)
//     {
//         var configData = SystemAPI.GetSingleton<GlobalConfigData>();
//         ref var elementArray = ref configData.value.Value.configTbevent_0s.configTbevent_0s;
//         int target = 0;
//
//      
//         for(int i = 0; i < events.Length; i++)
//         {
//            for(int j=0; j < elementArray.Length; j++)
//             {
//                 if (elementArray[j].id == events[i])
//                 {
//                     switch(elementArray[j].type)
//                     {
//                         case 1:
//                         case 2:
//                         case 11:
//                         case 3:target = 0;break;
//
//                         case 4:target = 63;break;
//                         case 5:target = 1;break;
//                         case 6:target = 63;break;
//                         case 7:target = 60;break;
//                         case 8:target = 63;break;      
//                         case 9:target = 1; break;
//                         case 10:target = 63; break;
//                         
//                     }
//                     break;
//                 }
//             }
//         }
//
//
//         //if (JudgeContainOnly(arrayTemp, 1001, 1003))
//         //{
//         //    for (int i = 0; i < events.Length; i++)
//         //    {
//         //        for (int j = 0; j < elementArray.Length; j++)
//         //        {
//         //            if (elementArray[j].id == events[i])
//         //            {
//         //                target += elementArray[j].target;
//         //            }
//         //        }
//         //    }
//         //    arrayTemp.Clear();
//         //    arrayTemp.Add(1001);
//         //}
//         //else if (JudgeContainOnly(arrayTemp, 1004, 1006))
//         //{
//         //    for (int i = 0; i < events.Length; i++)
//         //    {
//         //        for (int j = 0; j < elementArray.Length; j++)
//         //        {
//         //            if (elementArray[j].id == events[i])
//         //            {
//         //                target += elementArray[j].target;
//         //            }
//         //        }
//         //    }
//
//         //    arrayTemp.Clear();
//         //    arrayTemp.Add(1004);
//         //}
//         //else
//         //{
//         //    FilterNeedRange(ref arrayTemp, 1001, 1003);
//         //    FilterNeedRange(ref arrayTemp, 1004, 1006);
//         //    target = 63;
//         //}
//         //eventArray = arrayTemp;
//         return target;
//     }
//
//     private void FilterNeedRange(ref NativeList<int> arrayTemp, int start, int end)
//     {
//         int temp = start;
//         for(int i=0;i<arrayTemp.Length;i++)
//         {
//             if (arrayTemp[i] >= start && arrayTemp[i] <= end)
//             {
//                 arrayTemp.RemoveAt(i);
//             }
//         }
//         arrayTemp.Add(start);
//
//     }
//
//     private bool JudgeContainOnly(NativeList<int> array, int start, int end)
//     {
//         for (int i = 0; i < array.Length; i++)
//         {
//             if (array[i] < start || array[i] > end)
//             {
//                 return false;
//             }
//         }
//
//         return true;
//     }
//
//
//     private void AddEventTo(int eventID, Entity entity, ref SystemState state, GlobalConfigData configData)
//     {
//         ref var eventsTable = ref configData.value.Value.configTbevent_0s.configTbevent_0s;
//         int triggerType = 0, type = 0;
//         for(int i=0;i<eventsTable.Length;i++)
//         {
//             if (eventsTable[i].id==eventID)
//             {
//                 triggerType = eventsTable[i].triggerType;
//                 type= eventsTable[i].type;
//             }
//         }
//         DynamicBuffer<GameEvent> GameEvent = default;
//         if (!state.EntityManager.HasBuffer<GameEvent>(entity))
//         {
//             GameEvent = state.EntityManager.AddBuffer<GameEvent>(entity);
//         }
//
//         switch (eventID)
//         {
//             case 1001:
//                 GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//                 GameEvent.Add(new GameEvent_1001
//                 {
//                     id = eventID,
//                     triggerType = triggerType,
//                     eventType = type,
//                     triggerGap = 0,
//                     remainTime = 1f,
//                     duration = 0,
//                     isPermanent = true,
//                     target = default,
//                     onceTime = 0,
//                     colliderScale = 0,
//                     delayTime = 1f
//                 }.ToGameEvent());
//                 break;
//             case 1002:
//                 GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//                 GameEvent.Add(new GameEvent_1002
//                 {
//                     id = eventID,
//                     triggerType = triggerType,
//                     eventType = type,
//                     triggerGap = 0,
//                     remainTime = 1f,
//                     duration = 0,
//                     isPermanent = true,
//                     target = default,
//                     onceTime = 0,
//                     colliderScale = 0,
//                     delayTime = 1f
//                 }.ToGameEvent());
//                 break;
//             case 1004:
//                 GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//                 GameEvent.Add(new GameEvent_1004
//                 {
//                     id = eventID,
//                     triggerType = triggerType,
//                     eventType = type,
//                     triggerGap = 0,
//                     remainTime = 1f,
//                     duration = 0,
//                     isPermanent = true,
//                     target = default,
//                     onceTime = 0,
//                     colliderScale = 0,
//                     delayTime = 1f
//                 }.ToGameEvent());
//                 break;
//         case 1005:
//             GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//             GameEvent.Add(new GameEvent_1005
//             {
//                 id = eventID,
//                 triggerType = triggerType,
//                 eventType = type,
//                 triggerGap = 0,
//                 remainTime = 1f,
//                 duration = 0,
//                 isPermanent = true,
//                 target = default,
//                 onceTime = 0,
//                 colliderScale = 0,
//                 delayTime = 1f
//             }.ToGameEvent());
//             break;
//         //case 1006:
//         //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//         //    GameEvent.Add(new GameEvent_1001
//         //    {
//
//         //        id = 1006,
//
//         //        isPermanent = true,
//         //        remainTime = 1,
//         //        target = default,
//
//         //    }.ToGameEvent());
//         //    break;
//             case 1007:
//                 GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//                 GameEvent.Add(new GameEvent_1007
//                 {
//                     id = eventID,
//                     eventType = type,
//                     triggerType = triggerType,
//                     isPermanent = true,
//                     duration = 0f,
//                     remainTime = 1f,
//                     target = default,
//                     triggerGap = 0f,
//                 }.ToGameEvent()) ;
//                 break;
//             case 1008:
//                 GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//                 GameEvent.Add(new GameEvent_1008
//                 {
//                     id = eventID,
//                     eventType = type,
//                     triggerType = triggerType,
//                     isPermanent = true,
//                     duration = 0f,
//                     remainTime = 1f,
//                     target = default,
//                     triggerGap = 2f,
//                     delayTime=0,
//                 }.ToGameEvent());
//                 break;
//             //case 1009:
//             //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//             //    GameEvent.Add(new GameEvent_1009
//             //    {
//             //        id = 1009,
//             //        isPermanent = false,
//             //        remainTime = 1,
//             //        target = default,
//             //    }.ToGameEvent());
//             //    break;
//             //case 1010:
//             //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//             //    GameEvent.Add(new GameEvent_1010
//             //    {
//             //        id = 1010,
//             //        isPermanent = true,
//             //        remainTime = 1,
//             //        target = default,
//             //    }.ToGameEvent());
//             //    break;
//             //case 2002:
//             //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//             //    GameEvent.Add(new GameEvent_2002
//             //    {
//
//             //        id = 2002,
//             //        isPermanent = true,
//             //        remainTime = 1,
//             //        target = default,
//             //    }.ToGameEvent());
//             //    break;
//             //case 2003:
//             //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//             //    GameEvent.Add(new GameEvent_2003
//             //    {
//
//             //        id = 2003,
//
//             //        isPermanent = true,
//             //        remainTime = 1,
//             //        target = default,
//
//             //    }.ToGameEvent());
//             //    break;
//             //case 2004:
//             //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//             //    GameEvent.Add(new GameEvent_2004
//             //    {
//
//             //        id = 2004,
//             //        isPermanent = true,
//             //        remainTime = 1,
//             //        target = default,
//             //    }.ToGameEvent());
//             //    break;
//             case 2005:
//                 GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
//                 GameEvent.Add(new GameEvent_2005
//                 {
//                     id = 2005,
//                     triggerType = triggerType,
//                     eventType = type,
//                     triggerGap = 0,
//                     remainTime = 0,
//                     duration = 0,
//                     isPermanent = true,
//                     target = default,
//                     onceTime = 0,
//                     colliderScale = 0,
//                     delayTime = 0
//                 }.ToGameEvent());
//                 break;
//         }
//     }
//
//
//     /// <summary>
//     /// 拿到本次刷新所需要的entity 并初始化数据
//     /// </summary>
//     /// <param name="areaQuery"></param>
//     /// <param name="obstacleQuery"></param>
//     /// <param name="elements">刷新组</param>
//     /// <param name="outEntities"></param>
//     /// <param name="state"></param>
//     public void GetEntity(EntityQuery elementQuery, ref BlobArray<int2> elements,
//         out NativeList<Entity> outEntities, ref SystemState state)
//     {
//         outEntities = new NativeList<Entity>(StaticEnumDefine.maxLengthRefreshGroup, Allocator.Temp);
//         var elementEntities = elementQuery.ToEntityArray(Allocator.Temp);
//         if (elementEntities.Length == 0)
//         {
//             Debug.Log($"没有获取到实体");
//             return;
//         }
//
//         outEntities.Clear();
//         for (int i = 0; i < elements.Length; i++)
//         {
//             int id = elements[i].x;
//             foreach (var item in elementEntities)
//             {
//                 var element = SystemAPI.GetComponent<MapElementData>(item);
//                 if (element.elementID == id)
//                 {
//                  
//                     outEntities.Add(item);
//                     break;
//
//                 }
//             }
//             //else
//             //{
//             //    Debug.Log($"刷新组id有误 请核查");
//             //}
//         }
//     }
//
//     /// <summary>
//     /// 获取刷新位置
//     /// </summary>
//     /// <param name="itemRects">刷新区域存储</param>
//     /// <param name="configTbrefresh">刷新表</param>
//     /// <param name="refreshType">刷新类型</param>
//     /// <param name="state"></param>
//     /// <returns></returns>
//     public NativeList<float2> RefreshItem(ref NativeList<Rect> itemRects, ref ConfigTbmap_refresh configTbrefresh,
//         int refreshType, ref SystemState state)
//     {
//         EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
//         //是否可以重复刷新
//         bool exclusive = configTbrefresh.selfYn == 1 ? false : true;
//         var buffer = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>();
//
//
//         float3 startMax = new float3(0, -9999, 0);
//         for (int i = 0; i < buffer.Length; i++)
//         {
//             if (buffer[i].mapIndex == currentMapIndex)
//             {
//                //Debug.Log($"start11111:{buffer[i].mapPosStruct.startPos.y}");
//                 if (startMax.y < buffer[i].mapPosStruct.startPos.y)
//                     startMax = buffer[i].mapPosStruct.startPos;
//             }
//         }
//
//          Debug.Log($"startMax:{startMax}");
//         // var gridSize = buffer[buffer.Length - 1].mapPosStruct.gridSize;
//
//         float2 startPos = new float2(configTbrefresh.coordinate[0].x, configTbrefresh.coordinate[0].y);
//         float2 endPos = new float2(configTbrefresh.coordinate[1].x, configTbrefresh.coordinate[1].y);
//
//         //坐标转换为实际坐标
//         float2 startPosT = new float2(startMax.x + startPos.x, startMax.y - startPos.y);
//         float2 endPosT = new float2(startMax.x + endPos.x, startMax.y - endPos.y);
//         Debug.Log($"startPosT:{startPosT},endPosT:{endPosT}");
//         //是否随机刷新
//         bool isRandGenerate = configTbrefresh.randType == 1 ? true : false;
//
//
//         int count = configTbrefresh.num;
//         NativeList<float2> Positionlist = new NativeList<float2>(count, Allocator.Temp);
//         int gap = configTbrefresh.pointRange;
//         if (isRandGenerate)
//         {
//             var wbe = SystemAPI.GetSingletonEntity<GameRandomData>();
//             var gameRandomData = SystemAPI.GetSingletonRW<GameRandomData>().ValueRW;
//             var random = gameRandomData.rand;
//             //random. = new Random(StaticEnumDefine.mapRandomSeed);
//             switch (refreshType)
//             {
//                 case 1:
//                 case 2:
//                     for (int i = 0; i < count; ++i)
//                     {
//                         Rect itemRect;
//                         int times = 0;
//                         do
//                         {
//                             times++;
//                             var targetPos = new float2(random.NextFloat(startPosT.x, endPosT.x),
//                                 random.NextFloat(startPosT.y, endPosT.y));
//
//                             itemRect = new Rect(targetPos.x-gap, targetPos.y+gap, configTbrefresh.refreshElementSet[0].x + gap*2,
//                                 configTbrefresh.refreshElementSet[0].y + gap*2);
//
//                             if (IsMaxLimitLoop(times))
//                             {
//                                 Debug.Log("生成规则有误 选点失败");
//                                 Debug.Log(
//                                     $"目前选点个数为{Positionlist.Length},剩余的未选点个数为{configTbrefresh.num - Positionlist.Length},目前刷新组id为{configTbrefresh.id},目前的刷新组规则id为{configTbrefresh.ruleId}");
//
//                                 break;
//                             }
//                         } while (ItemOverlap(ref state, itemRect, itemRects));
//
//                         if (times >= 50)
//                         {
//                             break;
//                         }
//
//                         itemRects.Add(itemRect);
//                         Positionlist.Add(new float2(itemRect.x+gap, itemRect.y-gap));
//                     }
//
//                     break;
//                 case 3:
//                     ref var refreshArrayElement = ref configTbrefresh.refreshElementSet;
//                     float2 maxTemp = GetMaxWidthHeightFromRefreshGroup(ref configTbrefresh.refreshElementSet);
//                     float width;
//                     float height;
//                     for (int i = 0; i < count; ++i)
//                     {
//                         Rect itemRect;
//                         int times = 0;
//                         width = configTbrefresh.areaSize[0] * maxTemp.x;
//                         height = configTbrefresh.areaSize[1] * maxTemp.y;
//                         do
//                         {
//                             var targetPos = random.NextFloat2(startPosT, endPosT);
//                             itemRect = new Rect(targetPos.x, targetPos.y, width + gap, height + gap);
//                             times++;
//                             if (IsMaxLimitLoop(times))
//                             {
//                                 Debug.Log("生成规则有误 选点失败");
//                                 Debug.Log(
//                                     $"目前选点个数为{Positionlist.Length},剩余的未选点个数为{configTbrefresh.num - Positionlist.Length},目前刷新组id为{configTbrefresh.id},目前的刷新组规则id为{configTbrefresh.ruleId}");
//                                 break;
//                             }
//                         } while (ItemOverlap(ref state, itemRect, itemRects));
//
//                         itemRects.Add(itemRect);
//                         Positionlist.Add(new float2(itemRect.x, itemRect.y));
//                     }
//
//                     break;
//             }
//
//             //TODO:没加 看后续能不能随机成功
//             gameRandomData.rand = random;
//             ecb.SetComponent(wbe, gameRandomData);
//             ecb.Playback(state.EntityManager);
//         }
//         else
//         {
//             int width = configTbrefresh.refreshElementSet[0].x;
//             int height = configTbrefresh.refreshElementSet[0].y;
//             Positionlist = SequentialGenerate(ref itemRects, width, height, startPosT, endPosT, count, gap);
//         }
//
//         //允许重复刷新的话 清空本次rects记录
//         if (!exclusive)
//         {
//             itemRects.Clear();
//         }
//
//         return Positionlist;
//     }
//
//
//     float2 GetMaxWidthHeightFromRefreshGroup(ref BlobArray<int2> refresh)
//     {
//         int xMax = refresh[0].x;
//         int yMax = refresh[0].y;
//         for (int i = 1; i < refresh.Length; i++)
//         {
//             if (refresh[i].x > xMax)
//                 xMax = refresh[i].x;
//             if (refresh[i].y > yMax)
//                 yMax = refresh[i].y;
//         }
//
//         return new float2(xMax, yMax);
//     }
//
//     public bool ItemOverlap(ref SystemState state, Rect itemRect, NativeList<Rect> itemRects)
//     {
//         //位置不能与玩家位置重合
//         var player = SystemAPI.GetSingletonEntity<PlayerData>();
//         var loc = SystemAPI.GetComponentRO<LocalTransform>(player);
//         var playerRect = new Rect(0, 0, loc.ValueRO.Scale * 20, loc.ValueRO.Scale * 20);
//         // 检查物品是否与已经生成的物品重叠
//         foreach (Rect rect in itemRects)
//         {
//             if (itemRect.Overlaps(rect) || playerRect.Overlaps(itemRect))
//             {
//                 return true;
//             }
//         }
//
//         return false;
//     }
//
//
//     public NativeList<float2> SequentialGenerate(ref NativeList<Rect> itemRects, int width, int height, float2 startPos,
//         float2 endPos, int num, int gap)
//     {
//         NativeList<float2> result = new NativeList<float2>(num, Allocator.Temp);
//         float2 targetPos = new float2(startPos.x, startPos.y);
//         int i = 0;
//
//         while (i < num)
//         {
//             if (result.Length > 0)
//             {
//                 float2 lastPos = result[result.Length - 1];
//                 if (lastPos.x + 2 * width + gap > endPos.x)
//                 {
//                     targetPos = new float2(startPos.x, lastPos.y + gap + height);
//                 }
//                 else
//                 {
//                     targetPos = new float2(lastPos.x + gap + width, lastPos.y);
//                 }
//             }
//
//             Rect rect = new Rect(targetPos.x, targetPos.y, width + gap, height + gap);
//             itemRects.Add(rect);
//             result.Add(targetPos);
//             ++i;
//         }
//
//         return result;
//     }
//
//
//     public void InitEntities(ref SystemState state, NativeList<float2> positions, NativeList<Entity> entities, int type,
//         ref ConfigTbmap_refresh configTbrefresh, ref NativeHashMap<Entity, float3> posDic)
//     {
//         Debug.Log($"positions:{positions.Length},entities:{entities.Length}");
//         if (entities.Length <= 0) return;
//         if (positions.Length <= 0) return;
//
//         bool isReRefresh = configTbrefresh.againYn == 1 ? true : false;
//         int num = configTbrefresh.num;
//         if (positions.Length <= num)
//         {
//             num = positions.Length;
//         }
//
//         ref var relativePos = ref configTbrefresh.refreshElementSet;
//
//         var resultEntities = CollectionHelper.CreateNativeArray<Entity>(num, Allocator.Temp);
//         switch (type)
//         {
//             case 1:
//             case 2:
//                 var current = entities[0];
//                 state.EntityManager.Instantiate(current, resultEntities);
//                 AjustEntityScale(ref state, ref resultEntities);
//                 for (int i = 0; i < num; ++i)
//                 {
//                     if (!state.EntityManager.HasComponent<MapElementData>(resultEntities[i]))
//                     {
//                         AddDataToElement(resultEntities[i], ref state);
//                     }
//                     float3 position = new float3(positions[i].x, positions[i].y, 0);
//                     var transform = SystemAPI.GetComponentRW<LocalTransform>(resultEntities[i]);
//                     transform.ValueRW.Position = position;
//                     //Debug.Log($"transform.ValueRW.Position:{transform.ValueRW.Position}");
//                     InitMask(ref state, transform.ValueRW, current);
//                     posDic.Add(resultEntities[i], position);
//                     //if (isReRefresh)
//                     //{
//                     //    state.EntityManager.AddComponentData(resultEntities[i], new RefreshTag { });
//                     //}
//                 }
//
//                 break;
//             case 3:
//                 for (int i = 0; i < relativePos.Length; ++i)
//                 {
//                     var currentEntity = entities[i];
//                     var result = CollectionHelper.CreateNativeArray<Entity>(num, Allocator.Temp);
//                     state.EntityManager.Instantiate(currentEntity, result);
//                     AjustEntityScale(ref state, ref result);
//                     var relPos = relativePos[i].y;
//                     for (int j = 0; j < num; ++j)
//                     {
//                         if (!state.EntityManager.HasComponent<MapElementData>(result[j]))
//                         {
//                             AddDataToElement(result[j], ref state);
//                         }
//                         float3 position = new float3(positions[j].x + relPos * configTbrefresh.refreshElementSet[i].x,
//                             positions[j].y + configTbrefresh.refreshElementSet[i].y * relPos, 0);
//                         var transform = SystemAPI.GetComponentRW<LocalTransform>(result[j]);
//                         transform.ValueRW.Position = position;
//                         Debug.Log($"transform.ValueRW.Position:{transform.ValueRW.Position}");
//                         InitMask(ref state, transform.ValueRW, currentEntity);
//                         posDic.Add(result[j], position);
//                         //if (isReRefresh)
//                         //{
//                         //    state.EntityManager.AddComponentData(result[j], new RefreshTag { });
//                         //}
//                     }
//                 }
//
//                 break;
//         }
//     }
//
//     private void InitMask(ref SystemState state, LocalTransform transform, Entity current)
//     {
//         //if (!SystemAPI.TryGetSingleton<GlobalConfigData>(out var configData))
//         //{
//         //    return;
//         //}
//         //ref var refreshArray = ref configData.value.Value.configTbmap_modules.configTbmap_modules;
//         //var sbe = SystemAPI.GetSingletonEntity<PrefabMapData>();
//         //var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(sbe);
//         //var id = state.EntityManager.GetComponentData<MapElementData>(current).elementID;
//         //Entity mask = default;
//         //int type = 0;
//         //bool isMonsterRefresh = false;
//         //for (int i = 0; i < refreshArray.Length; ++i)
//         //{
//         //    if (refreshArray[i].id == id)
//         //    {
//         //        type = refreshArray[i].type;
//         //        isMonsterRefresh = refreshArray[i].refreshMonsterYn == 1 ? true : false;
//         //    }
//         //}
//         //if (type == 1)
//         //{
//         //    mask = state.EntityManager.Instantiate(prefabMapData.prefabHashMap["mask_green"]);
//         //}
//         //else if (type == 2)
//         //{
//         //    mask = state.EntityManager.Instantiate(prefabMapData.prefabHashMap["mask_red"]);
//         //}
//         //if (mask != default)
//         //{
//         //    var loc = SystemAPI.GetComponentRW<LocalTransform>(mask);
//         //    loc.ValueRW = transform;
//         //}
//     }
//
//     public bool IsMaxLimitLoop(int times)
//     {
//         return times > StaticEnumDefine.maxSelectPosTimes ? true : false;
//     }
// }

