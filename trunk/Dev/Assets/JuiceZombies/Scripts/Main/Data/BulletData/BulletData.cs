using ProjectDawn.Navigation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Main
{
    // [PolymorphicStruct]
    // interface IBullet
    // {
    //     LocalTransform DoSkillMove(ref BulletData_ReadWrite refData, in BulletData_ReadOnly inData);
    //
    //     /// <summary>
    //     /// 当实体击中目标时
    //     /// </summary>
    //     /// <param name="refData"></param>
    //     /// <param name="inData"></param>
    //     void OnHit(ref BulletData_ReadWrite refData, in BulletData_ReadOnly inData);
    //
    //     ///// <summary>
    //     ///// 当该技能实体被销毁时
    //     ///// </summary>
    //     ///// <param name="refData"></param>
    //     ///// <param name="inData"></param>
    //     void OnDestroy(ref BulletData_ReadWrite refData, in BulletData_ReadOnly inData);
    //
    //     /// <summary>
    //     /// 除第一帧每帧执行
    //     /// </summary>
    //     /// <param name="refData"></param>
    //     /// <param name="inData"></param>
    //     void OnUpdate(ref BulletData_ReadWrite refData, in BulletData_ReadOnly inData);
    //
    // }

    public struct BulletData_ReadWrite
    {
        public ComponentLookup<LocalTransform> cdfeLocalTransform;
        public EntityCommandBuffer.ParallelWriter ecb;
        public EntityStorageInfoLookup storageInfoFromEntity;
        public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
        public ComponentLookup<AgentBody> cdfeAgentBody;
    }


    public struct BulletData_ReadOnly
    {
        public int sortkey;
        public float fDT;

        public GlobalConfigData config;
        public PrefabMapData prefabMapData;
        public Entity player;
        public Entity entity;
        public ComponentLookup<ChaStats> cdfeChaStats;
        public ComponentLookup<ObstacleTag> cdfeObstacleTag;
        public NativeArray<Entity> otherEntities;
        public ComponentLookup<EnemyData> cdfeEnemyData;
        public ComponentLookup<SpiritData> cdfeSpiritData;
        public ComponentLookup<PlayerData> cdfePlayerData;
        public Random rand;
        public BufferLookup<BuffOld> bfeBuff;
        public ComponentLookup<BossTag> bfeBossTag;

        //public NativeArray<SkillAttackData> skillAttackDatas;
        // public ComponentLookup<QueryData> cdfeQueryData;
        // public ComponentLookup<RayCastDect> cdfeRayCastDect;
    }


    /// <summary>
    /// 弹幕轨迹类型
    /// </summary>
    // public enum BulletTrakType
    // {
    //     StraightLine = 1,
    //     QuadraticBezier = 2,
    //     CubicBezier = 3,
    //     BounceLine = 4,
    //     WaveLine = 5
    // }

    /// <summary>
    /// 子弹击中后的临时entity持有
    /// </summary>
    public struct BulletTempTag : IComponentData
    {
        public Entity onHitTarget;
        /// <summary>
        /// 子弹命中时的子弹飞行方向
        /// </summary>
        public float3 dir;
    }

    /// <summary>
    /// 分裂子弹击中后为了后续同时刻同技能失效的数据
    /// </summary>
    public struct BulletSonData : IComponentData
    {
        /// <summary>
        /// 发射者 
        /// </summary>
        public Entity caster;

        /// <summary>
        /// 技能id 
        /// </summary>
        public int skillId;

        /// <summary>
        /// 添加时间 
        /// </summary>
        public int addTime;

        public bool Equals(Entity caster, int skillId, int addTime)
        {
            return this.addTime == addTime && this.skillId == skillId;
        }
    }

    /// <summary>
    /// 弹幕组件
    /// </summary>
    //[InternalBufferCapacity(0)]
    public struct BulletCastData : IBufferElementData
    {
        /// <summary>
        /// 0 弹幕id
        /// </summary>
        public int id;

        /// <summary>
        /// 1 弹幕的持续时间
        /// </summary>
        public float duration;

        /// <summary>
        /// 2 当前帧
        /// </summary>
        public int tick;

        /// <summary>
        /// 3 技能实体释放者
        /// </summary>
        public Entity caster;

        /// <summary>
        ///9 是否已经锁定目标
        /// </summary>
        public bool isTargeted;

        /// <summary>
        /// 10 如果已经索敌 传改loc
        /// </summary>
        public LocalTransform localTransform;

        /// <summary>
        /// 当前弹幕波数 11
        /// </summary>
        public float curGroup;

        /// <summary>
        /// 弹幕发射延迟 12
        /// </summary>
        public float delay;

        /// <summary>
        /// 弹幕发射点 13
        /// </summary>
        public float3 bulletPos;


        /// <summary>
        /// 弹幕追踪目标 14
        /// </summary>
        public Entity followedEntity;

        /// <summary>
        /// 技能id 15
        /// </summary>
        public int skillId;

        /// <summary>
        /// 添加时间 16
        /// </summary>
        public int addTime;

        /// <summary>
        /// 子弹幕数量 17
        /// </summary>
        public int sonBulletNum;

        /// <summary>
        /// 子弹幕范围      18
        /// </summary>
        public float sonBulletRange;


        public bool isOrderDir;

        public float3 dir;
    }
}