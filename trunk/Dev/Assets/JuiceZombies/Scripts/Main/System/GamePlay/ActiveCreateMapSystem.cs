////---------------------------------------------------------------------
//// UnicornStudio
//// Author: jaco0012
//// Time: 2023-07-17 12:41:01
////---------------------------------------------------------------------

//
//using ProjectDawn.Navigation;
//using System;
//using Unity.Burst;
//using Unity.Burst.Intrinsics;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Transforms;
//using UnityEngine;

//namespace Main
//{
//    //动态生成地图
//    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//    [UpdateAfter(typeof(InitMapSystem))]
//    public partial struct ActiveCreateMapSystem : ISystem
//    {
//        [BurstCompile]
//        public void OnCreate(ref SystemState state)
//        {
//            state.RequireForUpdate<WorldBlackBoardTag>();
//            state.RequireForUpdate<MapIndexPosBuffer>();
//        }


//        [BurstCompile]
//        public void OnUpdate(ref SystemState state)
//        {
//            //var buffQuery = SystemAPI.QueryBuilder().WithAll<Buff>().Build();
//            //var configData = SystemAPI.GetSingleton<GlobalConfigData>();

//            //Debug.Log($"{configData.value.Value.configTbareas.configTbareas[0].areaEvents[0]}");
//            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
//            var mapGlobalData = SystemAPI.GetSingleton<MapGloablaData>();
//            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
//            var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
//            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
//            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

//            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

//            new CreateMapJob
//            {
//                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(),
//                gameRandomData = gameRandomData,
//                globalConfigData = globalConfigData,
//                mapGlobalData = mapGlobalData,
//                fdT = SystemAPI.Time.fixedDeltaTime,
//                prefabMapData = prefabMapData,
//                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
//                player = SystemAPI.GetSingletonEntity<PlayerData>(),
//                ecb = ecb,
//                mapPos = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>(),
//                wbe = wbe
//            }.ScheduleParallel();
//        }


//        [BurstCompile]
//        partial struct CreateMapJob : IJobEntity
//        {
//            public EntityCommandBuffer.ParallelWriter ecb;


//            [NativeDisableParallelForRestriction] public DynamicBuffer<MapIndexPosBuffer> mapPos;
//            [NativeDisableParallelForRestriction] public MapGloablaData mapGlobalData;
//            //[NativeDisableParallelForRestriction] public ComponentLookup<PlayerData> cdfePlayerData;
//            [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> cdfeLocalTransform;

//            [ReadOnly] public PrefabMapData prefabMapData;
//            [ReadOnly] public GlobalConfigData globalConfigData;
//            public GameRandomData gameRandomData;
//            [ReadOnly] public float fdT;
//            public EntityStorageInfoLookup storageInfoFromEntity;
//            [ReadOnly] public Entity player;
//            [ReadOnly] public Entity wbe;

//            public void Execute([EntityIndexInQuery] int index, Entity entity, ref MapBaseData mapInfo, ref RefreshTag refreshs)
//            {

//                //Debug.Log("ExecuteExecute");
//                if (mapPos.Length <= 0) return;
//                int currentMapIndex = mapPos[mapPos.Length - 1].mapIndex;
//                //var mapX = mapPos[mapPos.Length - 1].mapPosStruct.startPos.x;
//                if (currentMapIndex < 1) return;
//                if (currentMapIndex > 3)
//                {
//                    //销毁第一张地图
//                }

//                var playerLoc = cdfeLocalTransform[player];
//                var mapType = mapGlobalData.mapType;
//                var currentMapID = mapGlobalData.mapID;
//                var height = mapGlobalData.mapSize.y;
//                var width = mapGlobalData.mapSize.x;
//                if (mapType == 1)
//                {
//                    var limitBottom = mapGlobalData.minAndMaxPosY.x;
//                    var limitTop = mapGlobalData.minAndMaxPosY.y;
//                    float currentTop = playerLoc.Position.y + StaticEnumDefine.maxCameraSize;

//                    float currentBottom = playerLoc.Position.y - StaticEnumDefine.maxCameraSize;
//                    if (currentTop >= limitTop)
//                    {
//                        GenerateNewMapTop(index, height, width, currentMapIndex, limitTop, currentMapID);

