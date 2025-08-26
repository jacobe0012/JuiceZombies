//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-08-24 12:25:25
//---------------------------------------------------------------------

using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Main
{
    public static class PhysicsHelper
    {
        private const int FieldWidth = 4000;
        private const int FieldWidthHalf = FieldWidth / 2;
        private const int FieldHeight = 4000;
        private const int FieldHeightHalf = FieldHeight / 2;
        private const float Step = 1f;

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hash(float2 position)
        {
            var quantized = new int2(math.floor(position / Step));
            return quantized.x + FieldWidthHalf + (quantized.y + FieldHeightHalf) * FieldWidth;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float2 Correction(float2 position)
        {
            var quantized = new int2(math.floor(position / Step));
            return new float2(quantized.x * Step + Step / 2, quantized.y * Step + Step / 2);
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NativeArray<int> GetPhysicsEntityInRadius(
            float2 center,
            float radius,
            NativeParallelMultiHashMap<int, int> bucketList
        )
        {
            var list = new NativeList<int>(Allocator.Temp);

            var foundSize = (int)math.ceil(radius / Step);
            var cellSize = foundSize * 2 + 1;

            for (var i = 0; i < cellSize; i++)
            {
                for (var j = 0; j < cellSize; j++)
                {
                    var x = i - foundSize;
                    var y = j - foundSize;
                    var newCenter = center + new float2(x, y) * Step;
                    var hash = Hash(newCenter);
                    var found = bucketList.TryGetFirstValue(hash, out var index, out var iterator);
                    while (found)
                    {
                        list.Add(index);
                        found = bucketList.TryGetNextValue(out index, ref iterator);
                    }
                }
            }

            return new NativeArray<int>(list.AsArray(), Allocator.Temp);
        }


        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 QuardaticBezier(float t, float3 points0, float3 points1, float3 points2)
        {
            float3 a = points0;
            float3 b = points1;
            float3 c = points2;

            float3 aa = a + (b - a) * t;
            float3 bb = b + (c - b) * t;
            return aa + (bb - aa) * t;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 CubicBezier(float t, float3 points0, float3 points1, float3 points2, float3 points3)
        {
            float3 a = points0;
            float3 b = points1;
            float3 c = points2;
            float3 d = points3;

            float3 aa = a + (b - a) * t;
            float3 bb = b + (c - b) * t;
            float3 cc = c + (d - c) * t;

            float3 aaa = aa + (bb - aa) * t;
            float3 bbb = bb + (cc - bb) * t;
            return aaa + (bbb - aaa) * t;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LoopToClipTime(float time, float duration)
        {
            float wrappedTime = math.fmod(time, duration);
            wrappedTime += math.select(0f, duration, wrappedTime < 0f);
            return wrappedTime;
        }

        /// <summary>
        /// 创建新的碰撞盒并设置CollisionFilter的BelongsTo数值
        /// </summary>
        /// <param name="oldCollider">旧的碰撞盒</param>
        /// <param name="belongsTo">此碰撞盒属于谁</param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PhysicsCollider CreateColliderWithBelongsTo(PhysicsCollider oldCollider, uint belongsTo)
        {
            BlobAssetReference<Collider> colliderCopy = oldCollider.Value.Value.Clone();
            var oldFilter = oldCollider.Value.Value.GetCollisionFilter();
            // Set the GroupIndex to GroupId, which is negative
            // This ensures that the debris within a group don't collide
            colliderCopy.Value.SetCollisionFilter(new CollisionFilter
            {
                BelongsTo = belongsTo,
                CollidesWith = oldFilter.CollidesWith,
                GroupIndex = oldFilter.GroupIndex
            });
            //oldCollider.ColliderPtr
            PhysicsCollider newCollider = new PhysicsCollider
            {
                Value = colliderCopy
            };
            return newCollider;
        }


        /// <summary>
        /// 创建新的碰撞盒并设置box的size
        /// </summary>
        /// <param name="oldCollider">原碰撞盒</param>
        /// <param name="size">尺寸</param>
        /// <returns>新碰撞盒</returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PhysicsCollider CreateBoxColliderAndSetSize(PhysicsCollider oldCollider, float3 size)
        {
            //TODO:如果更改了其他则创建一个新的碰撞盒
            BlobAssetReference<Collider> colliderCopy = oldCollider.Value.Value.Clone();

            PhysicsCollider newCollider = new PhysicsCollider
            {
                Value = colliderCopy
            };

            newCollider.Value.Value.BakeTransform(new AffineTransform(float3x3.Scale(size)));

            // unsafe
            // {
            //     var boxColliderPtr = (BoxCollider*)newCollider.ColliderPtr;
            //
            //
            //     var geo = boxColliderPtr->Geometry;
            //     geo.Size = size;
            //     boxColliderPtr->Geometry = geo;
            // }

            return newCollider;
        }

        /// <summary>
        /// 创建一个新碰撞盒并设置其碰撞事件类型
        /// </summary>
        /// <param name="oldCollider">旧碰撞盒</param>
        /// <param name="collisionResponsePolicy">碰撞事件类型</param>
        /// <returns>新碰撞盒</returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PhysicsCollider CreateColliderAndSetType(PhysicsCollider oldCollider,
            CollisionResponsePolicy collisionResponsePolicy)
        {
            BlobAssetReference<Collider> colliderCopy = oldCollider.Value.Value.Clone();

            PhysicsCollider newCollider = new PhysicsCollider
            {
                Value = colliderCopy
            };
            newCollider.Value.Value.SetCollisionResponse(collisionResponsePolicy);

            return newCollider;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Entity NerestTarget(NativeArray<Entity> allEntities,
            ComponentLookup<PhysicsCollider> cdfePhysicsCollider,
            Entity thisEntity, uint targetType, ref float distance)
        {
            Entity target = thisEntity;
            for (var i = 0; i < allEntities.Length; i++)
            {
                if (allEntities[i] == thisEntity || !cdfePhysicsCollider.HasComponent(allEntities[i]))
                {
                    continue;
                }

                uint otherBelong = cdfePhysicsCollider[allEntities[i]].Value.Value.GetCollisionFilter().BelongsTo;
                if ((otherBelong & targetType) == 0)
                {
                    continue;
                }
                //var belongs = (cdfePhysicsCollider[allEntities[i]].Value.Value.GetCollisionFilter().BelongsTo;


                // float nextDis = math.distance(cdfeLocalTransform[allEntities[i]].Position,
                //     cdfeLocalTransform[thisEntity].Position);
                // if (distance > nextDis && cdfeChaStats[allEntities[i]].chaResource.hp > 0)
                // {
                //     target = allEntities[i];
                //     distance = nextDis;
                // }
            }

            if (target == thisEntity)
            {
                return Entity.Null;
            }

            return target;
        }

        //public static quaternion SetTrans(NativeArray<Entity> allEntities,
        //    ComponentLookup<LocalTransform> cdfeLocalTransform, ComponentLookup<ChaStats> cdfeChaStats,
        //    ComponentLookup<PlayerData> cdfePlayerData, ComponentLookup<EnemyData> cdfeEnemyData,
        //    ComponentLookup<SpiritData> cdfeSpiritData, ComponentLookup<ObstacleTag> cdfeObstacleTag, Entity player,
        //    TargetAttackType targetType, float distance,
        //    float angle)
        //{
        //    distance = 999f;
        //    var target = BuffHelper.NerestTarget(allEntities, cdfeLocalTransform,
        //        cdfeChaStats, cdfePlayerData, cdfeEnemyData, cdfeSpiritData, cdfeObstacleTag,
        //        player, targetType, ref distance);
        //    if (distance > distance)
        //    {
        //        target = default;
        //    }

        //    if (target != default ? true : false)
        //    {
        //        float3 dir = cdfeLocalTransform[target].Position -
        //                     cdfeLocalTransform[player].Position;


        //        float needAngel = MathHelper.SignedAngle(math.normalizesafe(dir),
        //            new float3(0, 1, 0));

        //        return cdfeLocalTransform[player].RotateZ(math.radians(needAngel - angle / 2f)).Rotation;
        //    }

        //    return cdfeLocalTransform[player].Rotation;
        //}


        /// <summary>
        /// 创建新的碰撞盒并设置CollisionFilter的CollidesWith数值
        /// </summary>
        /// <param name="oldCollider">旧的碰撞盒</param>
        /// <param name="collidesWith">此碰撞盒和谁发生碰撞</param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PhysicsCollider CreateColliderWithCollidesWith(PhysicsCollider oldCollider, uint collidesWith)
        {
            //return
            BlobAssetReference<Collider> colliderCopy = oldCollider.Value.Value.Clone();
            var oldFilter = oldCollider.Value.Value.GetCollisionFilter();
            // Set the GroupIndex to GroupId, which is negative
            // This ensures that the debris within a group don't collide
            colliderCopy.Value.SetCollisionFilter(new CollisionFilter
            {
                BelongsTo = oldFilter.BelongsTo,
                CollidesWith = collidesWith,
                GroupIndex = oldFilter.GroupIndex
            });

            PhysicsCollider newCollider = new PhysicsCollider
            {
                Value = colliderCopy
            };
            return newCollider;
        }

        /// <summary>
        /// 攻击者目标值能否打到受击者
        /// </summary>
        /// <param name="skillTarget">攻击者目标值</param>
        /// <param name="denfenderTarget">受击者目标值</param>
        /// <returns></returns>
        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTargetEnabled(TargetData attacker, TargetData denfender)
        {
            return ((uint)denfender.BelongsTo & (uint)attacker.AttackWith) != 0;
        }

        [BurstCompile]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsTargetEnabled(uint attacker, uint denfender)
        {
            return ((uint)denfender & (uint)attacker) != 0;
        }


        /// <summary>
        /// 修改碰撞盒belongsTo的值
        /// </summary>
        /// <param name="oldCollider">碰撞盒</param>
        /// <param name="belongsTo">此碰撞盒属于谁</param>
        /// <returns></returns>
        //[BurstCompile]
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        // public unsafe static PhysicsCollider SetColliderBelongsTo(PhysicsCollider oldCollider, uint belongsTo)
        // {
        //     var bxPtr = oldCollider.ColliderPtr;
        //     var oldFilter = bxPtr->GetCollisionFilter();
        //
        //     if (oldFilter.BelongsTo != belongsTo)
        //     {
        //         bxPtr->SetCollisionFilter(new CollisionFilter
        //         {
        //             BelongsTo = belongsTo,
        //             CollidesWith = oldFilter.CollidesWith,
        //             GroupIndex = oldFilter.GroupIndex
        //         });
        //     }
        //
        //     return oldCollider;
        // }

        /// <summary>
        /// 修改碰撞盒collidesWith的值
        /// </summary>
        /// <param name="oldCollider">碰撞盒</param>
        /// <param name="collidesWith">设置和谁碰撞</param>
        /// <returns></returns>
        //[BurstCompile]

        // public unsafe static PhysicsCollider SetColliderCollideWith(PhysicsCollider oldCollider, uint collideWith)
        // {
        //     var bxPtr = oldCollider.ColliderPtr;
        //     var oldFilter = bxPtr->GetCollisionFilter();
        //
        //     if (oldFilter.CollidesWith != collideWith)
        //     {
        //         bxPtr->SetCollisionFilter(new CollisionFilter
        //         {
        //             BelongsTo = oldFilter.BelongsTo,
        //             CollidesWith = collideWith,
        //             GroupIndex = oldFilter.GroupIndex
        //         });
        //     }
        //
        //     return oldCollider;
        // }


        // public static NativeArray<float2> GetMostMonsterPosInRadius(
        //     float2 center,
        //     float radius,
        //     int number,
        //     NativeMultiHashMap<int, int> bucketList,
        //     NativeHashMap<int, PhysicsData> physicsList
        // )
        // {
        //     var densityList = new NativeList<MonsterDensity>(Allocator.Temp);
        //     var list = new NativeList<float2>(Allocator.Temp);
        //
        //     var foundSize = (int)math.ceil(radius / Step);
        //     var cellSize = foundSize * 2 + 1;
        //
        //     for (var i = 0; i < cellSize; i++)
        //     {
        //         for (var j = 0; j < cellSize; j++)
        //         {
        //             var x = i - foundSize;
        //             var z = j - foundSize;
        //             var newCenter = center + new float2(x, z) * Step;
        //             var hash = Hash(newCenter);
        //             var tempSize = 0;
        //             var found = bucketList.TryGetFirstValue(hash, out var index, out var iterator);
        //             while (found)
        //             {
        //                 if (physicsList[index].RoleType != RoleType.Player)
        //                 {
        //                     tempSize++;
        //                 }
        //                 found = bucketList.TryGetNextValue(out index, ref iterator);
        //             }
        //
        //             if (tempSize > 0)
        //             {
        //                 densityList.Add(new MonsterDensity()
        //                 {
        //                     Position = Correction(newCenter),
        //                     Number = tempSize
        //                 });
        //             }
        //         }
        //     }
        //
        //     densityList.Sort();
        //
        //     for (var i = 0; i < densityList.Length && i < number; i++)
        //     {
        //         list.Add(densityList[i].Position);
        //     }
        //
        //     return new NativeArray<float2>(list.AsArray(), Allocator.Temp);
        // }
    }
}