//---------------------------------------------------------------------
// JiYuStudio
// Author: 如初
// Time: 2024-04-11 10:30:51
//---------------------------------------------------------------------

using cfg.blobstruct;
using cfg.config;
using ProjectDawn.Navigation;
using System;
using System.Globalization;

//using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct SpawnMapAndAreaSystem : ISystem
    {
        public const uint PosMaxValue = 99999;

        //一个刷新组的item最大值
        public const int MaxLengthRefreshGroup = 8;

        //选点的最大次数
        public const int MaxSelectPosTimes = 1000;

        public const int MaxCameraSize = 200;
        public const int MaxSelfRect = 10;
        public const int PlayerInitRect = 50;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            //state.Enabled = false;
            //state.RequireForUpdate<CrowdSurface>();
            //state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<StateMachine, AnimatorAspect>().Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var elementQuery = SystemAPI.QueryBuilder().WithAny<AreaTag, ObstacleTag>().Build();
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            var gameOtherData = SystemAPI.GetComponent<GameOthersData>(wbe);

            var mapBaseQuery = SystemAPI.QueryBuilder().WithAll<MapBaseData>().Build();
            var gametimeData = SystemAPI.GetComponent<GameTimeData>(wbe);
            //var randInt = (int)Stopwatch.GetTimestamp();
            new SpawnMapAndAreaSystemJob
            {
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(),
                cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(true),
                player = SystemAPI.GetSingletonEntity<PlayerData>(),
                gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe),
                gameOthersData = gameOtherData,
                prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe),
                globalConfigData = globalConfigData,
                wbe = wbe,
                ecbs = singleton.CreateCommandBuffer(state.WorldUnmanaged),
                elements = elementQuery.ToEntityArray(Allocator.TempJob),
                maps = mapBaseQuery.ToEntityArray(Allocator.TempJob),

                cdfePhysicsCollider = SystemAPI.GetComponentLookup<PhysicsCollider>(),
                cdfeBoardTag = SystemAPI.GetComponentLookup<BoardData>(true),
                timeTick = (uint)((SystemAPI.Time.ElapsedTime * 1000f)),
                gameTimeTick = gametimeData.refreshTime.tick,
                dT = SystemAPI.Time.fixedDeltaTime,
                cdfeMapElementData = SystemAPI.GetComponentLookup<MapElementData>(true),
                cdfePostTransformMatrix = SystemAPI.GetComponentLookup<PostTransformMatrix>(true),
            }.Schedule();
        }

        [BurstCompile]
        private partial struct SpawnMapAndAreaSystemJob : IJobEntity
        {
            public ComponentLookup<LocalTransform> cdfeLocalTransform;
            [ReadOnly] public ComponentLookup<PlayerData> cdfePlayerData;
            [ReadOnly] public ComponentLookup<BoardData> cdfeBoardTag;
            [ReadOnly] public Entity player;
            [ReadOnly] public float dT;

            [ReadOnly] public PrefabMapData prefabMapData;

            [NativeDisableParallelForRestriction] public GameRandomData gameRandomData;
            [ReadOnly] public int gameTimeTick;
            [ReadOnly] public GameOthersData gameOthersData;
            [ReadOnly] public GlobalConfigData globalConfigData;

            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> elements;

            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> maps;
            [ReadOnly] public uint timeTick;

            [ReadOnly] public ComponentLookup<MapElementData> cdfeMapElementData;

            [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
            [ReadOnly] public Entity wbe;
            [ReadOnly] public ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix;
            public EntityCommandBuffer ecbs;

            public void Execute(Entity entity, ref MapRefreshData mapRefreshData)
            {
                //Debug.LogError($"{timeTick}");
                int mapID = gameOthersData.mapData.mapID;
                float mapheight = gameOthersData.mapData.mapSize.y;
                float mapWidth = gameOthersData.mapData.mapSize.x;
                int mapType = gameOthersData.mapData.mapType;

                if (!mapRefreshData.isMapInit)
                {
                    mapRefreshData.isMapInit = true;

                    //适用于上下无限地图
                    if (mapType == 1)
                    {
                        GetNeedPrefab(mapID, out Entity mapPrefab, out NativeList<Entity> listLeft,
                            out NativeList<Entity> listRight, out NativeList<Entity> listBottom,
                            out NativeList<Entity> listTop);
                        GetNeedPrefab(mapID, out mapPrefab, out NativeHashMap<int, bool> borderPrefabLeft,
                            out NativeHashMap<int, bool> borderPrefabRight,
                            out NativeHashMap<int, bool> borderPrefabTop,
                            out NativeHashMap<int, bool> borderPrefabBottom);
                        float3 maxPosUpleft = float3.zero;
                        float3 minPosBottomLeft = float3.zero;
                        int minIndex = 0;
                        int maxIndex = 0;
                        for (int j = 0; j < 3; j++)
                        {
                            int mapIndex = j + 1;
                            float3 startPos = new float3(-0.5f * mapWidth, mapheight * (j - 0.5f), 0);
                            for (int k = 0; k < mapheight / mapWidth; k++)
                            {
                                var mapCenter = ecbs.Instantiate(mapPrefab);

                                var mapLeft = InstanceBorder(borderPrefabLeft, k, listLeft);
                                var mapRight = InstanceBorder(borderPrefabRight, k, listRight);
                                float ajustPar = k == 0 ? 1 : -1;
                                ajustPar = j - 0.25f * ajustPar;
                                AjustEntityScaleAndPos(mapCenter, 0, ajustPar);
                                AjustEntityScaleAndPos(mapLeft, -1, ajustPar);
                                AjustEntityScaleAndPos(mapRight, 1, ajustPar);

                                ecbs.SetComponent(mapLeft, new BoardData
                                {
                                    type = BoardTypeEnum.Left
                                });
                                ecbs.SetComponent(mapRight, new BoardData
                                {
                                    type = BoardTypeEnum.Right
                                });
                                ecbs.SetComponent<MapBaseData>(mapCenter, new MapBaseData
                                {
                                    mapIndex = mapIndex
                                });
                            }

                            if (startPos.y > maxPosUpleft.y)
                            {
                                maxPosUpleft = startPos;
                            }

                            if ((startPos.y - mapheight) < minPosBottomLeft.y)
                            {
                                minPosBottomLeft = new float3(startPos.x, startPos.y - mapheight, 0);
                            }

                            minIndex = mapIndex < minIndex ? minIndex : minIndex;
                            maxIndex = mapIndex > maxIndex ? mapIndex : maxIndex;

                            mapRefreshData.maxPosUpleft = maxPosUpleft;
                            mapRefreshData.minPosBottomLeft = minPosBottomLeft;

                            mapRefreshData.maxIndex = maxIndex;
                            mapRefreshData.minIndex = minIndex;

                            Debug.Log($"startPos:{startPos}");
                            InitMapElements(startPos, mapIndex);

                            ecbs.SetComponent(wbe, mapRefreshData);
                        }
                    }
                    else if (mapType == 2)
                    {
                    }
                    //适用于全封闭
                    else if (mapType == 3)
                    {
                        GetNeedPrefab(mapID, out Entity mapPrefab, out NativeList<Entity> borderPrefabLeft,
                            out NativeList<Entity> borderPrefabRight, out NativeList<Entity> borderPrefabTop,
                            out NativeList<Entity> borderPrefabBottom);
                        var borderLeft = GetRandomEntity(borderPrefabLeft);
                        var borderRight = GetRandomEntity(borderPrefabRight);
                        var borderTop = GetRandomEntity(borderPrefabTop);
                        var borderBottom = GetRandomEntity(borderPrefabBottom);
                        var newMap = ecbs.Instantiate(mapPrefab);
                        borderLeft = ecbs.Instantiate(borderLeft);
                        borderRight = ecbs.Instantiate(borderRight);
                        borderBottom = ecbs.Instantiate(borderBottom);
                        borderTop = ecbs.Instantiate(borderTop);
                        AjustMapScaleAndPos(newMap, cdfeLocalTransform, 0);
                        AjustMapScaleAndPos(borderLeft, cdfeLocalTransform, -1);
                        AjustMapScaleAndPos(borderRight, cdfeLocalTransform, 1);
                        AjustMapScaleAndPos(borderTop, cdfeLocalTransform, 2);
                        AjustMapScaleAndPos(borderBottom, cdfeLocalTransform, -2);
                        AddDataToBorder(borderLeft);
                        AddDataToBorder(borderRight);
                        AddDataToBorder(borderTop);
                        AddDataToBorder(borderBottom);
                        ecbs.SetComponent(borderLeft, new BoardData
                        {
                            type = BoardTypeEnum.Left
                        });
                        ecbs.SetComponent(borderRight, new BoardData
                        {
                            type = BoardTypeEnum.Right
                        });
                        ecbs.SetComponent(borderTop, new BoardData
                        {
                            type = BoardTypeEnum.Up
                        });
                        ecbs.SetComponent(borderBottom, new BoardData
                        {
                            type = BoardTypeEnum.Down
                        });
                        float3 startPos = new float3(-0.5f * mapWidth, 0.5f * mapheight, 0);
                        Debug.Log($"startPos:{startPos}");
                        InitMapElements(startPos, 1);
                    }
                    //适用于全开放
                    else if (mapType == 4)
                    {
                    }
                }
                else
                {
                    int currentMapIndex = (int)((cdfeLocalTransform[player].Position.y + 360) / mapheight);
                    //动态生成---上下无限
                    if (mapType == 1)
                    {
                        var playerLoc = cdfeLocalTransform[player];
                        var limitBottom = mapRefreshData.minPosBottomLeft.y;
                        var limitTop = mapRefreshData.maxPosUpleft.y;
                        float currentTop = playerLoc.Position.y + MaxCameraSize;

                        float currentBottom = playerLoc.Position.y - MaxCameraSize;
                        if (currentTop >= limitTop)
                        {
                            var e = ecbs.CreateEntity();

                            ecbs.AddComponent(e, new HybridEventData
                            {
                                type = 1,
                                args = default
                            });
                            ecbs.AddComponent(e, new TimeToDieData
                            {
                                duration = 10
                            });
                            GenerateNewMapTop(ref mapRefreshData, limitTop, mapID, mapWidth, mapheight,
                                cdfeLocalTransform);
                        }
                        else if (currentBottom <= limitBottom)
                        {
                            var e = ecbs.CreateEntity();

                            ecbs.AddComponent(e, new HybridEventData
                            {
                                type = 1,
                                args = default
                            });
                            ecbs.AddComponent(e, new TimeToDieData
                            {
                                duration = 10
                            });
                            GenerateNewMapBottom(ref mapRefreshData, limitBottom, mapID, mapWidth, mapheight,
                                cdfeLocalTransform);
                        }
                    }
                    else if (mapType == 2)
                    {
                    }
                    //适用于全封闭
                    else if (mapType == 3)
                    {
                    }
                    //动态生成---全开放
                    else if (mapType == 4)
                    {
                        ref var scene_modulesConfig =
                            ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
                        int scene_modulesIndex = 0;
                        for (int i = 0; i < scene_modulesConfig.Length; i++)
                        {
                            if (scene_modulesConfig[i].id == mapID)
                            {
                                scene_modulesIndex = i;
                                break;
                            }
                        }

                        ref var scenemodule = ref scene_modulesConfig[scene_modulesIndex];
                        if (!prefabMapData.prefabHashMap.TryGetValue(scenemodule.model, out var prefab))
                        {
                            Debug.LogError($"没找到预制件:{scenemodule.model} ");
                        }

                        const float MapSize = 120f;
                        const float BigMapSize = 360f;

                        const float SmallDetectRatius = 100f;

                        float bigDetectRadius = math.length(new float2(BigMapSize)) - math.length(new float2(MapSize));

                        int curMapPosXIndex =
                            (int)((cdfeLocalTransform[player].Position.x + BigMapSize / 2f) / BigMapSize);

                        int curMapPosYIndex =
                            (int)((cdfeLocalTransform[player].Position.y + BigMapSize / 2f) / BigMapSize);
                        if (cdfeLocalTransform[player].Position.x < 0)
                        {
                            curMapPosXIndex =
                                (int)((cdfeLocalTransform[player].Position.x - BigMapSize / 2f) / BigMapSize);
                        }

                        if (cdfeLocalTransform[player].Position.y < 0)
                        {
                            curMapPosYIndex =
                                (int)((cdfeLocalTransform[player].Position.y - BigMapSize / 2f) / BigMapSize);
                        }


                        float2 curMapPos = new float2(curMapPosXIndex * BigMapSize, curMapPosYIndex * BigMapSize);
                        int mapCount = 3;
                        //Debug.Log($"x{curMapPosXIndex} y{curMapPosYIndex}");
                        //SpawnABigMap(prefab, MapSize, new float2());
                        for (int i = 0; i < mapCount; i++)
                        {
                            for (int j = 0; j < mapCount; j++)
                            {
                                float posX = i * BigMapSize - (mapCount / 2) * BigMapSize;
                                float posY = j * BigMapSize - (mapCount / 2) * BigMapSize;
                                var offset = new float2(posX, posY);

                                var newPos = new float2(offset + curMapPos);
                                var distance = math.distance(cdfeLocalTransform[player].Position.xy, newPos.xy);
                                if (distance > bigDetectRadius)
                                {
                                    continue;
                                }

                                //Debug.LogError($"{newPos}");
                                bool isExist = false;
                                foreach (var map in maps)
                                {
                                    var dis = math.distance(newPos, cdfeLocalTransform[map].Position.xy);
                                    if (dis < SmallDetectRatius)
                                    {
                                        isExist = true;
                                        break;
                                    }
                                }

                                if (!isExist)
                                {
                                    //Debug.LogError($"{newPos}");
                                    SpawnABigMap(prefab, MapSize, newPos);
                                    float3 genElePos =
                                        new float3(newPos + new float2(-(BigMapSize / 2f), BigMapSize / 2f), 0);
                                    InitMapElements(genElePos, i * 10 + j);
                                }
                            }
                        }
                    }

                    GenerateMapModuleByTime(
                        (int)((cdfeLocalTransform[player].Position.y + mapWidth * 3) / mapheight) + 1,
                        mapType);
                }
            }

            #region 生成地图

            private void GetNeedPrefab(int mapID, out Entity mapPrefab, out NativeList<Entity> borderPrefabLeft,
                out NativeList<Entity> borderPrefabRight, out NativeList<Entity> borderPrefabBottom,
                out NativeList<Entity> borderPrefabTop)
            {
                mapPrefab = default;
                borderPrefabLeft = new NativeList<Entity>(5, Allocator.Temp);
                borderPrefabRight = new NativeList<Entity>(5, Allocator.Temp);
                borderPrefabBottom = new NativeList<Entity>(5, Allocator.Temp);
                borderPrefabTop = new NativeList<Entity>(5, Allocator.Temp);
                FixedString32Bytes flipStr = $"_flip";
                ref var mapArray = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
                for (int i = 0; i < mapArray.Length; i++)
                {
                    if (mapArray[i].id == mapID)
                    {
                        mapPrefab = prefabMapData.prefabHashMap[mapArray[i].model];
                        for (int j = 0; j < mapArray[i].bgLeft.Length; j++)
                        {
                            if (mapArray[i].bgLeft[j].Contains(flipStr))
                            {
                                int index = mapArray[i].bgLeft[j].IndexOf(flipStr);
                                FixedString128Bytes newString = mapArray[i].bgLeft[j].Substring(0, index);
                                borderPrefabLeft.Add(prefabMapData.prefabHashMap[newString]);
                            }
                            else
                            {
                                borderPrefabLeft.Add(prefabMapData.prefabHashMap[mapArray[i].bgLeft[j]]);
                            }
                        }

                        for (int j = 0; j < mapArray[i].bgRight.Length; j++)
                        {
                            if (mapArray[i].bgRight[j].Contains(flipStr))
                            {
                                int index = mapArray[i].bgRight[j].IndexOf(flipStr);
                                FixedString128Bytes newString = mapArray[i].bgRight[j].Substring(0, index);
                                borderPrefabRight.Add(prefabMapData.prefabHashMap[newString]);
                            }
                            else
                            {
                                borderPrefabRight.Add(prefabMapData.prefabHashMap[mapArray[i].bgRight[j]]);
                            }
                        }

                        for (int j = 0; j < mapArray[i].bgDown.Length; j++)
                        {
                            borderPrefabBottom.Add(prefabMapData.prefabHashMap[mapArray[i].bgDown[j]]);
                        }

                        for (int j = 0; j < mapArray[i].bgUp.Length; j++)
                        {
                            borderPrefabTop.Add(prefabMapData.prefabHashMap[mapArray[i].bgUp[j]]);
                        }

                        break;
                    }
                }
            }

            private void GetNeedPrefab(int mapID, out Entity mapPrefab,
                out NativeHashMap<int, bool> borderPrefabLeft,
                out NativeHashMap<int, bool> borderPrefabRight, out NativeHashMap<int, bool> borderPrefabBottom,
                out NativeHashMap<int, bool> borderPrefabTop)
            {
                mapPrefab = default;
                ref var mapArray = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
                borderPrefabLeft = default;
                borderPrefabRight = default;
                borderPrefabBottom = default;
                borderPrefabTop = default;
                for (int i = 0; i < mapArray.Length; i++)
                {
                    if (mapArray[i].id == mapID)
                    {
                        borderPrefabLeft = new NativeHashMap<int, bool>(mapArray[i].bgLeft.Length, Allocator.Temp);
                        borderPrefabRight = new NativeHashMap<int, bool>(mapArray[i].bgRight.Length, Allocator.Temp);
                        borderPrefabBottom = new NativeHashMap<int, bool>(mapArray[i].bgDown.Length, Allocator.Temp);
                        borderPrefabTop = new NativeHashMap<int, bool>(mapArray[i].bgUp.Length, Allocator.Temp);

                        FixedString32Bytes flipStr = $"_flip";
                        mapPrefab = prefabMapData.prefabHashMap[mapArray[i].model];
                        for (int j = 0; j < mapArray[i].bgLeft.Length; j++)
                        {
                            if (mapArray[i].bgLeft[j].Contains(flipStr))
                            {
                                borderPrefabLeft.Add(j, true);
                                //Debug.LogError($"1:{newString},PREFAB{prefabMapData.prefabHashMap[newString].Index}");
                            }
                            else
                            {
                                borderPrefabLeft.Add(j, false);
                                //Debug.LogError($"2:{mapArray[i].bgLeft[j]},PREFAB{prefabMapData.prefabHashMap[mapArray[i].bgLeft[j]].Index}");
                            }
                        }

                        for (int j = 0; j < mapArray[i].bgRight.Length; j++)
                        {
                            if (mapArray[i].bgRight[j].Contains(flipStr))
                            {
                                borderPrefabRight.Add(j, true);
                                //Debug.LogError($"1:{newString}");
                            }
                            else
                            {
                                borderPrefabRight.Add(j, false);
                                //Debug.LogError($"2:{mapArray[i].bgRight[j]}");
                            }
                        }

                        for (int j = 0; j < mapArray[i].bgDown.Length; j++)
                        {
                            if (mapArray[i].bgDown[j].Contains(flipStr))
                            {
                                borderPrefabBottom.Add(j, true);
                            }
                            else
                            {
                                borderPrefabBottom.Add(j, false);
                            }
                        }

                        for (int j = 0; j < mapArray[i].bgUp.Length; j++)
                        {
                            if (mapArray[i].bgUp[j].Contains(flipStr))
                            {
                                borderPrefabTop.Add(j, true);
                            }
                            else
                            {
                                borderPrefabTop.Add(j, false);
                            }
                        }

                        break;
                    }
                }
            }

            private Entity GetRandomEntity(NativeList<Entity> entities)
            {
                EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
                var random = gameRandomData.rand;
                int randomIndex = random.NextInt(0, entities.Length);
                gameRandomData.rand = random;
                ecb.SetComponent(wbe, gameRandomData);
                return entities[randomIndex];
            }


            private void AddDataToBorder(Entity entity)
            {
                ecbs.AddComponent<ObstacleTag>(entity);
                var elementData = new MapElementData { };
                ////初始化地形信息
                elementData.elementID = 2001;
                //给area添加基础信息
                ecbs.SetComponent(entity, elementData);
            }

            private Entity InstanceBorder(NativeHashMap<int, bool> borderPrefab, int order,
                NativeList<Entity> listPrefab)
            {
                //Debug.LogError($"order:{order}");
                //foreach (var i in borderPrefab)
                //{
                //    //Debug.LogError($"entity:{i.Key},{i.Value}");
                //}

                var num = borderPrefab.Count;
                if (num <= 1)
                {
                    order = 0;
                    Debug.LogError("！！！！！！！！！！！！！！！！！！！配表错误！！！！！！！");
                }

                var prefab = listPrefab[order];
                var border = ecbs.Instantiate(prefab);
                if (borderPrefab[order])
                {
                    ecbs.AddComponent(border, new JiYuFlip { value = { x = 1, y = 0 } });
                }

                AddDataToBorder(border);
                return border;
            }

            private void SpawnABigMap(Entity prefab, float MapSize, float2 pos)
            {
                int mapCount = 3;

                for (int i = 0; i < mapCount; i++)
                {
                    for (int j = 0; j < mapCount; j++)
                    {
                        float posX = i * MapSize - (mapCount / 2) * MapSize;
                        float posY = j * MapSize - (mapCount / 2) * MapSize;
                        var offset = new float2(posX, posY);
                        var newPos = new float2(offset + pos);

                        var ins = ecbs.Instantiate(prefab);
                        var tranPrefab = cdfeLocalTransform[prefab];
                        tranPrefab.Position = new float3(newPos, 0);
                        tranPrefab.Scale = MapSize / 10.24f;

                        ecbs.SetComponent(ins, tranPrefab);
                    }
                }
            }

            private void GenerateNewMapTop(ref MapRefreshData mapRefreshData, float limitTop, int mapID, float mapWidth,
                float mapHeight,
                ComponentLookup<LocalTransform> cdfeLocalTransform)
            {
                Debug.Log($"limitTop{limitTop}");

                Debug.Log($"1111111111向上生成");
                var mapIndex = mapRefreshData.maxIndex;

                GetNeedPrefab(mapID, out Entity mapPrefab, out NativeHashMap<int, bool> borderPrefabLeft,
                    out NativeHashMap<int, bool> borderPrefabRight, out NativeHashMap<int, bool> borderPrefabTop,
                    out NativeHashMap<int, bool> borderPrefabBottom);


                var bottmoPos = new float3(-mapWidth / 2f, limitTop, 0);
                var startPos = new float3(bottmoPos.x, bottmoPos.y + mapHeight, 0);

                //拼map 设置网格起始位置
                float needCount = mapHeight / mapWidth;
                for (int i = 0; i < needCount; i++)
                {
                    //调整地图预制体大小
                    AjustMap(mapPrefab, cdfeLocalTransform);

                    Entity newMapEntity = ecbs.Instantiate(mapPrefab);
                    int nextMapIndex = mapIndex + 1;
                    ecbs.SetComponent<MapBaseData>(newMapEntity, new MapBaseData { mapIndex = nextMapIndex });


                    var startTempPos = new float3(startPos.x, startPos.y - i * mapWidth, 0);
                    var endTempPos = new float3(startTempPos.x, startTempPos.y - mapWidth, 0);

                    var trans = cdfeLocalTransform[mapPrefab];
                    var position = new float3(0, (startTempPos.y + endTempPos.y) / 2f, 0);
                    ecbs.SetComponent<LocalTransform>(newMapEntity,
                        new LocalTransform { Position = position, Scale = trans.Scale, Rotation = trans.Rotation });
                    int index = i == 0 ? 1 : 0;
                    InstanceBorderSeq(trans, mapWidth, position, borderPrefabLeft, borderPrefabRight, index, mapID);
                    mapRefreshData.maxIndex = nextMapIndex;
                    ecbs.SetComponent(wbe, mapRefreshData);
                }

                //Debug.LogError($"startPos{startPos}");
                InitMapElements(startPos, mapIndex);

                mapRefreshData.maxPosUpleft = startPos;
                ecbs.SetComponent(wbe, mapRefreshData);
            }

            private void GenerateNewMapBottom(ref MapRefreshData mapRefreshData, float limitBottom, int mapID,
                float mapWidth, float mapHeight,
                ComponentLookup<LocalTransform> cdfeLocalTransform)
            {
                //Debug.Log($"limitTop{limitBottom}");

                Debug.Log($"22222222向下生成");
                var mapIndex = mapRefreshData.minIndex;

                GetNeedPrefab(mapID, out Entity mapPrefab, out NativeHashMap<int, bool> borderPrefabLeft,
                    out NativeHashMap<int, bool> borderPrefabRight, out NativeHashMap<int, bool> borderPrefabTop,
                    out NativeHashMap<int, bool> borderPrefabBottom);
                //var borderR = GetRandomEntity(borderPrefabRight);
                //var borderL = GetRandomEntity(borderPrefabLeft);

                var lastBottomPos = new float3(-mapWidth / 2f, limitBottom, 0);
                var startPos = lastBottomPos;

                //拼map 设置网格起始位置
                float needCount = mapHeight / mapWidth;
                for (int i = 0; i < needCount; i++)
                {
                    //调整地图预制体大小
                    AjustMap(mapPrefab, cdfeLocalTransform);

                    Entity newMapEntity = ecbs.Instantiate(mapPrefab);
                    int nextMapIndex = mapIndex - 1;
                    ecbs.SetComponent<MapBaseData>(newMapEntity, new MapBaseData { mapIndex = nextMapIndex });

                    var startTempPos = new float3(startPos.x, startPos.y - i * mapWidth, 0);
                    var endTempPos = new float3(startTempPos.x, startTempPos.y - mapWidth, 0);

                    var trans = cdfeLocalTransform[mapPrefab];
                    var position = new float3(0, (startTempPos.y + endTempPos.y) / 2f, 0);
                    //var loc = cdfeLocalTransform[newMapEntity];
                    //loc.Scale = trans.Scale;
                    //loc.Position= position;
                    //loc.Rotation = trans.Rotation;
                    //cdfeLocalTransform[newMapEntity] = loc;
                    ecbs.SetComponent<LocalTransform>(newMapEntity,
                        new LocalTransform { Position = position, Scale = trans.Scale, Rotation = trans.Rotation });
                    int index = i == 0 ? 1 : 0;
                    InstanceBorderSeq(trans, mapWidth, position, borderPrefabLeft, borderPrefabRight, index, mapID);
                    mapRefreshData.minIndex = nextMapIndex;
                    ecbs.SetComponent(wbe, mapRefreshData);
                }

                InitMapElements(startPos, mapIndex);

                mapRefreshData.minPosBottomLeft = startPos - mapHeight;
                ecbs.SetComponent(wbe, mapRefreshData);
            }

            private void InstanceBorderSeq(LocalTransform loc, float mapWidth, float3 position,
                NativeHashMap<int, bool> borderPrefabLeft, NativeHashMap<int, bool> borderPrefabRight, int order,
                int mapID)
            {
                GetNeedPrefab(mapID, out Entity mapPrefab, out NativeList<Entity> listLeft,
                    out NativeList<Entity> listRight, out NativeList<Entity> listBottom,
                    out NativeList<Entity> listTop);

                if (borderPrefabLeft.Count <= 1 || borderPrefabRight.Count <= 1)
                {
                    order = 0;
                    Debug.LogError("！！！！！！！！！！！！！！！！！！！配表错误！！！！！！！");
                }


                var prefabL = listLeft[order];
                var prefabR = listRight[order];
                var borderLeft = ecbs.Instantiate(prefabL);
                var borderRight = ecbs.Instantiate(prefabR);
                if (borderPrefabLeft[order])
                {
                    ecbs.AddComponent(borderLeft, new JiYuFlip { value = { x = 1, y = 0 } });
                    ecbs.SetComponent(borderLeft, new BoardData
                    {
                        type = BoardTypeEnum.Left
                    });
                }

                if (borderPrefabRight[order])
                {
                    ecbs.AddComponent(borderRight, new JiYuFlip { value = { x = 1, y = 0 } });
                    ecbs.SetComponent(borderRight, new BoardData
                    {
                        type = BoardTypeEnum.Right
                    });
                }

                AddDataToBorder(borderLeft);


                AddDataToBorder(borderRight);

                float xPos = mapWidth / 2f + loc.Scale * 5.12f / 2;
                var positionL = new float3(-xPos, position.y, 0);
                var positionR = new float3(xPos, position.y, 0);
                ecbs.SetComponent<LocalTransform>(borderLeft,
                    new LocalTransform { Position = positionL, Scale = loc.Scale, Rotation = loc.Rotation });
                ecbs.SetComponent<LocalTransform>(borderRight,
                    new LocalTransform { Position = positionR, Scale = loc.Scale, Rotation = loc.Rotation });
            }


            private void AjustMapScaleAndPos(Entity entity, ComponentLookup<LocalTransform> cdfeLocalWorld,
                int borderPos)
            {
                float mapheight = gameOthersData.mapData.mapSize.y;
                float mapWidth = gameOthersData.mapData.mapSize.x;
                float scale = mapWidth / 20.48f;
                var temp = new LocalTransform { Scale = scale };

                float xPos = -mapWidth / 2f - scale * 5.12f / 2;
                float yPos = mapheight / 2f + scale * 5.12f / 2;

                if (borderPos == 0)
                {
                    temp.Position = new float3(0, 0, 0);
                }
                //右
                else if (borderPos == 1)
                {
                    temp.Position = new float3(-xPos, 0, 0);
                }
                //左
                else if (borderPos == -1)
                {
                    temp.Position = new float3(xPos, 0, 0);
                }
                else if (borderPos == 2)
                {
                    temp.Position = new float3(0, -yPos, 0);
                }
                else
                {
                    temp.Position = new float3(0, yPos, 0);
                }

                ecbs.SetComponent<LocalTransform>(entity, temp);
            }

            private void AjustEntityScaleAndPos(Entity entity,
                int posType, float ajusthPar)
            {
                float mapheight = gameOthersData.mapData.mapSize.y;
                float mapWidth = gameOthersData.mapData.mapSize.x;
                var temp = new LocalTransform { Scale = mapWidth / 10.24f };


                if (posType == 1)
                {
                    temp.Position = new float3(mapWidth / 2f + temp.Scale * 5.12f / 2,
                        mapheight * (ajusthPar - 1), 0);
                }
                else if (posType == -1)
                {
                    temp.Position = new float3(-(mapWidth / 2f + temp.Scale * 5.12f / 2),
                        mapheight * (ajusthPar - 1), 0);
                }
                else
                {
                    temp.Position = new float3(0, mapheight * (ajusthPar - 1), 0);
                }

                ecbs.SetComponent<LocalTransform>(entity, temp);
            }

            private void AjustMap(Entity mapPrefab,
                ComponentLookup<LocalTransform> cdfeLocalTransform)
            {
                float mapWidth = gameOthersData.mapData.mapSize.x;
                var temp = cdfeLocalTransform[mapPrefab];
                temp.Scale = mapWidth / 10.24f;
                cdfeLocalTransform[mapPrefab] = temp;
                //var temp = new LocalTransform { Scale = mapWidth / 10.24f };
                //ecbs.SetComponent<LocalTransform>(mapPrefab, temp);
                //ecbs.SetComponent<LocalTransform>(prefabLeft, temp);
                //ecbs.SetComponent<LocalTransform>(prefabRight, temp);
            }

            #endregion

            #region 生成障碍物和地形

            private void InitMapElements(float3 startPos, int mapIndex)
            {
                int sceneId = gameOthersData.sceneId;
                ref var scene = ref globalConfigData.value.Value.configTbscenes.configTbscenes;
                ref var moduleTemplate =
                    ref globalConfigData.value.Value.configTbmodule_templates.configTbmodule_templates;
                ref var sceneModule = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;

                int moduleTemplateID;
                GetSceneModels(out NativeList<int> moduleIds, out NativeList<int> refreshIds,
                    out NativeList<Entity> modules, out moduleTemplateID, mapIndex);
                for (int i = 0; i < moduleIds.Length; i++)
                {
                    Debug.Log($"moduleId:{moduleIds[i]}");
                }

                for (int i = 0; i < refreshIds.Length; i++)
                {
                    Debug.Log($"refreshID:{refreshIds[i]}");
                }

                int numCount = 0;
                int maxRectCount = 0;
                ref var moduleRefreshConfig =
                    ref globalConfigData.value.Value.configTbmodule_refreshs.configTbmodule_refreshs;
                for (int i = 0; i < refreshIds.Length; i++)
                {
                    for (int j = 0; j < moduleRefreshConfig.Length; j++)
                    {
                        if (moduleRefreshConfig[j].id == refreshIds[i])
                        {
                            if (moduleRefreshConfig[j].selfYn == 0)
                            {
                                maxRectCount += moduleRefreshConfig[j].num;
                            }
                        }
                    }
                }

                maxRectCount = maxRectCount > 0 ? maxRectCount : MaxSelfRect;
                //Debug.LogError($"maxRectCount:{maxRectCount}");
                NativeList<Rect> outRects = new NativeList<Rect>(maxRectCount, Allocator.Temp);
                for (int i = 0; i < moduleTemplate.Length; i++)
                {
                    if (moduleTemplate[i].id == moduleTemplateID)
                    {
                        numCount += moduleTemplate[i].num;
                        int startIndex = numCount - moduleTemplate[i].num;
                        if (moduleTemplate[i].time == 0)
                        {
                            for (int j = startIndex; j < numCount; j++)
                            {
                                NativeList<float2> points = SelectPoints(startPos, ref outRects, false, refreshIds[j],
                                    (int)(numCount + mapIndex * 10 + gameRandomData.seed));
                                Debug.Log($"id:{moduleIds[j]}");
                                InitModuleEntities(cdfeLocalTransform, points, modules[j], moduleIds[j]);
                            }
                        }
                    }
                }
            }

            private void InitModuleEntities(ComponentLookup<LocalTransform> cdfeLocalTransform,
                NativeList<float2> points, Entity prefab, int moduleId)
            {
                ref var sceneModule = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
                //ref var paraPos = ref configTbmodule_template.paraPos;

                int num = points.Length;
                float scaleMulti = 1;
                for (int i = 0; i < num; i++)
                {
                    var ins = ecbs.Instantiate(prefab);

                    var newloc = cdfeLocalTransform[prefab];
                    //LocalTransform newloc = default;
                    newloc.Position = new float3(points[i].x, points[i].y, 0);


                    int index = 0;
                    for (int j = 0; j < sceneModule.Length; j++)
                    {
                        if (sceneModule[j].id == moduleId)
                        {
                            index = j;
                            break;
                        }
                    }

                    var scale = math.max(sceneModule[index].size[0] / 1000f, sceneModule[index].size[1] / 1000f);


                    if (cdfePostTransformMatrix.HasComponent(prefab))
                    {
                        var postTransformMatrix = cdfePostTransformMatrix[prefab];
                        var scaleprefab = postTransformMatrix.Value.DecomposeScale();
                        scaleMulti *= scaleprefab.x;

                        ecbs.SetComponent(ins, new PostTransformMatrix
                        {
                            Value = float4x4.Scale(scale * scaleMulti, scale * scaleMulti, 1)
                        });
                    }
                    else
                    {
                        scale *= scaleMulti;
                    }


                    FixedString32Bytes flipStr = $"_flip";

                    var type = sceneModule[index].type;
                    newloc.Scale = scale;
                    var elementData = new MapElementData { elementID = moduleId };
                    ecbs.SetComponent(ins, elementData);
                    //Debug.LogError($"dfdfffffffffffffffffffffscale:{scale}");
                    if (sceneModule[index].model.Contains(flipStr))
                    {
                        Debug.Log("Contains");
                        newloc.Rotation = quaternion.AxisAngle(math.up(), math.radians(180));
                    }

                    //Debug.LogError($"entity:{ins.Index},pos:{newloc.Position}");


                    if (type == 2)
                    {
                        newloc.Scale = 0.001f;
                        //newloc.Scale *= 10;
                        //ecbs.SetComponent(ins, newloc);
                        ecbs.SetComponent(ins, new PushColliderData
                        {
                            targetScale = scale
                        });
                    }

                    ecbs.SetComponent(ins, newloc);

                    AddDataToElement(ins, moduleId, prefab);
                }
            }

            public void InitEntities(NativeList<float2> positions, NativeHashMap<Entity, int> outEntitiesInfo, int type,
                ref ConfigTbmodule_template configTbrefresh, int mapIndex)
            {
                //debug.log($"positions:{positions.length},entities:{outentitiesinfo.count}");
                //if (outentitiesinfo.count <= 0) return;
                //if (positions.length <= 0) return;

                //bool isrerefresh = configtbrefresh.againyn == 1 ? true : false;
                //int num = configtbrefresh.num;
                //if (positions.length <= num)
                //{
                //    num = positions.length;
                //}

                //ref var relativepos = ref configtbrefresh.refreshelementset;

                //var resultentities = collectionhelper.createnativearray<entity>(num, allocator.temp);
                //var outentities = outentitiesinfo.getkeyarray(allocator.temp);

                //ref var mapmodulesarray = ref globalconfigdata.value.value.configtbscene_modules.configtbscene_modules;
                //int size = 0, moudletype = 0, hp = 0;

                //var current = outEntities[0];
                //for (int i = 0; i < mapModulesArray.Length; i++)
                //{
                //    if (mapModulesArray[i].id == outEntitiesInfo[current])
                //    {
                //        size = mapModulesArray[i].size[0] > mapModulesArray[i].size[1]
                //            ? mapModulesArray[i].size[0] / 1000
                //            : mapModulesArray[i].size[1] / 1000;
                //        moudletype = mapModulesArray[i].id / 1000 == 1 ? 1 : 2;
                //        hp = mapModulesArray[i].hp;
                //        //Debug.Log($"refreshType:{type}, id:{outEntitiesInfo[current]},size{size}");
                //        break;
                //    }
                //}

                //for (int i = 0; i < num; i++)
                //{
                //    resultEntities[i] = ecbs.Instantiate(current);
                //}

                //for (int i = 0; i < num; ++i)
                //{
                //    float3 position = new float3(positions[i].x, positions[i].y, 0);
                //    ecbs.SetComponent(resultEntities[i],
                //        new LocalTransform { Scale = size, Position = position });
                //}
            }

            private void GetSceneModels(out NativeList<int> moduleIds, out NativeList<int> refreshIds,
                out NativeList<Entity> modules,
                out int templateId, int mapIndex)
            {
                int sceneId = gameOthersData.sceneId;
                ref var sceneConfig = ref globalConfigData.value.Value.configTbscenes.configTbscenes;
                ref var moduleTemplateConfig =
                    ref globalConfigData.value.Value.configTbmodule_templates.configTbmodule_templates;
                ref var sceneModuleConfig =
                    ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
                ref var moduleRefreshConfig =
                    ref globalConfigData.value.Value.configTbmodule_refreshs.configTbmodule_refreshs;
                //ref var moduleGroup = ref globalConfigData.value.Value.configTbmodule_groups.configTbmodule_groups;


                modules = new NativeList<Entity>(Allocator.Temp);
                refreshIds = new NativeList<int>(Allocator.Temp);
                moduleIds = new NativeList<int>(Allocator.Temp);

                int sceneIndex = 0;
                for (int i = 0; i < sceneConfig.Length; i++)
                {
                    if (sceneConfig[i].id == sceneId)
                    {
                        sceneIndex = i;
                        break;
                    }
                }

                templateId = sceneConfig[sceneIndex].moduleTemplateId;
                ref var moduleTemplatePara = ref sceneConfig[sceneIndex].moduleTemplatePara;
                for (int i = 0; i < moduleTemplateConfig.Length; i++)
                {
                    if (moduleTemplateConfig[i].id == templateId)
                    {
                        var moduleGroupID = moduleTemplatePara[moduleTemplateConfig[i].ruleId - 1];
                        var tmpIDs = new NativeList<int>(Allocator.Temp);
                        var tmpRefreshIDs = new NativeList<int>(Allocator.Temp);
                        for (int j = 0; j < moduleRefreshConfig.Length; j++)
                        {
                            if (moduleRefreshConfig[j].group == moduleGroupID)
                            {
                                for (int k = 0; k < moduleRefreshConfig[j].power; k++)
                                {
                                    tmpIDs.Add(moduleRefreshConfig[j].sceneModule);
                                    tmpRefreshIDs.Add(moduleRefreshConfig[j].id);
                                }
                            }
                        }

                        int m = 0, times = 0;
                        do
                        {
                            times++;
                            var seed = (uint)(i * 100 + m + times * 10 + timeTick + mapIndex);
                            var rand = Unity.Mathematics.Random.CreateFromIndex(
                                seed);

                            Debug.Log($"seed:{seed},i:{i},m:{m},times:{times},tick:{timeTick}");

                            int randomNum = rand.NextInt(0, tmpIDs.Length);

                            if (!refreshIds.Contains(tmpRefreshIDs[randomNum]))
                            {
                                refreshIds.Add(tmpRefreshIDs[randomNum]);
                                //Debug.LogError($"refreshIds:{tmpRefreshIDs[randomNum]}");
                                moduleIds.Add(tmpIDs[randomNum]);
                                for (int k = 0; k < sceneModuleConfig.Length; k++)
                                {
                                    if (sceneModuleConfig[k].id == tmpIDs[randomNum])
                                    {
                                        FixedString32Bytes flipStr = $"_flip";
                                        FixedString128Bytes pic = sceneModuleConfig[k].model;
                                        if (sceneModuleConfig[k].model.Contains(flipStr))
                                        {
                                            int index = sceneModuleConfig[k].model.IndexOf(flipStr);
                                            pic = sceneModuleConfig[k].model.Substring(0, index);
                                        }

                                        var prefab = prefabMapData.prefabHashMap[pic];

                                        modules.Add(prefab);
                                        //Debug.LogError($"pic name:{pic}");
                                        break;
                                    }
                                }

                                m++;
                                times = 0;
                            }
                        } while (m < moduleTemplateConfig[i].num && times < 1000);
                    }
                }
            }


            private void GetSceneModelsByTime(out NativeList<int> moduleIds, out NativeList<int> refreshIds,
                out NativeList<Entity> modules,
                out int templateId, int time)
            {
                int sceneId = gameOthersData.sceneId;
                ref var sceneConfig = ref globalConfigData.value.Value.configTbscenes.configTbscenes;
                ref var moduleTemplateConfig =
                    ref globalConfigData.value.Value.configTbmodule_templates.configTbmodule_templates;
                ref var sceneModuleConfig =
                    ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
                ref var moduleRefreshConfig =
                    ref globalConfigData.value.Value.configTbmodule_refreshs.configTbmodule_refreshs;
                //ref var moduleGroup = ref globalConfigData.value.Value.configTbmodule_groups.configTbmodule_groups;


                modules = new NativeList<Entity>(Allocator.Temp);
                refreshIds = new NativeList<int>(Allocator.Temp);
                moduleIds = new NativeList<int>(Allocator.Temp);

                int sceneIndex = 0;
                for (int i = 0; i < sceneConfig.Length; i++)
                {
                    if (sceneConfig[i].id == sceneId)
                    {
                        sceneIndex = i;
                        break;
                    }
                }

                templateId = sceneConfig[sceneIndex].moduleTemplateId;
                ref var moduleTemplatePara = ref sceneConfig[sceneIndex].moduleTemplatePara;
                for (int i = 0; i < moduleTemplateConfig.Length; i++)
                {
                    if (moduleTemplateConfig[i].id == templateId && moduleTemplateConfig[i].time == time)
                    {
                        var moduleGroupID = moduleTemplatePara[moduleTemplateConfig[i].ruleId - 1];
                        var tmpIDs = new NativeList<int>(Allocator.Temp);
                        var tmpRefreshIDs = new NativeList<int>(Allocator.Temp);
                        for (int j = 0; j < moduleRefreshConfig.Length; j++)
                        {
                            if (moduleRefreshConfig[j].group == moduleGroupID)
                            {
                                for (int k = 0; k < moduleRefreshConfig[j].power; k++)
                                {
                                    tmpIDs.Add(moduleRefreshConfig[j].sceneModule);
                                    tmpRefreshIDs.Add(moduleRefreshConfig[j].id);
                                }
                            }
                        }

                        int m = 0, times = 0;
                        do
                        {
                            times++;
                            var seed = (uint)(i * 100 + m + times * 10 + timeTick);
                            var rand = Unity.Mathematics.Random.CreateFromIndex(
                                seed);

                            Debug.Log($"seed:{seed},i:{i},m:{m},times:{times},tick:{timeTick}");

                            int randomNum = rand.NextInt(0, tmpIDs.Length);

                            if (!refreshIds.Contains(tmpRefreshIDs[randomNum]))
                            {
                                refreshIds.Add(tmpRefreshIDs[randomNum]);
                                //Debug.LogError($"refreshIds:{tmpRefreshIDs[randomNum]}");
                                moduleIds.Add(tmpIDs[randomNum]);
                                for (int k = 0; k < sceneModuleConfig.Length; k++)
                                {
                                    if (sceneModuleConfig[k].id == tmpIDs[randomNum])
                                    {
                                        FixedString32Bytes flipStr = $"_flip";
                                        FixedString128Bytes pic = sceneModuleConfig[k].model;
                                        if (sceneModuleConfig[k].model.Contains(flipStr))
                                        {
                                            int index = sceneModuleConfig[k].model.IndexOf(flipStr);
                                            pic = sceneModuleConfig[k].model.Substring(0, index);
                                        }

                                        var prefab = prefabMapData.prefabHashMap[pic];

                                        modules.Add(prefab);
                                        //Debug.LogError($"pic name:{pic}");
                                        break;
                                    }
                                }

                                m++;
                                times = 0;
                            }
                        } while (m < moduleTemplateConfig[i].num && times < 1000);
                    }
                }
            }


            private NativeList<float2> SelectPoints(float3 mapstartPos, ref NativeList<Rect> outRects, bool isNeedCheck,
                int refreshID, int numCount)
            {
                ref var sceneModuleConfig =
                    ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
                ref var moduleRefreshConfig =
                    ref globalConfigData.value.Value.configTbmodule_refreshs.configTbmodule_refreshs;
                var mapwidth = gameOthersData.mapData.mapSize.x;
                var mapHeight = gameOthersData.mapData.mapSize.y;
                int index = 0;
                for (int i = 0; i < moduleRefreshConfig.Length; i++)
                {
                    if (moduleRefreshConfig[i].id == refreshID)
                    {
                        index = i;
                        break;
                    }
                }

                ref var moduleRefresh = ref moduleRefreshConfig[index];
                var rationsStx = moduleRefresh.coordinate[0].x / 10000f;
                var rationsSty = moduleRefresh.coordinate[0].y / 10000f;
                var rationsEndx = moduleRefresh.coordinate[1].x / 10000f;
                var rationsEndy = moduleRefresh.coordinate[1].y / 10000f;
                int moduleID = moduleRefresh.sceneModule;
                float2 rectSize = default;
                for (int j = 0; j < sceneModuleConfig.Length; j++)
                {
                    if (sceneModuleConfig[j].id == moduleID)
                    {
                        rectSize = sceneModuleConfig[j].size.Length > 1
                            ? new float2(sceneModuleConfig[j].size[0] / 1000f, sceneModuleConfig[j].size[1] / 1000f)
                            : default;
                        break;
                    }
                }

                float2 startPos = new float2(rationsStx * mapwidth, rationsSty * mapHeight);
                float2 endPos = new float2(rationsEndx * mapwidth, rationsEndy * mapHeight);
                //坐标转换为实际坐标
                float2 startPosT = new float2(mapstartPos.x + startPos.x, mapstartPos.y - startPos.y);
                float2 endPosT = new float2(mapstartPos.x + endPos.x, mapstartPos.y - endPos.y);

                //Debug.LogError($"startT:{startPosT},endPost:{endPosT}");
                bool isRandGenerate = moduleRefresh.randType == 1 ? true : false;
                //生成数量
                int count = moduleRefresh.num;
                NativeList<float2> Positionlist = new NativeList<float2>(count, Allocator.Temp);

                int gap = moduleRefresh.pointRange / 1000;
                if (isRandGenerate)
                {
                    for (int k = 0; k < count; ++k)
                    {
                        uint seed = (uint)(k + timeTick + refreshID * 10 + numCount * 100);
                        //Debug.LogError($"seed:{seed}");
                        var random = Unity.Mathematics.Random.CreateFromIndex(seed);
                        Rect itemRect;
                        //float2 randPos=float2.zero;
                        int times = 0;
                        do
                        {
                            var randPos = random.NextFloat2(new float2(startPosT.x, startPosT.y),
                                new float2(endPosT.x, endPosT.y));

                            if (float.IsNaN(randPos.x) || float.IsNaN(randPos.y))
                            {
                                randPos = float2.zero;
                            }

                            times++;


                            itemRect = new Rect(randPos.x - rectSize.x / 2f - gap, randPos.y - gap - rectSize.y / 2f,
                                rectSize.x + gap * 2, rectSize.y + gap * 2);

                            if (IsMaxLimitLoop(times))
                            {
                                Debug.LogError(
                                    $"生成规则有误 选点失败,已选点个数{k}个,未选点个数{count - k}个,错误的刷新组id为{moduleRefresh.id},错误的组件id为{moduleRefresh.sceneModule}");
                                itemRect.x = 9999999f;
                                itemRect.y = -9999999f;
                                //Positionlist.Add(new float2(9999999f,));
                                break;
                            }
                        } while (ItemOverlap(itemRect, outRects));

                        outRects.Add(itemRect);
                        Positionlist.Add(new float2(itemRect.x + rectSize.x / 2f + gap,
                            itemRect.y - gap - rectSize.y / 2f));
                    }
                }
                else
                {
                    Positionlist = SelectPointsRandomSequential(ref outRects, rectSize, startPosT, endPosT, count, gap);
                }

                //不独占就清除
                if (moduleRefresh.selfYn == 1 ? true : false)
                {
                    outRects.Clear();
                }


                return Positionlist;
            }

            private NativeList<float2> SelectPointsRandomSequential(ref NativeList<Rect> outRects,
                float2 size, float2 startPosT, float2 endPosT, int count, int gap)
            {
                NativeList<float2> result = new NativeList<float2>(count, Allocator.Temp);
                //float2 targetPos = new float2(startPosT.x, startPosT.y);

                int i = 0;
                uint seed = (uint)(timeTick + (uint)startPosT.y);
                //Debug.LogError($"seec:{seed}");
                var random = Unity.Mathematics.Random.CreateFromIndex(seed);
                var targetPos =
                    random.NextFloat2(new float2(startPosT.x, startPosT.y), new float2(endPosT.x, endPosT.y));
                while (i < count)
                {
                    if (result.Length != 0)
                    {
                        targetPos = result[^1];
                    }

                    var judgePos = targetPos.x + 2 * gap + size.x * 0.5f + size.x * 0.5f;
                    //Debug.LogError($"judgePos:{judgePos}");
                    if (judgePos > endPosT.x)
                    {
                        //Debug.LogError($"换行:{endPosT.x}");
                        targetPos = new float2(startPosT.x, targetPos.y - 2 * gap - size.y);
                    }
                    else
                    {
                        targetPos = new float2(judgePos, targetPos.y);
                    }

                    var rect = new Rect(targetPos.x - gap, targetPos.y - gap,
                        size.x + gap * 2,
                        size.y + gap * 2);
                    outRects.Add(rect);
                    result.Add(targetPos);
                    ++i;
                }

                return result;
            }

            private void GenerateMapModuleByTime(int currentMapIndex, int mapType)
            {
                //TODO:这两个值跟地图的宽高有关


                ref var moduleTemplate =
                    ref globalConfigData.value.Value.configTbmodule_templates.configTbmodule_templates;
                ref var sceneModule = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
                ref var scene = ref globalConfigData.value.Value.configTbscenes.configTbscenes;


                GetTemplateID(out int moduleTemplateID);
                int numCount = 0, maxRectCount = 0;
                for (int i = 0; i < moduleTemplate.Length; i++)
                {
                    if (moduleTemplate[i].id == moduleTemplateID)
                    {
                        numCount += moduleTemplate[i].num;

                        int currentTick = (int)(moduleTemplate[i].time / dT);
                        if (moduleTemplate[i].time != 0 && currentTick == gameTimeTick)
                        {
                            GetSceneModels(out NativeList<int> moduleIds, out NativeList<int> refreshIds,
                                out NativeList<Entity> modules, out moduleTemplateID, currentMapIndex);
                            for (int j = 0; j < moduleIds.Length; j++)
                            {
                                // Debug.LogError($"moduleIdByTime:{moduleIds[j]}");
                            }

                            for (int j = 0; j < refreshIds.Length; j++)
                            {
                                //Debug.LogError($"refreshIDByTime:{refreshIds[j]}");
                            }

                            int startIndex = numCount - moduleTemplate[i].num;
                            float3 startPos = float3.zero;
                            float3 mapCenterPos = float3.zero;
                            var inBoss = cdfePlayerData[player].playerOtherData.isBossFight;
                            float mapWidth, mapheight;
                            if (inBoss)
                            {
                                mapWidth = mapheight = cdfePlayerData[player].playerOtherData.bossScenePos.y;
                                switch (mapType)
                                {
                                    case 1:
                                        mapCenterPos = new float3(1999, 0, 0);
                                        break;
                                    case 2:
                                        mapCenterPos = new float3(0, 1999, 0);
                                        break;
                                    case 3:
                                        mapCenterPos = new float3(1999, 0, 0);

                                        break;
                                    case 4:
                                        //TODO:全开放的boss场景选点
                                        mapCenterPos = new float3(1999, 1999, 0);
                                        break;
                                }
                            }
                            else
                            {
                                mapWidth = gameOthersData.mapData.mapSize.x;
                                mapheight = gameOthersData.mapData.mapSize.y;
                            }


                            ref var moduleRefreshConfig = ref globalConfigData.value.Value.configTbmodule_refreshs
                                .configTbmodule_refreshs;
                            for (int k = 0; k < refreshIds.Length; k++)
                            {
                                for (int j = 0; j < moduleRefreshConfig.Length; j++)
                                {
                                    if (moduleRefreshConfig[j].id == refreshIds[k])
                                    {
                                        if (moduleRefreshConfig[j].selfYn == 0)
                                        {
                                            maxRectCount += moduleRefreshConfig[j].num;
                                        }
                                    }
                                }
                            }

                            maxRectCount = maxRectCount > 0 ? maxRectCount : MaxSelfRect;
                            //Debug.LogError($"maxRectCount:{maxRectCount}");
                            NativeList<Rect> outRects = new NativeList<Rect>(maxRectCount, Allocator.Temp);
                            for (int j = startIndex; j < numCount; j++)
                            {
                                var refreshID = refreshIds[j];
                                int index = 0;
                                for (int k = 0; k < moduleRefreshConfig.Length; k++)
                                {
                                    if (moduleRefreshConfig[k].id == refreshID)
                                    {
                                        index = k;
                                        break;
                                    }
                                }

                                int num = moduleRefreshConfig[index].num;
                                NativeList<float2> points = new NativeList<float2>(num, Allocator.Temp);
                                //以地图坐标
                                if (moduleRefreshConfig[index].type == 1)
                                {
                                    if (inBoss)
                                    {
                                        startPos = new float3(mapCenterPos.x - mapWidth / 2f,
                                            mapCenterPos.y + mapheight / 2f, 0);
                                    }
                                    else
                                    {
                                        switch (mapType)
                                        {
                                            case 1:
                                                startPos = new float3(-mapWidth / 2f,
                                                    -(mapWidth * 3) + mapheight * currentMapIndex, 0);
                                                break;
                                            case 3:
                                                startPos = new float3(-mapWidth / 2f, mapheight / 2f, 0);
                                                break;
                                            case 4:
                                                //没想好
                                                break;
                                        }
                                    }

                                    points = SelectPoints(startPos, ref outRects, false, refreshID,
                                        (int)(j + currentMapIndex * 10 + dT * 1000 + gameRandomData.seed));
                                }
                                //以玩家坐标
                                else if (moduleRefreshConfig[index].type == 2)
                                {
                                    switch (mapType)
                                    {
                                        case 1:
                                            var temp = new float3(mapCenterPos.x, cdfeLocalTransform[player].Position.y,
                                                0);
                                            startPos = new float3(temp.x - mapWidth / 2f, temp.y + mapheight / 2f, 0);
                                            break;
                                        case 3:

                                            startPos = float3.zero;
                                            break;
                                        case 4:

                                            startPos = cdfeLocalTransform[player].Position;
                                            break;
                                        default:
                                            break;
                                    }

                                    points = SelectPoints(startPos, ref outRects, false, refreshID,
                                        (int)(j + currentMapIndex * 10 + dT * 1000 + gameRandomData.seed));
                                    Debug.LogError($"GenerateMapModuleByTime{startPos}");
                                }
                                //在视野外
                                else if (moduleRefreshConfig[index].type == 3)
                                {
                                    var rand = Unity.Mathematics.Random.CreateFromIndex(timeTick);
                                    int maxTimes = 0;
                                    var width = mapWidth / 120f * 100;
                                    var height = width * 3;
                                    var playerLoc = cdfeLocalTransform[player].Position;
                                    var smallRect = new Rect(playerLoc.x, playerLoc.y, width, height);
                                    var bigRect = new Rect(playerLoc.x, playerLoc.y, width + 25, height + 25);
                                    float3 point = default;

                                    while (num > 0)
                                    {
                                        do
                                        {
                                            point.x = rand.NextFloat(bigRect.center.x - bigRect.width / 2f,
                                                bigRect.center.x + bigRect.width / 2f);
                                            point.y = rand.NextFloat(bigRect.center.y - bigRect.height / 2f,
                                                bigRect.center.y + bigRect.height / 2f);
                                            maxTimes++;
                                            if (maxTimes > 1000)
                                            {
                                                Debug.LogError("选点失败 无法选到视野外");
                                                return;
                                            }
                                        } while (!BuffHelper.IsPosCanUse(point, globalConfigData, elements,
                                                     cdfeMapElementData, cdfeLocalTransform) &&
                                                 IsPosInside(smallRect, bigRect, point));

                                        points.Add(point.xy);
                                        num--;
                                    }
                                }

                                InitModuleEntities(cdfeLocalTransform, points, modules[j], moduleIds[j]);
                            }
                        }
                    }
                }
            }


            private bool IsPosInside(Rect smallRect, Rect bigRect, float3 point)
            {
                if (!smallRect.Contains(point) && bigRect.Contains(point))
                {
                    return true;
                }

                return false;
            }

            private void GetTemplateID(out int moduleTemplateID)
            {
                int sceneId = gameOthersData.sceneId;
                ref var scene = ref globalConfigData.value.Value.configTbscenes.configTbscenes;
                int sceneIndex = 0;
                for (int i = 0; i < scene.Length; i++)
                {
                    if (scene[i].id == sceneId)
                    {
                        sceneIndex = i;
                        break;
                    }
                }

                ref var sceneRow = ref scene[sceneIndex];
                moduleTemplateID = sceneRow.moduleTemplateId;
            }


            public void AddDataToElement(Entity entity, int mapModuleID, Entity prefab)
            {
                //Debug.LogError("AddDataToElement");

                ref var elementArray = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
                ref var elementConfig =
                    ref globalConfigData.value.Value.configTbelement_effects.configTbelement_effects;
                int index = -1;

                ref var eventsTable = ref globalConfigData.value.Value.configTbevent_0s.configTbevent_0s;
                for (int i = 0; i < elementArray.Length; i++)
                {
                    if (mapModuleID == elementArray[i].id)
                    {
                        index = i;
                        break;

                        //TODO：Add SKill
                        // ref var events = ref elementArray[i].event0;
                        // if (events.Length <= 0) break;
                        // ecbs.AddBuffer<GameEvent>(entity);
                        // for (int j = 0; j < events.Length; j++)
                        // {
                        //     int eventID = events[j];
                        //     //AddEventTo(eventID, entity, globalConfigData);
                        // }
                    }
                }

                if (index < 0) return;
                ref var mapModule = ref elementArray[index];
                var hp = mapModule.hp;
                var type = mapModule.type;
                var pathPriority = mapModule.pathPriority;
                var width = mapModule.size[0] / 1000;
                var display_order = mapModule.displayOrder;
                var restitutionRatios = mapModule.impactSpeedRatio;
                restitutionRatios = math.clamp(restitutionRatios, 0, 10000);
                var elementId = mapModule.elementId;
                var mapModuleDuration = mapModule.duration;

                if (type == 2)
                {
                    //TODO:
                    var precol = cdfePhysicsCollider[prefab];

                    unsafe
                    {
                        var colliderPtr = precol.ColliderPtr;
                        colliderPtr->SetRestitution(restitutionRatios / 10000f);
                    }

                    ecbs.SetComponent(entity, precol);
                    //给碰撞体增加ChaStats组件 和可承受伤害组件
                    ecbs.AddComponent<ObstacleTag>(entity);

                    var chaStats = new ChaStats() { };
                    if (hp > 0)
                    {
                        if (mapModule.battleDrop.Length > 0)
                        {
                            ecbs.AddBuffer<DropsBuffer>(entity);
                            for (int i = 0; i < mapModule.battleDrop.Length; i++)
                            {
                                var drop = mapModule.battleDrop[i];
                                ecbs.AppendToBuffer(entity, new DropsBuffer
                                {
                                    id = drop
                                });
                            }
                        }
                        
                        chaStats.chaProperty.maxHp = hp;
                        chaStats.chaResource.hp = chaStats.chaProperty.maxHp;
                        //ecbs.AddComponent(entity,chaStats);
                        //ecbs.AddBuffer<DamageInfo>(entity);
                        ecbs.AddBuffer<Buff>(entity);
                    }

                    chaStats.chaProperty.mass = mapModule.mass;
                    ecbs.AddComponent(entity, chaStats);
                    ecbs.AddComponent(entity, new AgentShape
                    {
                        Radius = width / 2f,
                        Height = 0,
                        Type = ShapeType.Circle
                    });
                    ecbs.AddComponent(entity, new Agent
                    {
                        Layers = NavigationLayers.Default
                    });
                    ecbs.AddComponent(entity, new AgentBody
                    {
                        Force = default,
                        Velocity = default,
                        Destination = default,
                        RemainingDistance = 0,
                        IsStopped = true
                    });
                    if (elementId != 0)
                    {
                        ecbs.AddComponent(entity, new ElementData
                        {
                            type = type,
                            id = elementId
                        });
                    }
                }
                else if (type == 1)
                {
                    ecbs.SetComponent(entity,
                        new TargetData
                        {
                            BelongsTo = (uint)BuffHelper.TargetEnum.Area,
                            AttackWith = 1 + 2 + 4 + 8 + 16 + 32 + 128
                        });
                    //ecbs.AddComponent<AreaTag>(entity);
                    ecbs.SetComponent(entity,
                        new JiYuSort { value = new int2((int)JiYuLayer.Area, math.clamp(display_order, 0, 10)) });
                    if (pathPriority >= 25)
                    {
                        ecbs.AddComponent(entity, new AgentShape
                        {
                            Radius = width / 2f,
                            Height = 0,
                            Type = ShapeType.Circle
                        });
                        ecbs.AddComponent(entity, new Agent
                        {
                            Layers = NavigationLayers.Default
                        });
                        ecbs.AddComponent(entity, new AgentBody
                        {
                            Force = default,
                            Velocity = default,
                            Destination = default,
                            RemainingDistance = 0,
                            IsStopped = true
                        });
                    }

                    if (elementId != 0)
                    {
                        ecbs.AddComponent(entity, new ElementData
                        {
                            type = 0,
                            id = elementId
                        });
                    }

                    //给火地形加临时生命值
                    if (elementId == 201)
                    {
                        int maxHp = 0;
                        for (int i = 0; i < elementConfig.Length; i++)
                        {
                            if (elementConfig[i].from == 201 && elementConfig[i].target == 102)
                            {
                                maxHp = elementConfig[i].para[0];
                                break;
                            }
                        }

                        ecbs.AddComponent(entity, new ChaStats
                        {
                            chaProperty = new ChaProperty
                            {
                                maxHp = maxHp,
                                hpRatios = 0,
                                hpAdd = 0,
                                hpRecovery = 0,
                                hpRecoveryRatios = 0,
                                hpRecoveryAdd = 0,
                                atk = 0,
                                atkRatios = 0,
                                atkAdd = 0,
                                rebirthCount = 0,
                                critical = 0,
                                criticalDamageRatios = 0,
                                damageRatios = 0,
                                damageAdd = 0,
                                reduceHurtRatios = 0,
                                reduceHurtAdd = 0,
                                maxMoveSpeed = 0,
                                maxMoveSpeedRatios = 0,
                                speedRecoveryTime = 0,
                                mass = 0,
                                massRatios = 0,
                                pushForce = 0,
                                pushForceRatios = 0,
                                reduceHitBackRatios = 0,
                                dodge = 0,
                                shieldCount = 0,
                                defaultcoolDown = 0,
                            },
                            enviromentProperty = default,
                            chaResource = new ChaResource
                            {
                                curPushForce = 0,
                                curMoveSpeed = 0,
                                direction = default,
                                continuousCollCount = 0,
                                actionSpeed = 0,
                                hp = maxHp,
                                env = default
                            },
                            chaControlState = default
                        });
                        ecbs.AddBuffer<Buff>(entity);
                        ecbs.AddComponent(entity, new TimeToDieData { duration = mapModuleDuration / 1000f });
                    }

                    ecbs.AddComponent(entity, new SkillAttackData
                    {
                        data = new SkillAttack_9999
                        {
                            duration = 9999f,
                            enableExit = true,
                            isBullet = true,
                            isOnStayTrigger = true,
                            onStayTriggerCd = 0.5f,
                            hp = MathHelper.MaxNum
                        }.ToSkillAttack()
                    });
                }

                ref var skills = ref mapModule.skillGroup;
                if (skills.Length > 0)
                {
                    ecbs.AddBuffer<Skill>(entity);
                }

                for (int i = 0; i < skills.Length; i++)
                {
                    BuffHelper.AddSkillByEcbs(ref ecbs, skills[i], entity, 1);
                }


                //ecbs.AddComponent<ObstacleTag>(entity);
            }

            private void AddEventTo(int eventID, Entity entity, GlobalConfigData configData)
            {
                ref var eventsTable = ref configData.value.Value.configTbevent_0s.configTbevent_0s;
                int triggerType = 0, type = 0;
                for (int i = 0; i < eventsTable.Length; i++)
                {
                    if (eventsTable[i].id == eventID)
                    {
                        triggerType = eventsTable[i].triggerType;
                        type = eventsTable[i].type;
                    }
                }


                //switch (eventID)
                //{
                //    case 1001:
                //        ecbs.AppendToBuffer<GameEvent>(entity, new GameEvent_1001
                //        {
                //            id = eventID,
                //            triggerType = triggerType,
                //            eventType = type,
                //            triggerGap = 0,
                //            remainTime = 1f,
                //            duration = 0,
                //            isPermanent = true,
                //            target = default,
                //            onceTime = 0,
                //            colliderScale = 0,
                //            delayTime = 1f
                //        }.ToGameEvent());

                //        break;
                //    case 1002:
                //        ecbs.AppendToBuffer<GameEvent>(entity, new GameEvent_1002
                //        {
                //            id = eventID,
                //            triggerType = triggerType,
                //            eventType = type,
                //            triggerGap = 0,
                //            remainTime = 1f,
                //            duration = 0,
                //            isPermanent = true,
                //            target = default,
                //            onceTime = 0,
                //            colliderScale = 0,
                //            delayTime = 1f
                //        }.ToGameEvent());

                //        break;
                //    case 1004:
                //        //ecbs.AppendToBuffer<GameEvent>(entity, new GameEvent_1004
                //        //{
                //        //    id = eventID,
                //        //    triggerType = triggerType,
                //        //    eventType = type,
                //        //    triggerGap = 0,
                //        //    remainTime = 1f,
                //        //    duration = 0,
                //        //    isPermanent = true,
                //        //    target = default,
                //        //    onceTime = 0,
                //        //    colliderScale = 0,
                //        //    delayTime = 1f
                //        //}.ToGameEvent());

                //        break;
                //    case 1005:
                //        //ecbs.AppendToBuffer<GameEvent>(entity, new GameEvent_1005
                //        //{
                //        //    id = eventID,
                //        //    triggerType = triggerType,
                //        //    eventType = type,
                //        //    triggerGap = 0,
                //        //    remainTime = 1f,
                //        //    duration = 0,
                //        //    isPermanent = true,
                //        //    target = default,
                //        //    onceTime = 0,
                //        //    colliderScale = 0,
                //        //    delayTime = 1f
                //        //}.ToGameEvent());

                //        break;
                //    //case 1006:
                //    //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
                //    //    GameEvent.Add(new GameEvent_1001
                //    //    {

                //    //        id = 1006,

                //    //        isPermanent = true,
                //    //        remainTime = 1,
                //    //        target = default,

                //    //    }.ToGameEvent());
                //    //    break;
                //    case 1007:
                //        ecbs.AppendToBuffer<GameEvent>(entity, new GameEvent_1007
                //        {
                //            id = eventID,
                //            eventType = type,
                //            triggerType = triggerType,
                //            isPermanent = true,
                //            duration = 0f,
                //            remainTime = 1f,
                //            target = default,
                //            triggerGap = 0f,
                //        }.ToGameEvent());
                //        break;
                //    case 1008:
                //        ecbs.AppendToBuffer<GameEvent>(entity, new GameEvent_1008
                //        {
                //            id = eventID,
                //            eventType = type,
                //            triggerType = triggerType,
                //            isPermanent = true,
                //            duration = 0f,
                //            remainTime = 1f,
                //            target = default,
                //            triggerGap = 2f,
                //            delayTime = 0,
                //        }.ToGameEvent());

                //        break;
                //    //case 1009:
                //    //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
                //    //    GameEvent.Add(new GameEvent_1009
                //    //    {
                //    //        id = 1009,
                //    //        isPermanent = false,
                //    //        remainTime = 1,
                //    //        target = default,
                //    //    }.ToGameEvent());
                //    //    break;
                //    //case 1010:
                //    //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
                //    //    GameEvent.Add(new GameEvent_1010
                //    //    {
                //    //        id = 1010,
                //    //        isPermanent = true,
                //    //        remainTime = 1,
                //    //        target = default,
                //    //    }.ToGameEvent());
                //    //    break;
                //    //case 2002:
                //    //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
                //    //    GameEvent.Add(new GameEvent_2002
                //    //    {

                //    //        id = 2002,
                //    //        isPermanent = true,
                //    //        remainTime = 1,
                //    //        target = default,
                //    //    }.ToGameEvent());
                //    //    break;
                //    //case 2003:
                //    //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
                //    //    GameEvent.Add(new GameEvent_2003
                //    //    {

                //    //        id = 2003,

                //    //        isPermanent = true,
                //    //        remainTime = 1,
                //    //        target = default,

                //    //    }.ToGameEvent());
                //    //    break;
                //    //case 2004:
                //    //    GameEvent = state.EntityManager.GetBuffer<GameEvent>(entity);
                //    //    GameEvent.Add(new GameEvent_2004
                //    //    {

                //    //        id = 2004,
                //    //        isPermanent = true,
                //    //        remainTime = 1,
                //    //        target = default,
                //    //    }.ToGameEvent());
                //    //    break;
                //    case 2005:
                //        ecbs.AppendToBuffer<GameEvent>(entity, new GameEvent_2005
                //        {
                //            id = 2005,
                //            triggerType = triggerType,
                //            eventType = type,
                //            triggerGap = 0,
                //            remainTime = 0,
                //            duration = 0,
                //            isPermanent = true,
                //            target = default,
                //            onceTime = 0,
                //            colliderScale = 0,
                //            delayTime = 0
                //        }.ToGameEvent());

                //        break;
                //    default: break;
                //}
            }


            private bool ItemOverlap(Rect judgeRect, NativeList<Rect> itemRects1)
            {
                //位置不能与玩家位置重合
                //var playerRect = new Rect(-PlayerInitRect/2f,PlayerInitRect/2f, PlayerInitRect, PlayerInitRect);
                //if (judgeRect.Overlaps(playerRect))
                //{
                //    return true;
                //}

                // 检查物品是否与已经生成的物品重叠 之前的
                //foreach (Rect rect in itemRects2)
                //{
                //    if (judgeRect.Overlaps(rect))
                //    {
                //        return true;
                //    }
                //}
                // 检查物品是否与已经生成的物品重叠 每一行的
                foreach (Rect rect in itemRects1)
                {
                    if (judgeRect.Overlaps(rect))
                    {
                        return true;
                    }
                }

                return false;
            }

            private bool IsMaxLimitLoop(int times)
            {
                return times > MaxSelectPosTimes ? true : false;
            }

            #endregion
        }
    }
}