//                    }
//                    else if (currentBottom <= limitBottom)
//                    {
//                        GenerateNewMapBottom(index, height, width, currentMapIndex, limitBottom, currentMapID);

//                    }


//                }
//                else if (mapType == 3)
//                {
//                    var loctrans = cdfeLocalTransform[entity];
//                    var rect = new Rect(loctrans.Position.x, loctrans.Position.y, width / 3f, width / 3f);
//                    if (rect.Contains(playerLoc.Position))
//                    {
//                        int refreshNum = refreshs.refreshIndex;
//                        Debug.Log($"位于九宫格的第{refreshNum}个格子内");
//                        float3 startPos = mapPos[mapPos.Length - 1].mapPosStruct.startPos;
//                        float3 endPos = mapPos[mapPos.Length - 1].mapPosStruct.pathPos;
//                        GenerateNewMap(startPos, endPos, refreshNum);

//                    }
//                }

//            }
//            private void EnableGenerateItem()
//            {
//                Debug.Log("EnableGenerateItem");
//                //生成item

//                //var systemHandle = state.WorldUnmanaged.GetExistingUnmanagedSystem<AreaAnd0bstacleGenerateSystem>();
//                //ref var stateRef = ref state.WorldUnmanaged.ResolveSystemStateRef(systemHandle);
//                //stateRef.Enabled = true;
//            }
//            private void GenerateNewMapTop(int index, int mapHeight, int mapWidth, int currentMapIndex,
//      float limitTop, int mapID)
//            {
//                Debug.Log($"limitTop{limitTop}");

//                Debug.Log($"1111111111向上生成");
//                GetNeedPrefab(mapID, out Entity mapPrefab, out NativeList<Entity> borderPrefabLeft,
//                   out NativeList<Entity> borderPrefabRight);

//                var bottmoPos = new float3(-mapWidth / 2f, limitTop, 0);
//                var startPos = new float3(bottmoPos.x, bottmoPos.y + mapHeight, 0);

//                //拼map 设置网格起始位置
//                int needCount = mapHeight / mapWidth;
//                for (int i = 0; i < needCount; i++)
//                {
//                    var borderR = GetRandomEntity(borderPrefabRight);
//                    var borderL = GetRandomEntity(borderPrefabLeft);

//                    //调整地图预制体大小
//                    AjustMap(mapPrefab, borderR, borderL, mapHeight, mapWidth);
//                    Entity newMapEntity = ecb.Instantiate(index, mapPrefab);
//                    int nextMapIndex = currentMapIndex + 1;
//                    ecb.AddComponent<MapBaseData>(index, newMapEntity, new MapBaseData { mapIndex = nextMapIndex });

//                    MapPosStruct mapPosStruct = new MapPosStruct();
//                    var startTempPos = new float3(startPos.x, startPos.y - i * mapWidth, 0);
//                    var endTempPos = new float3(startTempPos.x, startTempPos.y - mapWidth, 0);

//                    mapPosStruct.startPos = startTempPos;
//                    mapPosStruct.pathPos = endTempPos;
//                    var trans = cdfeLocalTransform[newMapEntity];
//                    trans.Position = new float3(0, (startTempPos.y + endTempPos.y) / 2f, 0);
//                    cdfeLocalTransform[newMapEntity] = trans;

//                    InstanceBorder(index, trans, borderR, borderL, mapWidth, nextMapIndex);
//                    //if (mapPos.IsCreated)
//                    //{
//                    //    mapPos.Add(new MapIndexPosBuffer
//                    //    { mapPosStruct = mapPosStruct, mapIndex = currentMapIndex + 1 });
//                    //}


//                }
//            }


