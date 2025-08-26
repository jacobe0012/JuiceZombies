// //---------------------------------------------------------------------
// // UnicornStudio
// // Author: 迅捷蟹
// // Time: 2023-08-30 14:30:10
// //---------------------------------------------------------------------
//
//
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Physics;
// using Unity.Transforms;
// using UnityEngine;
//
// namespace Flow
// {
//     public static class CustomTags
//     {
//         public const uint Tag1 = 1 << 0; // 0001
//         public const uint Tag2 = 1 << 1; // 0010
//         public const uint Tag3 = 1 << 2; // 0100
//     }
//
//     public static class CostFieldHelper
//     {
//         /// <summary>
//         /// 额外消耗,也就是地形为1,特殊地形为2-254,不可通过为255
//         /// </summary>
//         /// <param name="worldPos"></param>
//         /// <param name="cellRadius">单元格直径</param>
//         /// <returns></returns>
//         public static byte EvaluateCost(float3 worldPos, float cellRadius, ref PhysicsWorld physicsWorld,
//             NativeArray<LocalTransform> trans)
//         {
//             foreach (var tran in trans)
//             {
//                 
//                 
//                 
//                 
//                 
//                 
//                 
//             }
//
//             byte newCost = 0;
//             byte InitCost = 5;
//             float3 cellHalfExtents = Vector3.one * cellRadius;
//             //UnityEngine.Collider[] obstacles = Physics.OverlapBox(worldPos, cellHalfExtents, Quaternion.identity, _terrainMask);
//             var filter = new CollisionFilter
//             {
//                 GroupIndex = 0,
//                 BelongsTo = CustomTags.Tag1 | CustomTags.Tag2 | CustomTags.Tag3,
//                 CollidesWith = ~0u,
//             };
//             bool hasIncreasedCost = false;
//
//             // 获取当前的CollisionWorld
//             var collisionWorld = physicsWorld.CollisionWorld;
//
//             //盒子碰撞
//             var aabb = new Aabb
//             {
//                 Max = worldPos + cellHalfExtents,
//                 Min = worldPos - cellHalfExtents,
//             };
//
//             // 定义一个Allocator，用于存储查询结果
//             var allocator = new NativeList<int>(Allocator.Temp);
//             // 执行AABB重叠查询
//             collisionWorld.OverlapAabb(new OverlapAabbInput
//             {
//                 Aabb = aabb,
//                 Filter = filter // 指定要查询的碰撞层
//             }, ref allocator);
//
//             // 遍历查询结果
//             foreach (var hit in allocator)
//             {
//                 // 获取实体
//                 var hitCollider = physicsWorld.Bodies[hit].Collider;
//                 var hitFilter = hitCollider.Value.GetCollisionFilter();
//
//                 if (!hasIncreasedCost)
//                 {
//                     if (((hitFilter.BelongsTo & CustomTags.Tag3) != 0) && InitCost <= byte.MaxValue)
//                     {
//                         // 对具有Tag3标签的物体进行处理
//
//                         newCost = byte.MaxValue;
//                         InitCost = newCost;
//                         hasIncreasedCost = true;
//                     }
//                     else if (((hitFilter.BelongsTo & CustomTags.Tag2) != 0) && InitCost <= 6)
//                     {
//                         // 对具有Tag2标签的物体进行处理
//                         newCost = 6;
//                         InitCost = newCost;
//                     }
//                     else if (((hitFilter.BelongsTo & CustomTags.Tag1) != 0) && InitCost <= 5)
//                     {
//                         // 对具有Tag1标签的物体进行处理
//                         newCost = 5;
//                         InitCost = newCost;
//                     }
//                 }
//             }
//
//
//             // 释放分配器
//             allocator.Dispose();
//
//             return newCost;
//         }
//     }
// }

