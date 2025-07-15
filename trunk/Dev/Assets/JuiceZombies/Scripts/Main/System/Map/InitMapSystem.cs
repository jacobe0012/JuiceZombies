// using cfg.blobstruct;
// using Main;
// using NSprites;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Entities.UniversalDelegates;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
// using Random = Unity.Mathematics.Random;
//
// /// <summary>
// ///初始化地图
// /// </summary>
// //[DisableAutoCreation]
// //[BurstCompile]
// //[RequireMatchingQueriesForUpdate]
// [UpdateInGroup(typeof(InitializationSystemGroup))]
// public partial struct InitMapSystem : ISystem, ISystemStartStop
// {
//     [BurstCompile]
//     public void OnCreate(ref SystemState state)
//     {
//         state.Enabled = false;
//         state.RequireForUpdate<WorldBlackBoardTag>();
//         //state.RequireForUpdate<MapIndexPosBuffer>();
//     }
//
//
//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         if (!SystemAPI.TryGetSingletonRW<GameOthersData>(out RefRW<GameOthersData> mapGlobalData)) return;
//
//
//         int mapID = mapGlobalData.ValueRO.mapData.mapID;
//         int mapheight = mapGlobalData.ValueRO.mapData.mapSize.y;
//         int mapWidth = mapGlobalData.ValueRO.mapData.mapSize.x;
//         int mapType = mapGlobalData.ValueRO.mapData.mapType;
//
//
//         GetNeedPrefab(ref state, mapID, out Entity mapPrefab, out NativeList<Entity> borderPrefabLeft,
//             out NativeList<Entity> borderPrefabRight, out NativeList<Entity> borderPrefabTop,
//             out NativeList<Entity> borderPrefabBottom);
//
//
//         DynamicBuffer<MapIndexPosBuffer> mapPos = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>();
//         mapPos.Clear();
//         //���õ�ͼƴ�ӷ�ʽ
//         int needCount = 0;
//         if (mapType == 1)
//         {
//             var borderLeft = GetRandomEntity(borderPrefabLeft, ref state);
//             var borderRight = GetRandomEntity(borderPrefabRight, ref state);
//             var borderTop = GetRandomEntity(borderPrefabTop, ref state);
//             var borderBottom = GetRandomEntity(borderPrefabBottom, ref state);
//             AjustMap(ref state, mapPrefab, borderRight, borderLeft, borderBottom, borderTop, mapWidth, mapType);
//             var newMap = state.EntityManager.Instantiate(mapPrefab);
//             borderLeft = state.EntityManager.Instantiate(borderLeft);
//             borderRight = state.EntityManager.Instantiate(borderRight);
//             borderBottom = state.EntityManager.Instantiate(borderBottom);
//             borderTop = state.EntityManager.Instantiate(borderTop);
//
//             var trans = SystemAPI.GetComponentRW<LocalTransform>(borderLeft);
//             float xPos = -mapWidth / 2f - trans.ValueRW.Scale * 5.12f / 2;
//             //float xPos = -300;
//             trans.ValueRW.Position = new float3(xPos, 0, 0);
//             trans = SystemAPI.GetComponentRW<LocalTransform>(borderRight);
//             trans.ValueRW.Position = new float3(-xPos, 0, 0);
//
//             trans = SystemAPI.GetComponentRW<LocalTransform>(borderTop);
//             float yPos = mapheight / 2f + trans.ValueRW.Scale * 5.12f / 2;
//             //float yPos = 270f;
//             trans.ValueRW.Position = new float3(0, yPos, 0);
//             trans = SystemAPI.GetComponentRW<LocalTransform>(borderBottom);
//             trans.ValueRW.Position = new float3(0, -yPos, 0);
//
//
//             //var flip= SystemAPI.GetComponentRW<Flip>(borderLeft);
//             //flip.ValueRW.Value.x = -1;
//             state.EntityManager.AddComponentData(newMap, new MapBaseData { mapIndex = 1 });
//             MapPosStruct mapPosStruct = new MapPosStruct();
//             var startPos = new float3(-mapWidth / 2f, mapheight / 2f, 0);
//             var endPos = new float3(-mapWidth / 2f, -mapheight / 2f, 0);
//             mapPosStruct.startPos = startPos;
//             mapPosStruct.pathPos = endPos;
//             mapPos = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>();
//
//             if (mapPos.IsCreated)
//             {
//                 mapPos.Clear();
//                 mapPos.Add(new MapIndexPosBuffer
//                     { mapPosStruct = mapPosStruct, mapIndex = 1 });
//             }
//
//             state.EntityManager.AddComponent<ObstacleTag>(borderRight);
//             state.EntityManager.AddComponent<ObstacleTag>(borderLeft);
//             state.EntityManager.AddComponent<ObstacleTag>(borderTop);
//             state.EntityManager.AddComponent<ObstacleTag>(borderBottom);
//         }
//         //else if (mapType == 2)
//         //{
//         //    for (int i = 0; i < 3; i++)
//         //    {
//         //        for (int j = 0; j < mapheight / mapWidth; j++)
//         //        {
//
//         //        }
//         //    }
//
//         //    needCount = mapheight / mapWidth;
//         //    float3 path = new float3(-mapWidth / 2f, -mapheight / 2f, 0);
//         //    for (int i = 0; i < needCount; i++)
//         //    {
//         //        var newMap = state.EntityManager.Instantiate(mapPrefab);
//         //        var trans = SystemAPI.GetComponentRW<LocalTransform>(newMap);
//         //        trans.ValueRW.Scale = mapWidth / 10.24f;
//         //        state.EntityManager.AddComponentData(newMap, new MapBaseData { mapIndex = 1 });
//
//         //        MapPosStruct mapPosStruct = new MapPosStruct();
//         //        var endTempPos = new float3(path.x, path.y + i * mapWidth, 0);
//         //        var startTempPos = new float3(endTempPos.x, endTempPos.y + mapWidth, 0);
//         //        mapPosStruct.startPos = startTempPos;
//         //        mapPosStruct.pathPos = endTempPos;
//
//         //        var currentLoc = new LocalTransform
//         //        {
//         //            Position = new float3(0, (startTempPos.y + endTempPos.y) / 2f, 0),
//         //            Scale = SystemAPI.GetComponentRO<LocalTransform>(newMap).ValueRO.Scale
//         //        };
//         //        InstanceBorder(ref state, currentLoc, GetRandomEntity(borderPrefabLeft, ref state),
//         //            GetRandomEntity(borderPrefabRight, ref state), mapWidth);
//
//         //        var mapTrans = SystemAPI.GetComponentRW<LocalTransform>(newMap);
//         //        mapTrans.ValueRW = currentLoc;
//
//         //        //��ʼ����������
//         //        mapPos = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>();
//         //        if (mapPos.IsCreated)
//         //        {
//         //            mapPos.Clear();
//         //            mapPos.Add(new MapIndexPosBuffer
//         //            { mapPosStruct = mapPosStruct, mapIndex = 1 });
//         //        }
//
//         //        state.EntityManager.AddComponentData(newMap, new RefreshTag { refreshIndex = i + 1 });
//         //    }
//
//         //}
//         //else if (mapType == 3)
//         //{
//         //    var perWidth = mapWidth / 3f;
//         //    var perHeight = mapheight / 3f;
//         //    var startPos = new float3(-perWidth, perHeight, 0);
//         //    MapPosStruct mapPosStruct = new MapPosStruct();
//         //    var startTempPos = new float3(startPos.x - perWidth / 2f, startPos.y + perHeight / 2f, 0);
//         //    var endTempPos = new float3(startTempPos.x, startTempPos.y + mapheight, 0);
//         //    mapPosStruct.startPos = startTempPos;
//         //    mapPosStruct.pathPos = endTempPos;
//         //    for (int i = 0; i < 3; i++)
//         //    {
//         //        for (int j = 0; j < 3; j++)
//         //        {
//         //            var currentPos = new float3(startPos.x + i * perWidth, startPos.y - j * perHeight, 0);
//         //            var newMap = state.EntityManager.Instantiate(mapPrefab);
//         //            var trans = SystemAPI.GetComponentRW<LocalTransform>(newMap);
//         //            trans.ValueRW.Scale = perHeight / 10.24f;
//         //            trans.ValueRW.Position = currentPos;
//         //            state.EntityManager.AddComponentData(newMap, new MapBaseData { mapIndex = 1 });
//         //            state.EntityManager.AddComponentData(newMap, new RefreshTag { refreshIndex = i * 3 + j + 1 });
//         //        }
//         //    }
//
//         //    mapPos = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>();
//         //    Debug.LogError($"mapPos{mapPos.Length}");
//         //    if (mapPos.IsCreated)
//         //    {
//         //        Debug.LogError($"mapPos 222");
//         //        mapPos.Clear();
//         //        mapPos.Add(new MapIndexPosBuffer
//         //        { mapPosStruct = mapPosStruct, mapIndex = 1 });
//         //    }
//         //}
//
//
//         float limitTop = 0;
//         float limitBottom = 0;
//
//
//         //mapPos = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>();
//         //for (int i = 0; i < mapPos.Length; i++)
//         //{
//         //    if (mapPos[i].mapPosStruct.startPos.y > limitTop)
//         //    {
//         //        limitTop = mapPos[i].mapPosStruct.startPos.y;
//         //    }
//
//         //    if (mapPos[i].mapPosStruct.pathPos.y < limitBottom)
//         //    {
//         //        limitBottom = mapPos[i].mapPosStruct.pathPos.y;
//         //        //Debug.Log($"limitBottom{limitBottom}");
//         //    }
//         //}
//         //if (!SystemAPI.TryGetSingletonRW<MapGloablaData>(out mapGlobalData)) return;
//         //mapGlobalData.ValueRW.minAndMaxPosY = new float2(limitBottom, limitTop);
//         //Debug.Log($"map isInit,limitBottom{limitBottom},limitTop{limitTop}");
//
//         //if (isInit && !SystemAPI.GetEntityStorageInfoLookup().Exists(mapEntity))
//         //{
//         //    //Debug.Log($"{mapEntity.ToString()}");
//         //    isInit = false;
//         //}
//
//
//         //EnableGenerateItem();
//
//
//         //var systemHandle = state.WorldUnmanaged.GetExistingUnmanagedSystem<AreaAnd0bstacleGenerateSystem>();
//         //ref var areaState = ref state.WorldUnmanaged.ResolveSystemStateRef(systemHandle);
//         //areaState.Enabled = true;
//         //state.Enabled = false;
//     }
//
//     private Entity GetRandomEntity(NativeList<Entity> entities, ref SystemState state)
//     {
//         if (!SystemAPI.TryGetSingletonEntity<GameRandomData>(out Entity wbe)) return default;
//         EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
//         var gameRandomData = SystemAPI.GetSingletonRW<GameRandomData>().ValueRW;
//         var random = gameRandomData.rand;
//         int randomIndex = random.NextInt(0, entities.Length);
//         gameRandomData.rand = random;
//         ecb.SetComponent(wbe, gameRandomData);
//         ecb.Playback(state.EntityManager);
//         return entities[randomIndex];
//     }
//
//
//     /// <summary>
//     /// �����õ�����Ԥ����
//     /// </summary>
//     /// <param name="state"></param>
//     /// <param name="mapID"></param>
//     /// <param name="mapPrefab">��ͼ</param>
//     /// <param name="borderPrefabLeft">���Ե</param>
//     /// <param name="borderPrefabRight">�ұ�Ե</param>
//     /// <param name="borderPrefabTop">�ϱ�Ե</param>
//     /// <param name="borderPrefabBottom">�±�Ե</param>
//     private void GetNeedPrefab(ref SystemState state, int mapID, out Entity mapPrefab,
//         out NativeList<Entity> borderPrefabLeft, out NativeList<Entity> borderPrefabRight,
//         out NativeList<Entity> borderPrefabTop, out NativeList<Entity> borderPrefabBottom)
//     {
//         mapPrefab = default;
//         borderPrefabBottom = new NativeList<Entity>(Allocator.Temp);
//         borderPrefabTop = new NativeList<Entity>(Allocator.Temp);
//         borderPrefabLeft = new NativeList<Entity>(Allocator.Temp);
//         borderPrefabRight = new NativeList<Entity>(Allocator.Temp);
//         if (!SystemAPI.TryGetSingleton<GlobalConfigData>(out GlobalConfigData globalConfigData))
//         {
//             Debug.Log($"û����GlobalConfigData");
//             return;
//         }
//
//         ;
//         if (!SystemAPI.TryGetSingleton<PrefabMapData>(out PrefabMapData prefabMapData))
//         {
//             Debug.Log($"û����prefabMapData");
//             return;
//         }
//
//         ;
//         ref var mapArray = ref globalConfigData.value.Value.configTbmap_types.configTbmap_types;
//         for (int i = 0; i < mapArray.Length; i++)
//         {
//             if (mapArray[i].id == mapID)
//             {
//                 mapPrefab = prefabMapData.prefabHashMap[mapArray[i].bg];
//                 for (int j = 0; j < mapArray[i].bgLeft.Length; j++)
//                 {
//                     borderPrefabLeft.Add(prefabMapData.prefabHashMap[mapArray[i].bgLeft[j]]);
//                 }
//
//                 for (int j = 0; j < mapArray[i].bgRight.Length; j++)
//                 {
//                     borderPrefabRight.Add(prefabMapData.prefabHashMap[mapArray[i].bgRight[j]]);
//                 }
//
//                 for (int j = 0; j < mapArray[i].bgBottom.Length; j++)
//                 {
//                     borderPrefabBottom.Add(prefabMapData.prefabHashMap[mapArray[i].bgBottom[j]]);
//                 }
//
//                 for (int j = 0; j < mapArray[i].bgTop.Length; j++)
//                 {
//                     borderPrefabTop.Add(prefabMapData.prefabHashMap[mapArray[i].bgTop[j]]);
//                 }
//
//                 break;
//             }
//         }
//     }
//
//     private void InstanceBorder(ref SystemState state, LocalTransform currentLoc, Entity borderLeft,
//         Entity borderRight, int mapWidth)
//     {
//         borderLeft = state.EntityManager.Instantiate(borderLeft);
//         borderRight = state.EntityManager.Instantiate(borderRight);
//         AddDataToBorder(borderLeft, ref state, 1);
//         AddDataToBorder(borderRight, ref state, 1);
//         var locLeft = SystemAPI.GetComponentRW<LocalTransform>(borderLeft);
//         var locRight = SystemAPI.GetComponentRW<LocalTransform>(borderRight);
//         locLeft.ValueRW.Scale = mapWidth / 10.24f;
//         locRight.ValueRW.Scale = mapWidth / 10.24f;
//         float xPos = mapWidth / 2f + locLeft.ValueRO.Scale * 5.12f / 2;
//         locLeft.ValueRW.Position = new float3(currentLoc.Position.x - xPos, currentLoc.Position.y, 0);
//         locRight.ValueRW.Position = new float3(currentLoc.Position.x + xPos, currentLoc.Position.y, 0);
//     }
//
//
//     public void AddDataToBorder(Entity entity, ref SystemState state, int mapIndex)
//     {
//         state.EntityManager.AddComponent<ObstacleTag>(entity);
//
//
//         var elementData = new MapElementData { };
//
//
//         ////��ʼ��������Ϣ
//         elementData.belongsIndex = mapIndex;
//         elementData.elementID = 2001;
//         //��area��ӻ�����Ϣ
//         state.EntityManager.AddComponentData(entity, elementData);
//
//         if (state.EntityManager.HasComponent<MapElementData>(entity))
//         {
//             state.EntityManager.RemoveComponent<MapElementData>(entity);
//         }
//     }
//
//     private void AjustMap(ref SystemState state, Entity mapPrefab, Entity right, Entity left, Entity bottom, Entity top,
//         int mapWidth, int mapType)
//     {
//         var trans = SystemAPI.GetComponentRW<LocalTransform>(mapPrefab);
//         var transBorderLeft = SystemAPI.GetComponentRW<LocalTransform>(left);
//         var transBorderRight = SystemAPI.GetComponentRW<LocalTransform>(right);
//         //���
//         if (mapType == 1)
//         {
//             trans.ValueRW.Scale = mapWidth / 20.48f;
//             transBorderLeft.ValueRW.Scale = mapWidth / 20.48f;
//             transBorderRight.ValueRW.Scale = mapWidth / 20.48f;
//
//             var transTop = SystemAPI.GetComponentRW<LocalTransform>(top);
//             var transBottom = SystemAPI.GetComponentRW<LocalTransform>(bottom);
//             transTop.ValueRW.Scale = mapWidth / 20.48f;
//             transBottom.ValueRW.Scale = mapWidth / 20.48f;
//         }
//         //����
//         else if (mapType == 2)
//         {
//             trans.ValueRW.Scale = mapWidth / 10.24f;
//             transBorderLeft.ValueRW.Scale = mapWidth / 10.24f;
//             transBorderRight.ValueRW.Scale = mapWidth / 10.24f;
//         }
//     }
//
//     private void EnableGenerateItem()
//     {
//         //var initSysGroup =
//         //    World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<InitializationSystemGroup>();
//
//
//         ////����item
//         //var areaAnd0bstacleGenerateSystem =
//         //    World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<AreaAnd0bstacleGenerateSystem>();
//         //initSysGroup.AddSystemToUpdateList(areaAnd0bstacleGenerateSystem);
//         //var state = World.DefaultGameObjectInjectionWorld.EntityManager.WorldUnmanaged
//         //    .GetExistingSystemState<AreaAnd0bstacleGenerateSystem>();
//     }
//
//     [BurstCompile]
//     public void OnDestroy(ref SystemState state)
//     {
//     }
//
//     [BurstCompile]
//     public void OnStartRunning(ref SystemState state)
//     {
//     }
//
//     [BurstCompile]
//     public void OnStopRunning(ref SystemState state)
//     {
//         //  Debug.Log("OnStopRunning SystemState");
//     }
// }

