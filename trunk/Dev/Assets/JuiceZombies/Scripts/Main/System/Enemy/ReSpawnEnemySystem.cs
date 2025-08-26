// //---------------------------------------------------------------------
// // UnicornStudio
// // Author: jaco0012
// // Time: 2023-08-24 12:25:25
// //---------------------------------------------------------------------
//
//
// using ProjectDawn.Navigation;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Physics;
// using Unity.Physics.Systems;
// using Unity.Transforms;
// using UnityEngine;
// using Collider = Unity.Physics.Collider;
// using Random = Unity.Mathematics.Random;
// using Rectangle = Main.SpawnEnemySystem.Rectangle;
//
// namespace Main
// {
//     //刷新怪物系统
//     [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//     [UpdateAfter(typeof(SpawnEnemySystem))]
//     public partial struct ReSpawnEnemySystem : ISystem
//     {
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//           
//             state.RequireForUpdate<WorldBlackBoardTag>();
//             state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
//             state.RequireForUpdate<EnemyData>();
//             state.RequireForUpdate<PlayerData>();
//             state.RequireForUpdate<ObstacleTag>();
//             state.Enabled = false;
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
//
//             var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
//             var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
//             var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
//             var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
//             var gameCameraSizeData = SystemAPI.GetComponent<GameCameraSizeData>(wbe);
//             var gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe);
//
//
//             var singleton = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
//
//             var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();
//             var obstacleQuery = SystemAPI.QueryBuilder().WithAll<ObstacleTag, MapElementData>().Build();
//
//
//             new ReSpawnEnemyJob
//             {
//                 gameSetUpData = gameSetUpData,
//                 globalConfigData = globalConfigData,
//                 gameCameraSizeData = gameCameraSizeData,
//                 gameOthersData = gameOthersData,
//                 gameRandomData = gameRandomData,
//                 cdfeMapElementData = SystemAPI.GetComponentLookup<MapElementData>(true),
//                 cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(true),
//                 player = SystemAPI.GetSingletonEntity<PlayerData>(),
//                 wbe = wbe,
//                 ecb = ecb,
//                 obstacles = obstacleQuery.ToEntityArray(Allocator.TempJob)
//             }.ScheduleParallel();
//         }
//
//         [BurstCompile]
//         public partial struct ReSpawnEnemyJob : IJobEntity
//         {
//             [ReadOnly] public GameSetUpData gameSetUpData;
//             [ReadOnly] public GlobalConfigData globalConfigData;
//             [ReadOnly] public GameCameraSizeData gameCameraSizeData;
//             [ReadOnly] public GameOthersData gameOthersData;
//             [ReadOnly] public ComponentLookup<LocalTransform> cdfeLocalTransform;
//             public GameRandomData gameRandomData;
//
//             [ReadOnly] public ComponentLookup<MapElementData> cdfeMapElementData;
//             public Entity player;
//             public Entity wbe;
//             public EntityCommandBuffer.ParallelWriter ecb;
//             [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Entity> obstacles;
//
//             private void Execute([EntityIndexInQuery] int entityIndexInQuery, ref LocalTransform tran,
//                 ref ChaStats chaStats, ref AgentLocomotion AgentLocomotion, in EnemyData enemyData)
//             {
//                 var dis = math.length(cdfeLocalTransform[player].Position - tran.Position);
//
//
//                 var mapWidthHeight = GetMapWidthHeight(globalConfigData, gameOthersData);
//
//
//                 var minDis = math.sqrt(mapWidthHeight.x * mapWidthHeight.x + mapWidthHeight.y * mapWidthHeight.y);
//                 if (dis < minDis) return;
//
//                 var rand = gameRandomData.rand;
//
//                 var pos = GenerateRandomPoint(new Rectangle
//                     {
//                         center = cdfeLocalTransform[player].Position.xy,
//                         width = gameCameraSizeData.width + 15,
//                         height = gameCameraSizeData.height + 15
//                     }, new Rectangle
//                     {
//                         center = cdfeLocalTransform[player].Position.xy,
//                         width = gameCameraSizeData.width + 25,
//                         height = gameCameraSizeData.height + 25
//                     }, ref rand, mapWidthHeight, globalConfigData, obstacles,
//                     cdfeLocalTransform,
//                     cdfeMapElementData);
//                 tran.Position = new float3(pos, 0);
//
//
//                 chaStats.chaResource.hp = chaStats.chaProperty.maxHp;
//                 chaStats.chaResource.curMoveSpeed = 0;
//                 //AgentLocomotion.Speed = 0;
//
//                 gameRandomData.rand = rand;
//                 //Debug.Log($"{gameRandomData.rand.state}");
//                 ecb.SetComponent(entityIndexInQuery, wbe, gameRandomData);
//             }
//         }
//
//         private static float2 GetMapWidthHeight(GlobalConfigData globalConfigData, GameOthersData gameOthersData)
//         {
//             ref var refreshs =
//                 ref globalConfigData.value.Value.configTbmonster_templates.configTbmonster_templates;
//             ref var monsters = ref globalConfigData.value.Value.configTbmonsters.configTbmonsters;
//             ref var scene_module = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
//             ref var scene = ref globalConfigData.value.Value.configTbscenes.configTbscenes;
//
//
//             int mapType = 0;
//             //BlobArray<int> mapSize = new BlobArray<int>();
//
//             int mapWith = 0;
//             int mapHeight = 0;
//             for (int i = 0; i < scene.Length; i++)
//             {
//                 if (scene[i].id == gameOthersData.sceneId)
//                 {
//                     var mapId = scene[i].mapId;
//
//                     for (int j = 0; j < scene_module.Length; j++)
//                     {
//                         if (scene_module[j].id == mapId)
//                         {
//                             mapType = scene_module[j].type;
//                             mapWith = scene_module[j].size[0];
//                             mapHeight = scene_module[j].size[1];
//                             break;
//                         }
//                     }
//
//                     break;
//                 }
//             }
//
//             float2 mapWithHeight = default;
//             if (mapType == 1)
//             {
//                 mapWithHeight = new float2(mapWith, mapHeight);
//             }
//             else if (mapType == 2)
//             {
//                 mapWithHeight = new float2(mapWith, math.INFINITY);
//             }
//             else if (mapType == 3)
//             {
//                 mapWithHeight = new float2(math.INFINITY, math.INFINITY);
//             }
//
//             return mapWithHeight;
//         }
//
//         private static int2 GenerateRandomPoint(Rectangle smallRect, Rectangle bigRect, ref Random rand,
//             float2 mapWithHeight, GlobalConfigData globalConfigData, NativeArray<Entity> obstacles,
//             ComponentLookup<LocalTransform> cdfeLocalTransform, ComponentLookup<MapElementData> cdfeMapElementData)
//         {
//             ref var scene_module = ref globalConfigData.value.Value.configTbscene_modules.configTbscene_modules;
//
//             NativeArray<Rectangle> obstacleList = new NativeArray<Rectangle>(obstacles.Length, Allocator.Temp);
//             for (int i = 0; i < obstacles.Length; i++)
//             {
//                 int width = 0;
//                 int height = 0;
//                 for (int j = 0; j < scene_module.Length; j++)
//                 {
//                     if (cdfeMapElementData[obstacles[i]].elementID == scene_module[j].id)
//                     {
//                         width = scene_module[j].size[0];
//                         height = scene_module[j].size[1];
//                         break;
//                     }
//                 }
//
//                 obstacleList[i] = new Rectangle
//                 {
//                     center = cdfeLocalTransform[obstacles[i]].Position.xy,
//                     width = width,
//                     height = height
//                 };
//             }
//
//             var mapRectangle = new Rectangle
//             {
//                 center = float2.zero,
//                 width = math.abs(mapWithHeight.x - 8),
//                 height = math.abs(mapWithHeight.y - 8)
//             };
//             int2 point = new int2((int)smallRect.center.x, (int)smallRect.center.y + 120);
//             for (int i = 0; i < 99999; i++)
//             {
//                 point.x = rand.NextInt((int)(bigRect.center.x - bigRect.width / 2),
//                     (int)(bigRect.center.x + bigRect.width / 2));
//                 point.y = rand.NextInt((int)(bigRect.center.y - bigRect.height / 2),
//                     (int)(bigRect.center.y + bigRect.height / 2));
//
//
//                 //TODO:在地图内部 不可在地图外部生成
//                 if (!smallRect.Contains(point) && mapRectangle.Contains(point))
//                 {
//                     bool isAllOut = true;
//                     foreach (var rec in obstacleList)
//                     {
//                         if (rec.Contains(point))
//                         {
//                             isAllOut = false;
//                             break;
//                         }
//                     }
//
//                     if (isAllOut)
//                     {
//                         break;
//                     }
//                 }
//             }
//
//             return point;
//         }
//     }
// }