//            private void InstanceBorder(int sortKey, LocalTransform currentLoc, Entity borderLeft, Entity borderRight,
//      int mapWidth, int currentMapIndex)
//            {
//                borderLeft = ecb.Instantiate(sortKey, borderLeft);
//                borderRight = ecb.Instantiate(sortKey, borderRight);
//                AddDataToBorder(borderLeft, sortKey, currentMapIndex);
//                AddDataToBorder(borderRight, sortKey, currentMapIndex);
//                var locLeft = cdfeLocalTransform[borderLeft];
//                var locRight = cdfeLocalTransform[borderRight];
//                float xPos = mapWidth / 2f + locLeft.Scale * 5.12f / 2;
//                locLeft.Position = new float3(currentLoc.Position.x - xPos, currentLoc.Position.y, 0);
//                locRight.Position = new float3(currentLoc.Position.x + xPos, currentLoc.Position.y, 0);
//                cdfeLocalTransform[borderLeft] = locLeft;
//                cdfeLocalTransform[borderRight] = locRight;
//            }
//            public void AddDataToBorder(Entity entity, int sortKey, int mapIndex)
//            {
//                ecb.AddComponent<ObstacleTag>(sortKey, entity);


//                var elementData = new MapElementData { };


//                ////初始化地形信息
//                elementData.belongsIndex = mapIndex;
//                elementData.elementID = 2001;
//                //给area添加基础信息
//                ecb.AddComponent(sortKey, entity, elementData);

//                ecb.RemoveComponent<MapElementData>(sortKey, entity);
//            }
//            private void GenerateNewMapBottom(int index, int mapHeight, int mapWidth, int currentMapIndex,
//      float limitBottom, int mapID)
//            {
//                GetNeedPrefab(mapID, out Entity mapPrefab, out NativeList<Entity> borderPrefabLeft,
//                    out NativeList<Entity> borderPrefabRight);

//                if (mapPrefab == default)
//                {
//                    return;
//                }

//                Debug.Log($"22222222向下生成");
//                var lastBottomPos = new float3(-mapWidth / 2f, limitBottom, 0);
//                var startPos = lastBottomPos;

//                //拼map 设置网格起始位置
//                int needCount = mapHeight / mapWidth;
//                for (int i = 0; i < needCount; i++)
//                {
//                    var borderR = GetRandomEntity(borderPrefabRight);
//                    var borderL = GetRandomEntity(borderPrefabLeft);

//                    //调整地图预制体大小
//                    AjustMap(mapPrefab, borderR, borderL, mapHeight, mapWidth);
//                    Entity newMapEntity = ecb.Instantiate(index, mapPrefab);
//                    int nextMapIndex = currentMapIndex + 1;
//                    ecb.AddComponent(index, newMapEntity, new MapBaseData { mapIndex = nextMapIndex });
//                    MapPosStruct mapPosStruct = new MapPosStruct();
//                    var startTempPos = new float3(startPos.x, startPos.y - i * mapWidth, 0);
//                    var endTempPos = new float3(startTempPos.x, startTempPos.y - mapWidth, 0);
//                    mapPosStruct.startPos = startTempPos;
//                    mapPosStruct.pathPos = endTempPos;
//                    var trans = cdfeLocalTransform[newMapEntity];
//                    trans.Position = new float3(0, (startTempPos.y + endTempPos.y) / 2f, 0);
//                    cdfeLocalTransform[newMapEntity] = trans;

//                    InstanceBorder(index, trans, borderR, borderL, mapWidth, nextMapIndex);
//                    //初始化网格数据

//                    if (mapPos.IsCreated)
//                    {
//                        mapPos.Add(new MapIndexPosBuffer
//                        { mapPosStruct = mapPosStruct, mapIndex = currentMapIndex + 1 });
//                    }
//                }
//            }

//            private Entity GetRandomEntity(NativeList<Entity> entities)
//            {

//                EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);
//                var random = gameRandomData.rand;
//                int randomIndex = random.NextInt(0, entities.Length);
//                gameRandomData.rand = random;
//                ecb.SetComponent(wbe, gameRandomData);
//                return entities[randomIndex];
//            }

//            private void AjustMap(Entity mapPrefab, Entity borderPrefabRight, Entity borderPrefabLeft,
//                   int mapheight, int mapWidth)
//            {
//                var trans = cdfeLocalTransform[mapPrefab];
//                var transBorderLeft = cdfeLocalTransform[borderPrefabLeft];
//                var transBorderRight = cdfeLocalTransform[borderPrefabRight];
//                trans.Scale = mapWidth / 10.24f;
//                transBorderLeft.Scale = mapWidth / 10.24f;
//                transBorderRight.Scale = mapWidth / 10.24f;
//                cdfeLocalTransform[mapPrefab] = trans;
//                cdfeLocalTransform[borderPrefabLeft] = transBorderLeft;
//                cdfeLocalTransform[borderPrefabRight] = transBorderRight;
//            }
//            private void GenerateNewMap(float3 startPos, float3 endPos, int refreshNum)
//            {
//                //GetNeedPrefab()
//                //if (refreshNum == 1 || refreshNum == 2 || refreshNum == 3)
//                //{

