using ProjectDawn.Navigation;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Stateful;
using Unity.Transforms;

namespace Main
{
    [PolymorphicStruct]
    interface ISkillOld
    {
        /// <summary>
        /// 技能产生时回调点
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData);

        /// <summary>
        /// 技能每帧更新
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData);

        /// <summary>
        /// 技能结束时回调点
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData);

        // /// <summary>
        // /// 技能结束时回调点
        // /// </summary>
        // /// <param name="refData"></param>
        // /// <param name="inData"></param>
        // void On(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData);


        void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData);
    }

    //[InternalBufferCapacity(0)]
    public partial struct SkillOld : IBufferElementData
    {
    }

    [PolymorphicStruct]
    interface ISkill
    {
        /// <summary>
        /// 技能产生时回调点
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        void OnCast(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData);

        /// <summary>
        /// 技能每帧更新
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        void OnUpdate(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData);

        /// <summary>
        /// 技能结束时回调点
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        void OnDestroy(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData);

        // /// <summary>
        // /// 技能结束时回调点
        // /// </summary>
        // /// <param name="refData"></param>
        // /// <param name="inData"></param>
        // void On(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData);


        void OnAwake(ref TimeLineData_ReadWrite refData, in TimeLineData_ReadOnly inData);
    }

    //[InternalBufferCapacity(0)]
    public partial struct Skill : IBufferElementData
    {
    }

    public enum TargetAttackType
    {
        /// <summary>
        /// 角色阵营
        /// </summary>
        PlayerOrSpirit = 7,

        /// <summary>
        /// 怪物阵营
        /// </summary>
        Enemy = 56,

        /// <summary>
        /// 仅角色
        /// </summary>
        Player = 1,

        /// <summary>
        /// 小怪
        /// </summary>
        LittleMonster = 8,

        /// <summary>
        /// boss
        /// </summary>
        Boss = 32,

        /// <summary>
        /// boss和精英
        /// </summary>
        NoLittleEnemy = 48,

        /// <summary>
        /// 障碍物
        /// </summary>
        Obstacle = 128,

        /// <summary>
        /// 一切
        /// </summary>
        All = 255
    }

    /// <summary>
    /// 用于陷阱技能
    /// </summary>
    public struct TrapTag : IComponentData
    {
    }

    //用于荆棘装备技能
    public struct ThronTag : IComponentData
    {
    }

    // public struct RayCastDect:IEnableableComponent,IComponentData { }

    public struct BlackHoleSuction : IComponentData
    {
    }

    public struct TimeLineData_ReadWrite
    {
        public EntityCommandBuffer.ParallelWriter ecb;

        public DynamicBuffer<Skill> skills;
        public EntityStorageInfoLookup storageInfoFromEntity;
        public ComponentLookup<ChaStats> cdfeChaStats;
        public GameRandomData rand;
        public ComponentLookup<AgentBody> cdfeAgentBody;
        public NativeArray<SkillAttackData> skillAtackDatas;
        public ComponentLookup<StateMachine> cdfeStateMachine;
        // public BulletPrefabs prefabs;
    }

    public struct TimeLineData_ReadOnly
    {
        public int sortkey;
        public Entity entity;
        public Entity player;
        public PrefabMapData prefabMapData;
        public GlobalConfigData globalConfig;
        public ComponentLookup<ChaStats> cdfeChaStats;
        public ComponentLookup<LocalTransform> cdfeLocalTransform;
        public ComponentLookup<PlayerData> cdfePlayerData;
        public ComponentLookup<EnemyData> cdfeEnemyData;
        public ComponentLookup<SpiritData> cdfeSpiritData;
        public BufferLookup<StatefulTriggerEvent> allTriggerEvent;
        public NativeArray<Entity> allEntities;
        public BufferLookup<Buff> bfeBuff;
        public ComponentLookup<TrapTag> cdfeTrapTag;
        public ComponentLookup<ObstacleTag> cdfeObstacleTag;
        public NativeArray<Entity> mapModules;
        public ComponentLookup<MapElementData> cdfeMapElementData;

        public NativeArray<Entity> skillAtackDatasEntity;
        public ComponentLookup<BossTag> cdfeBossTag;
        //public BufferLookup<PlayerEquipSkillBuffer> bfePlayerEquipSkillBuffer;
        public float fdT;
        public double eT;
        public DynamicBuffer<Trigger> triggers;
    }
}