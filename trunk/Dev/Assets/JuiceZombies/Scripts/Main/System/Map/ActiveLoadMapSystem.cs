//using Main;
//using UnityEngine;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using System;
//using NSprites;
//using static UnityEditor.PlayerSettings;

///// <summary>
///// 当地图类型为上线无限时 动态加载
///// </summary>
////[RequireMatchingQueriesForUpdate]
//[UpdateAfter(typeof(InitMapSystem))]
//[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//public partial struct ActiveLoadMapSystem : ISystem
//{
//    Entity player;
//    float mapX;

//    [BurstCompile]
//    public void OnCreate(ref SystemState state)
//    {
//        state.RequireForUpdate<WorldBlackBoardTag>();
//        state.RequireForUpdate<MapIndexPosBuffer>();
//        //isInit = false;
//        player = default;
//    }

//    [BurstCompile]
//    public void OnDestroy(ref SystemState state)
//    {
//    }

//    [BurstCompile]
//    public void OnUpdate(ref SystemState state)
//    {

//        var mapPos = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>();
//        if (mapPos.Length <= 0) return;

//        //拿到目前最大的mapindex
//        int currentMapIndex = mapPos[mapPos.Length - 1].mapIndex;
//        mapX = mapPos[mapPos.Length - 1].mapPosStruct.startPos.x;
//        if (currentMapIndex < 1) return;
//        if (currentMapIndex > 3)
//        {
//            //销毁第一张地图
//        }


//        if (!SystemAPI.TryGetSingleton<GlobalConfigData>(out var configData))
//        {
//            return;
//        }

//        var mapGlobal = SystemAPI.GetSingleton<GameOthersData>().mapData;
//        var type = mapGlobal.mapType;
//        if (type == 1) return;
//        int mapHeight = mapGlobal.mapSize[1];
//        int mapWidth = mapGlobal.mapSize[0];
//        int mapID = mapGlobal.mapID;
//        if (type == 2)
//        {
//            float limitTop = mapGlobal.minAndMaxPosY.y;
//            float limitBottom = mapGlobal.minAndMaxPosY.x;
//            //var queryBuilder = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerData>();
//            //var playerArray = state.GetEntityQuery(queryBuilder).ToEntityArray(Allocator.Temp);

//            //if (playerArray.Length == 0) return;
//            //player = playerArray[0];
//            var player = SystemAPI.GetSingletonEntity<PlayerData>();
//            var playerTransform = SystemAPI.GetComponentRO<LocalTransform>(player);
//            float3 currentPosition = playerTransform.ValueRO.Position;

//            float currentTop = currentPosition.y + StaticEnumDefine.maxCameraSize;
//            //Debug.Log($"currentTop:{currentTop}");
//            float currentBottom = currentPosition.y - StaticEnumDefine.maxCameraSize;
//            //Debug.Log($"currentBottom:{currentBottom}");
//            if (currentTop >= limitTop)
//            {
//                GenerateNewMapTop(ref state, mapHeight, mapWidth, currentMapIndex, limitTop, mapID);
//                EnableGenerateItem(ref state);
//            }
//            else if (currentBottom <= limitBottom)
//            {
//                GenerateNewMapBottom(ref state, mapHeight, mapWidth, currentMapIndex, limitBottom, mapID);
//                EnableGenerateItem(ref state);
//            }
//        }
//        else
//        {
//            //int width = mapTable[currentMapID - 1].size[0];
//            //var player = SystemAPI.GetSingletonEntity<PlayerData>();
//            //var playerLoc=SystemAPI.GetComponentRO<LocalTransform>(player);
//            //foreach (var (loctrans, refreshPos) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<RefreshTag>>())
//            //{
//            //    var rect = new Rect(loctrans.ValueRO.Position.x, loctrans.ValueRO.Position.y, width/3f, width/3f);
//            //    if (rect.Contains(playerLoc.ValueRO.Position)){
//            //        int index = refreshPos.ValueRO.refreshIndex;
//            //        Debug.Log($"位于九宫格的第{index}个格子内");
//            //        switch (index)
//            //        {
//            //            case 1:
//            //                break;
//            //            case 2:
//            //                break;
//            //            case 3:
//            //                break;
//            //            case 4:
//            //                break;
//            //            case 5:
//            //                break;
//            //            case 6:
//            //                break;
//            //            case 7:
//            //                break;
//            //            case 8:
//            //                break;
//            //            case 9:
//            //                break;

//            //        }
//            //    }

//            //}