//                //    var perWidth = mapWidth / 3f;
//                //    var perHeight = mapheight / 3f;
//                //    startPos = new float3(-perWidth, perHeight, 0);
//                //    MapPosStruct mapPosStruct = new MapPosStruct();
//                //    var startTempPos = new float3(startPos.x - perWidth / 2f, startPos.y + perHeight / 2f, 0);
//                //    var endTempPos = new float3(startTempPos.x, startTempPos.y + mapheight, 0);
//                //    mapPosStruct.startPos = startTempPos;
//                //    mapPosStruct.pathPos = endTempPos;
//                //for (int i = 0; i < 3; i++)
//                //{
//                //    for (int j = 0; j < 3; j++)
//                //    {
//                //        var currentPos = new float3(startPos.x + i * perWidth, startPos.y - j * perHeight, 0);
//                //        var newMap = ecb.Instantiate(mapPrefab);
//                //        var trans = SystemAPI.GetComponentRW<LocalTransform>(newMap);
//                //        trans.ValueRW.Scale = perHeight / 10.24f;
//                //        trans.ValueRW.Position = currentPos;
//                //        ecb.AddComponent.AddComponentData(newMap, new MapBaseData { mapIndex = 1, mapID = mapID });
//                //        state.EntityManager.AddComponentData(newMap, new RefreshTag { refreshIndex = i * 3 + j + 1 });
//                //    }
//                //}

//                //    mapPos = SystemAPI.GetSingletonBuffer<MapIndexPosBuffer>();
//                //    Debug.LogError($"mapPos{mapPos.Length}");
//                //    if (mapPos.IsCreated)
//                //    {
//                //        Debug.LogError($"mapPos 222");
//                //        mapPos.Clear();
//                //        mapPos.Add(new MapIndexPosBuffer
//                //        { gridSize = 5, mapPosStruct = mapPosStruct, mapIndex = 1 });
//                //    }
//                //    GenerateMapTop(startPos, endPos);
//                //    //向上生成
//                //}
//                //if (refreshNum == 1 || refreshNum == 4 || refreshNum == 7)
//                //{
//                //    GenerateMapLeft();
//                //    //向左生成
//                //}
//                //if (refreshNum == 7 || refreshNum == 8 || refreshNum == 9)
//                //{
//                //    GenerateMapBottom();
//                //    //向下生成
//                //}
//                //if (refreshNum == 3 || refreshNum == 6 || refreshNum == 9)
//                //{
//                //    GenerateMapRight();
//                //    //向右生成
//                //}
//            }

//            private void GetNeedPrefab(int mapID, out Entity mapPrefab, out NativeList<Entity> borderPrefabLeft, out NativeList<Entity> borderPrefabRight)
//            {
//                mapPrefab = default;
//                borderPrefabLeft = new NativeList<Entity>(Allocator.Temp);
//                borderPrefabRight = new NativeList<Entity>(Allocator.Temp);
//                ref var mapArray = ref globalConfigData.value.Value.configTbmap_types.configTbmap_types;
//                for (int i = 0; i < mapArray.Length; i++)
//                {
//                    if (mapArray[i].id == mapID)
//                    {
//                        mapPrefab = prefabMapData.prefabHashMap[mapArray[i].bg];
//                        for (int j = 0; j < mapArray[i].bgLeft.Length; j++)
//                        {
//                            borderPrefabLeft.Add(prefabMapData.prefabHashMap[mapArray[i].bgLeft[j]]);
//                        }

//                        for (int j = 0; j < mapArray[i].bgRight.Length; j++)
//                        {
//                            borderPrefabRight.Add(prefabMapData.prefabHashMap[mapArray[i].bgRight[j]]);
//                        }
//                        break;
//                    }
//                }
//            }
//        }


//    }
//}

