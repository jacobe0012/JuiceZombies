using ProjectDawn.Navigation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Main
{
    [PolymorphicStruct]
    interface ISkillAttack
    {
        LocalTransform DoSkillMove(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData);

        /// <summary>
        /// 当实体击中目标时
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        void OnHit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData);

        ///// <summary>
        ///// 当该技能实体被销毁时
        ///// </summary>
        ///// <param name="refData"></param>
        ///// <param name="inData"></param>
        void OnDestroy(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData);

        /// <summary>
        /// 除第一帧每帧执行
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        void OnUpdate(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData);

        /// <summary>
        /// 第一帧执行
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        void OnStart(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData);

        /// <summary>
        /// 离开时
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        void OnExit(ref SkillAttackData_ReadWrite refData, in SkillAttackData_ReadOnly inData);
    }

    public struct SkillAttackData_ReadWrite
    {
        public ComponentLookup<LocalTransform> cdfeLocalTransform;
        public EntityCommandBuffer.ParallelWriter ecb;
        public EntityStorageInfoLookup storageInfoFromEntity;
        public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
        public ComponentLookup<AgentBody> cdfeAgentBody;
        public BufferLookup<Skill> bfeSkill;
        public PhysicsCollider physicsCollider;
        public BufferLookup<Buff> bfeBuff;
        public ComponentLookup<PhysicsVelocity> cdfePhysicsVelocity;
        public ComponentLookup<TargetData> cdfeTargetData;
        public ComponentLookup<EnemyData> cdfeEnemyData;
        public ComponentLookup<BulletSonData> cdfeBulletSonData;
        public ComponentLookup<AreaTempHp> cdfeAreaTempHp;
        public ComponentLookup<AgentLocomotion> cdfeAgentLocomotion;
        public ComponentLookup<PhysicsMass> cdfePhysicsMass;
        public ComponentLookup<ChaStats> cdfeChaStats;
    }


    public struct SkillAttackData_ReadOnly
    {
        public uint timeTick;
        public int sortkey;
        public float fDT;
        public float eT;
        public GlobalConfigData config;
        public PrefabMapData prefabMapData;
        public Entity player;
        public Entity entity;
        public ComponentLookup<ChaStats> cdfeChaStats;
        public NativeArray<Entity> allEntities;
        public NativeArray<Entity> targetEntities;
        public ComponentLookup<TrapTag> cdfeTrapTag;
        public ComponentLookup<ThronTag> cdfeThronTag;
        public ComponentLookup<ObstacleTag> cdfeObstacleTag;
        public ComponentLookup<AreaTag> cdfeAreaTag;
        public ComponentLookup<BoardData> cdfeBoardData;
        public ComponentLookup<BulletData> cdfeBulletTag;
        public FixedList512Bytes<Entity> otherEntities;

        public ComponentLookup<SpiritData> cdfeSpiritData;
        public ComponentLookup<PlayerData> cdfePlayerData;
        public Entity wbe;
        public ComponentLookup<BossTag> bfeBossTag;
        public ComponentLookup<ElementData> cdfeElementData;

        public NativeArray<SkillAttackData> skillAttackDatas;

        public ComponentLookup<MapElementData> cdfemapElementData;
        public ComponentLookup<WeaponData> cdfeWeaponData;
        public ComponentLookup<BulletTempTag> cdfeBulletTempTag;
        public ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix;
        public BufferLookup<Trigger> cdfeTrigger;
        public GameOthersData gameOthersData;
        public GameTimeData gameTimeData;
        public uint seed;
    }


    /// <summary>
    /// 技能实体
    /// </summary>
    public struct SkillAttackData : IComponentData
    {
        public SkillAttack data;
    }


    /// <summary>
    /// 弃用
    /// </summary>
    //[InternalBufferCapacity(0)]
    public struct SkillHitEffectBuffer : IBufferElementData
    {
        /// <summary>
        /// buffID
        /// </summary>
        public int buffID;


        /// <summary>
        /// buff参数 (只接收含有推力，伤害，控制效果的buffID的参数，并且值是最终计算过的参数值)
        /// 有推力效果buff：比如BUFF40000003
        /// c0:推力万分比，属性id，属性对他的影响万分比；
        /// c1:(float3)推力方向             
        /// c2:x: 1技能持有者  ，2自身，技能实体(优先级 最高) 3.向垂直正方向两边击退
        /// c3:x:目标
        /// 其他赋0
        /// 
        /// 有伤害效果并有属性加成buff：比如BUFF30000003
        /// c0：buff属性，属性id，属性对他的影响万分比；
        /// c3:x:目标
        /// 其他赋0
        ///
        /// 固定伤害buff：比如BUFF30000004
        /// c0：x：伤害值
        /// c3:x:目标
        /// 其他赋0
        /// 
        /// </summary>
        public float3x4 buffArgs;
    }


    /// <summary>
    /// 记录了2分钟的位置
    /// </summary>
    [InternalBufferCapacity(0)]
    public struct Skill2MinutesBuffer:IBufferElementData
    {
        public float3 position;
    }

    ///// <summary>
    ///// 技能实体的数据 用于数学检测碰撞
    ///// </summary>
    //public struct SkillAttackShape : IComponentData
    //{
    //    /// <summary>
    //    /// 技能实体类型 1.点对点 2.矩形 用凸多边 3.扇形 4.圆形
    //    /// </summary>
    //    public SKILLSHAPETYPE shapeType;

    //    /// <summary>
    //    /// 矩形的寬高
    //    /// </summary>
    //    public float2 size;

    //    /// <summary>
    //    /// x为扇形和圆形的半径 y为角度
    //    /// </summary>
    //    public float2 sectorPar;
    //}

    //public enum SKILLSHAPETYPE
    //{
    //    Point = 0,
    //    Rectangle = 1,
    //    Sector = 2,
    //    Circle = 3
    //}
}