//            //全开放地图的生成逻辑
//        }
//    }

//    private void EnableGenerateItem(ref SystemState state)
//    {
//        Debug.Log("EnableGenerateItem");
//        //生成item


//        var systemHandle = state.WorldUnmanaged.GetExistingUnmanagedSystem<AreaAnd0bstacleGenerateSystem>();
//        ref var stateRef = ref state.WorldUnmanaged.ResolveSystemStateRef(systemHandle);
//        stateRef.Enabled = true;
//        // ref var state = ref  
//        // state.Enabled = true;
//    }

//    private void AjustMap(ref SystemState state, Entity mapPrefab, Entity borderPrefabRight, Entity borderPrefabLeft,
//        int mapheight, int mapWidth)
//    {
//        var trans = SystemAPI.GetComponentRW<LocalTransform>(mapPrefab);
//        var transBorderLeft = SystemAPI.GetComponentRW<LocalTransform>(borderPrefabLeft);
//        var transBorderRight = SystemAPI.GetComponentRW<LocalTransform>(borderPrefabRight);
//        trans.ValueRW.Scale = mapWidth / 10.24f;
//        transBorderLeft.ValueRW.Scale = mapWidth / 10.24f;
//        transBorderRight.ValueRW.Scale = mapWidth / 10.24f;
//    }

//    /// <summary>
//    /// 读表拿到所需预制体
//    /// </summary>
//    /// <param name="state"></param>
//    /// <param name="mapID"></param>
//    /// <param name="mapPrefab">地图</param>
//    /// <param name="borderPrefabLeft">左边缘</param>
//    /// <param name="borderPrefabRight">右边缘</param>
//    /// <param name="borderPrefabTop">上边缘</param>
//    /// <param name="borderPrefabBottom">下边缘</param>
//    private void GetNeedPrefab(ref SystemState state, int mapID, out Entity mapPrefab,
//        out NativeList<Entity> borderPrefabLeft, out NativeList<Entity> borderPrefabRight,
//        out NativeList<Entity> borderPrefabTop, out NativeList<Entity> borderPrefabBottom)
//    {
//        mapPrefab = default;
//        borderPrefabBottom = new NativeList<Entity>(Allocator.Temp);
//        borderPrefabTop = new NativeList<Entity>(Allocator.Temp);
//        borderPrefabLeft = new NativeList<Entity>(Allocator.Temp);
//        borderPrefabRight = new NativeList<Entity>(Allocator.Temp);
//        if (!SystemAPI.TryGetSingleton<GlobalConfigData>(out GlobalConfigData globalConfigData))
//        {
//            Debug.Log($"没读到GlobalConfigData");
//            return;
//        }

//        ;
//        if (!SystemAPI.TryGetSingleton<PrefabMapData>(out PrefabMapData prefabMapData))
//        {
//            Debug.Log($"没读到prefabMapData");
//            return;
//        }

//        ;
//        ref var mapArray = ref globalConfigData.value.Value.configTbmap_types.configTbmap_types;
//        for (int i = 0; i < mapArray.Length; i++)
//        {
//            if (mapArray[i].id == mapID)
//            {
//                mapPrefab = prefabMapData.prefabHashMap[mapArray[i].bg];
//                for (int j = 0; j < mapArray[i].bgLeft.Length; j++)
//                {
//                    borderPrefabLeft.Add(prefabMapData.prefabHashMap[mapArray[i].bgLeft[j]]);
//                }

//                for (int j = 0; j < mapArray[i].bgRight.Length; j++)
//                {
//                    borderPrefabRight.Add(prefabMapData.prefabHashMap[mapArray[i].bgRight[j]]);
//                }

//                for (int j = 0; j < mapArray[i].bgBottom.Length; j++)
//                {
//                    borderPrefabBottom.Add(prefabMapData.prefabHashMap[mapArray[i].bgBottom[j]]);
//                }

//                for (int j = 0; j < mapArray[i].bgTop.Length; j++)
//                {
//                    borderPrefabTop.Add(prefabMapData.prefabHashMap[mapArray[i].bgTop[j]]);
//                }

//                break;
//            }
//        }
//    }

//    private Entity GetRandomEntity(NativeList<Entity> entities, ref SystemState state)
//    {
//        if (!SystemAPI.TryGetSingletonEntity<GameRandomData>(out Entity wbe)) return default;
//        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
//        var gameRandomData = SystemAPI.GetSingletonRW<GameRandomData>().ValueRW;
//        var random = gameRandomData.rand;
//        int randomIndex = random.NextInt(0, entities.Length);
//        gameRandomData.rand = random;
//        ecb.SetComponent(wbe, gameRandomData);
//        ecb.Playback(state.EntityManager);
//        return entities[randomIndex];
//    }

