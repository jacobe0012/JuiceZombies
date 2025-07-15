using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using BoxCollider = Unity.Physics.BoxCollider;
using SphereCollider = Unity.Physics.SphereCollider;

//using NSprites;

namespace Main
{
    public class EnemyDataAuthoring : MonoBehaviour
    {
        [SerializeField] public ChaStats ChaStats;
        [SerializeField] public EnemyData EnemyData;

        //[SerializeField] public GameObject RenderEntity;

        public class EnemyDataBaker : Baker<EnemyDataAuthoring>
        {
            public override void Bake(EnemyDataAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, authoring.ChaStats);
                var myHybridMono = authoring.GetComponent<MyHybridMono>();
                if (myHybridMono != null)
                {
                    authoring.EnemyData.isHybridSpine = true;
                    AddComponent<TransformHybridUpdateData>(entity);
                }


                // authoring.EnemyData.rendererEntity =
                //     GetEntity(authoring.RenderEntity);
                // if (authoring.GetComponentInChildren<MeshRenderer>()?.gameObject != null)
                // {
                //     authoring.EnemyData.rendererEntity =
                //         GetEntity(authoring.GetComponentInChildren<MeshRenderer>()?.gameObject);
                // }

                AddComponent(entity, authoring.EnemyData);
                AddBuffer<State>(entity);
                AddComponent<StateMachine>(entity);
                AddBuffer<DropsBuffer>(entity);
                //AddBuffer<BuffOld>(entity);
                AddBuffer<BanBulletTriggerBuffer>(entity);
                AddBuffer<Buff>(entity);
                AddBuffer<Skill>(entity);
                AddBuffer<Trigger>(entity);
                AddBuffer<LinkedEntityGroup>(entity);
                //AddComponent<Flip>(entity);
                AddBuffer<BulletCastData>(entity);
                AddBuffer<GameEvent>(entity);
                AddComponent<BulletSonData>(entity);
                AddComponent<FlipData>(entity);
                AddComponent<PushColliderData>(entity);
                AddComponent<EntityGroupData>(entity);
                AddComponent(entity, new TargetData
                {
                    tick = 0,
                    BelongsTo = (uint)BuffHelper.TargetEnum.LittleMonster,
                    AttackWith = 0
                });
            }
        }
    }

    /// <summary>
    /// 怪物掉落buffer  怪物生成时已确定 掉落组id
    /// </summary>
    //[InternalBufferCapacity(0)]
    public struct DropsBuffer : IBufferElementData
    {
        public int id;
    }

    [Serializable]
    public struct EnemyData : IComponentData
    {
        public int enemyID;
        public int type;

        /// <summary>
        /// 势力id
        /// </summary>
        public int powerId;

        public EnemyAttackType attackType;
        public float attackRadius;
        public Entity target;

        public uint targetGroup;
        public bool isHitBackAttack;
        public EnemyMoveType moveType;
        public int4 moveTypePara;
        public FixedList32Bytes<int> enemyFeature;
        //public int survivalTick;

        /// <summary>
        /// entity集合
        /// </summary>
        //public EntityGroup entityGroup;

        /// <summary>
        /// 不能在预制件上本来就有的赋Entity 否则实例化的都会指向预制件本身
        /// </summary>
        //public Entity rendererEntity;
        public int weaponId;

        /// <summary>
        /// boss场景id
        /// </summary>
        public int bossSceneId;

        //Boss 公共冷却
        public float commonSkillCd;
        public float curCommonSkillCd;

        //分裂技能+当前时间取整
        public int sonBulletId;

        //怪物卡住移动
        public EnemyMove enemyMove;

        /// <summary>
        /// 怪物刷新组id
        /// </summary>
        public int enemyRefreshGroupId;

        /// <summary>
        /// 是否是spine动画
        /// </summary>
        public bool isHybridSpine;

        /// <summary>
        /// 是否能放技能
        /// </summary>
        public bool canCastSkill;

        /// <summary>
        /// 禁用Flip 0启用  1禁用
        /// </summary>
        public int changeDirYn;
    }

    // public struct EntityGroup
    // {
    //     /// <summary>
    //     /// 武器entity 动态创建的代码里赋值可以
    //     /// </summary>
    //     public Entity weaponEntity;
    //
    //     /// <summary>
    //     /// 渲染entity 动态创建的代码里赋值可以
    //     /// </summary>
    //     //public Entity rendererEntity;
    // }

    public struct EnemyMove
    {
        public float3 lastPosition;
        public float stuckTimer;
        public bool isStuck;
        public bool isReversing;
        public float reverseTimer;
    }


    public enum EnemyAttackType
    {
        None,

        //以前保留
        CollideAttack,

        /// <summary>
        /// 近战
        /// </summary>
        NormalShortAttack,

        /// <summary>
        /// 
        /// </summary>
        //NormalShortSectorAttack,

        /// <summary>
        /// 远程
        /// </summary>
        NormalLongAttack,
    }


    public enum EnemyMoveType
    {
        Default = 1,
        FollowPlayerShadow = 2,
        FollowPlayerShadowWithOffset = 3,
        Escape = 4,

        //保持距离 适用于boss1502的移动方式
        KeepDis = 52,

        //选点移动
        RandomSelect = 53,

        //与玩家保持一个水平线
        KeepPlayerOnLine = 55,

        //垂直行走
        WalkStraight = 56
    }

    // [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    // partial struct PhysicsSizeEnemyBakingSystem : ISystem
    // {
    //     public void OnUpdate(ref SystemState state)
    //     {
    //         foreach (var (physicsColl, tran, _, entity) in
    //                  SystemAPI.Query<RefRW<PhysicsCollider>, RefRO<LocalTransform>, RefRO<EnemyData>>()
    //                      .WithEntityAccess())
    //         {
    //             unsafe
    //             {
    //                 var rwColl = physicsColl.ValueRW.Value.Value.Type;
    //                 float3 oldSize = 1.0f;
    //                 float3 newSize = 1.0f;
    //                 switch (rwColl)
    //                 {
    //                     case ColliderType.Box:
    //                         BoxCollider* bxPtr = (BoxCollider*)physicsColl.ValueRW.ColliderPtr;
    //                         oldSize = bxPtr->Size;
    //                         newSize = oldSize / tran.ValueRO.Scale;
    //                         Debug.Log($"oldSize{oldSize} newSize{newSize}");
    //                         var boxGeometry = bxPtr->Geometry;
    //                         boxGeometry.Size = newSize;
    //                         bxPtr->Geometry = boxGeometry;
    //                         break;
    //                     case ColliderType.Sphere:
    //                         SphereCollider* sphereCollider = (SphereCollider*)physicsColl.ValueRW.ColliderPtr;
    //                         var oldSize0 = sphereCollider->Radius;
    //                         var newSize0 = oldSize0 / tran.ValueRO.Scale;
    //
    //                         var boxGeometry0 = sphereCollider->Geometry;
    //                         boxGeometry0.Radius = newSize0;
    //                         sphereCollider->Geometry = boxGeometry0;
    //                         break;
    //                 }
    //             }
    //         }
    //     }
    // }
}