//    private void GenerateNewMapTop(ref SystemState state, int mapHeight, int mapWidth, int currentMapIndex,
//        float limitTop, int mapID)
//    {
//        Debug.Log($"limitTop{limitTop}");
//        GetNeedPrefab(ref state, mapID, out Entity mapPrefab, out NativeList<Entity> borderPrefabLeft,
//            out NativeList<Entity> borderPrefabRight, out NativeList<Entity> borderPrefabTop,
//            out NativeList<Entity> borderPrefabBottom);
//        if (mapPrefab == default)
//        {
//            return;
//        }

//        Debug.Log($"1111111111向上生成");

//        var bottmoPos = new float3(mapX, limitTop, 0);
//        var startPos = new float3(bottmoPos.x, bottmoPos.y + mapHeight, 0);

//        //拼map 设置网格起始位置
//        int needCount = mapHeight / mapWidth;
//        for (int i = 0; i < needCount; i++)
//        {
//            var borderR = GetRandomEntity(borderPrefabRight, ref state);
//            var borderL = GetRandomEntity(borderPrefabLeft, ref state);

//            //调整地图预制体大小
//            AjustMap(ref state, mapPrefab, borderR, borderL, mapHeight, mapWidth);
//            Entity newMapEntity = state.EntityManager.Instantiate(mapPrefab);

//            if (newMapEntity == default) return;
//            int nextMapIndex = currentMapIndex + 1;
//            if (SystemAPI.HasComponent<MapBaseData>(newMapEntity))
//            {
//                state.EntityManager.SetComponentData(newMapEntity,
//                    new MapBaseData { mapIndex = nextMapIndex });
//            }
//            else
//            {
//                state.EntityManager.AddComponentData(newMapEntity,
//                    new MapBaseData { mapIndex = nextMapIndex });
//            }

//            MapPosStruct mapPosStruct = new MapPosStruct();
//            var startTempPos = new float3(startPos.x, startPos.y - i * mapWidth, 0);
//            var endTempPos = new float3(startTempPos.x, startTempPos.y - mapWidth, 0);

//            mapPosStruct.startPos = startTempPos;
//            mapPosStruct.pathPos = endTempPos;
//            var trans = SystemAPI.GetComponentRW<LocalTransform>(newMapEntity);
//            trans.ValueRW.Position = new float3(0, (startTempPos.y + endTempPos.y) / 2f, 0);

//            InstanceBorder(ref state, trans.ValueRW, borderR, borderL, mapWidth, nextMapIndex);
//            //初始化网格数据
//            DynamicBuffer<MapIndexPosBuffer> mapPosTemp = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>();
//            if (mapPosTemp.IsCreated)
//            {
//                mapPosTemp.Add(new MapIndexPosBuffer
//                { mapPosStruct = mapPosStruct, mapIndex = currentMapIndex + 1 });
//            }
//        }

//        var mapPos = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>();
//        for (int i = 0; i < mapPos.Length; i++)
//        {
//            if (mapPos[i].mapPosStruct.startPos.y > limitTop)
//            {
//                limitTop = mapPos[i].mapPosStruct.startPos.y;
//            }

//        }
//        if (!SystemAPI.TryGetSingletonRW<MapGloablaData>(out RefRW<MapGloablaData> mapGlobalData)) return;
//        mapGlobalData.ValueRW.minAndMaxPosY.y = limitTop;
//    }

//    public void AddDataToBorder(Entity entity, ref SystemState state, int mapIndex)
//    {
//        state.EntityManager.AddComponent<ObstacleTag>(entity);


//        var elementData = new MapElementData { };


//        ////初始化地形信息
//        elementData.belongsIndex = mapIndex;
//        elementData.elementID = 2001;
//        //给area添加基础信息
//        state.EntityManager.AddComponentData(entity, elementData);

//        if (state.EntityManager.HasComponent<MapElementData>(entity))
//        {
//            state.EntityManager.RemoveComponent<MapElementData>(entity);
//        }
//    }


//    private void InstanceBorder(ref SystemState state, LocalTransform currentLoc, Entity borderLeft, Entity borderRight,
//        int mapWidth, int currentMapIndex)
//    {
//        borderLeft = state.EntityManager.Instantiate(borderLeft);
//        borderRight = state.EntityManager.Instantiate(borderRight);
//        AddDataToBorder(borderLeft, ref state, currentMapIndex);
//        AddDataToBorder(borderRight, ref state, currentMapIndex);
//        var locLeft = SystemAPI.GetComponentRW<LocalTransform>(borderLeft);
//        var locRight = SystemAPI.GetComponentRW<LocalTransform>(borderRight);
//        float xPos = mapWidth / 2f + locLeft.ValueRO.Scale * 5.12f / 2;
//        locLeft.ValueRW.Position = new float3(currentLoc.Position.x - xPos, currentLoc.Position.y, 0);
//        locRight.ValueRW.Position = new float3(currentLoc.Position.x + xPos, currentLoc.Position.y, 0);
//        state.EntityManager.AddComponent<ObstacleTag>(borderLeft);
//        state.EntityManager.AddComponent<ObstacleTag>(borderRight);
//    }

//    private void GenerateNewMapBottom(ref SystemState state, int mapHeight, int mapWidth, int currentMapIndex,
//        float limitBottom, int mapID)
//    {
//        GetNeedPrefab(ref state, mapID, out Entity mapPrefab, out NativeList<Entity> borderPrefabLeft,
//            out NativeList<Entity> borderPrefabRight, out NativeList<Entity> borderPrefabTop,
//            out NativeList<Entity> borderPrefabBottom);

//        if (mapPrefab == default)
//        {
//            return;
//        }

//        Debug.Log($"22222222向下生成");
//        var lastBottomPos = new float3(mapX, limitBottom, 0);
//        var startPos = lastBottomPos;

//        //拼map 设置网格起始位置
//        int needCount = mapHeight / mapWidth;
//        for (int i = 0; i < needCount; i++)
//        {
//            var borderR = GetRandomEntity(borderPrefabRight, ref state);
//            var borderL = GetRandomEntity(borderPrefabLeft, ref state);

//            //调整地图预制体大小
//            AjustMap(ref state, mapPrefab, borderR, borderL, mapHeight, mapWidth);
//            Entity newMapEntity = state.EntityManager.Instantiate(mapPrefab);

//            if (newMapEntity == default) return;
//            int nextMapIndex = currentMapIndex + 1;
//            if (SystemAPI.HasComponent<MapBaseData>(newMapEntity))
//            {
//                state.EntityManager.SetComponentData(newMapEntity,
//                    new MapBaseData { mapIndex = nextMapIndex });
//            }
//            else
//            {
//                state.EntityManager.AddComponentData(newMapEntity,
//                    new MapBaseData { mapIndex = nextMapIndex });
//            }

//            //  state.EntityManager.AddComponentData(newMapEntity, new MapBaseData { mapIndex = currentMapIndex + 1 });
//            MapPosStruct mapPosStruct = new MapPosStruct();
//            var startTempPos = new float3(startPos.x, startPos.y - i * mapWidth, 0);
//            var endTempPos = new float3(startTempPos.x, startTempPos.y - mapWidth, 0);
//            mapPosStruct.startPos = startTempPos;
//            mapPosStruct.pathPos = endTempPos;
//            var trans = SystemAPI.GetComponentRW<LocalTransform>(newMapEntity);
//            trans.ValueRW.Position = new float3(0, (startTempPos.y + endTempPos.y) / 2f, 0);
//            InstanceBorder(ref state, trans.ValueRW, borderL, borderR, mapWidth, nextMapIndex);
//            int gridSize = 5;
//            //初始化网格数据
//            DynamicBuffer<MapIndexPosBuffer> mapPosTemp = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>();
//            if (mapPosTemp.IsCreated)
//            {
//                mapPosTemp.Add(new MapIndexPosBuffer
//                { mapPosStruct = mapPosStruct, mapIndex = currentMapIndex + 1 });
//            }
//        }

//        var mapPos = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>();
//        for (int i = 0; i < mapPos.Length; i++)
//        {
//            if (mapPos[i].mapPosStruct.pathPos.y < limitBottom)
//            {
//                limitBottom = mapPos[i].mapPosStruct.pathPos.y;
//                //Debug.Log($"limitBottom{limitBottom}");
//            }
//        }
//        if (!SystemAPI.TryGetSingletonRW<MapGloablaData>(out RefRW<MapGloablaData> mapGlobalData)) return;
//        mapGlobalData.ValueRW.minAndMaxPosY.x = limitBottom;
//    }

//}

