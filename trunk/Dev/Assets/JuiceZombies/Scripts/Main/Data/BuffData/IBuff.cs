//---------------------------------------------------------------------
// UnicornStudio
// Author: jaco0012
// Time: 2023-07-17 12:11:46
//---------------------------------------------------------------------


using ProjectDawn.Navigation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Main
{
    //Buff缓冲区组件 由ManageBuffSystem管理生命周期
    //[InternalBufferCapacity(0)]
    public partial struct BuffOld : IBufferElementData
    {
    }

    //BuffNew缓冲区组件 由ManageBuffSystem管理生命周期
    //[InternalBufferCapacity(0)]
    public partial struct Buff : IBufferElementData
    {
    }

    [PolymorphicStruct]
    public interface IBuff
    {
        public void OnOccur(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData);
        public void OnRemoved(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData);

        public void OnUpdate(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData);

        public void OnTick(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData);

        //
        public void OnHit(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData);

        public void OnBeHurt(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData);

        public void OnPerUnitMove(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData);
        public void OnKill(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData);
        public void OnBeforeBeKilled(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData);
        public void OnBeKilled(ref BuffData_ReadWrite refData, in BuffData_ReadOnly inData);
    }

    [PolymorphicStruct]
    public interface IBuffOld
    {
        public void OnOccur(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);
        public void OnRemoved(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);
        public void OnTick(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);
        public void OnCast(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);
        public void OnHit(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);
        public void OnBeHurt(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);
        public void OnKill(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);
        public void OnBeforeBeKilled(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);
        public void OnBeKilled(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);

        public void OnPerUnitMove(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);
        //public void On(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);

        /// <summary>
        /// 升级时回调点,目前玩家专用
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnLevelUp(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);
    }

    public struct BuffOldData_ReadWrite
    {
        public DynamicBuffer<BuffOld> buff;
        public EntityCommandBuffer.ParallelWriter ecb;
        public DamageInfo damageInfo;
        public ComponentLookup<ChaStats> cdfeChaStats;
        public ComponentLookup<PlayerData> cdfePlayerData;
        public Skill skill;
        public EntityStorageInfoLookup storageInfoFromEntity;
        public ComponentLookup<DropsData> cdfeDropsData;
        public ChaStats defenderChaStats;
        public ComponentLookup<AgentBody> cdfeAgentBody;
        public ComponentLookup<AgentLocomotion> cdfeAgentLocomotion;
        public BufferLookup<SkillOld> cdfeSkill;
        public int subDamage;
        public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
        public ComponentLookup<PhysicsVelocity> cdfePhysicsVolocity;
    }

    public struct BuffOldData_ReadOnly
    {
        public int sortkey;
        public Entity entity;
        public Entity player;
        public Entity wbe;
        public PrefabMapData prefabMapData;
        public GlobalConfigData globalConfigData;
        public ComponentLookup<LocalTransform> cdfeLocalTransform;
        public NativeArray<Entity> entities;
        public NativeArray<Entity> dropsEntities;
        public NativeArray<Entity> mapModules;
        public ComponentLookup<MapElementData> cdfeMapElementData;
        public ComponentLookup<EnemyData> cdfeEnemyData;
        public ComponentLookup<SpiritData> cdfeSpiritData;
        public ComponentLookup<PhysicsMass> cdfePhysicsMass;
        public float fdT;
    }

    public struct BuffData_ReadWrite
    {
        public DynamicBuffer<Buff> buff;
        public EntityCommandBuffer.ParallelWriter ecb;
        public DamageInfo damageInfo;
        public ComponentLookup<ChaStats> cdfeChaStats;
        public ComponentLookup<PlayerData> cdfePlayerData;
        public Skill skill;
        public EntityStorageInfoLookup storageInfoFromEntity;
        public ComponentLookup<DropsData> cdfeDropsData;
        public ChaStats defenderChaStats;
        public ComponentLookup<AgentBody> cdfeAgentBody;
        public ComponentLookup<AgentLocomotion> cdfeAgentLocomotion;
        public BufferLookup<Skill> cdfeSkill;
        public BufferLookup<BackTrackTimeBuffer> bfeBackTrackTimeBuffer;
        //public long sumDamage;
        public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
        public ComponentLookup<PhysicsVelocity> cdfePhysicsVolocity;
        public ComponentLookup<PhysicsMass> cdfePhysicsMass;
        public ComponentLookup<EnemyData> cdfeEnemyData;
        
        public ComponentLookup<LocalTransform> cdfeLocalTransform;
        public ComponentLookup<StateMachine> cdfeStateMachine;
        public ComponentLookup<JiYuSort> cdfeJiYuSort;
    }

    public struct BuffData_ReadOnly
    {
        public int sortkey;
        public Entity entity;
        public Entity player;
        public Entity wbe;
        public PrefabMapData prefabMapData;
        public GlobalConfigData globalConfigData;
        public ComponentLookup<LocalTransform> cdfeLocalTransform;
        public NativeArray<Entity> entities;
        public NativeArray<Entity> dropsEntities;
        public NativeArray<Entity> mapModules;
        public ComponentLookup<MapElementData> cdfeMapElementData;
        public ComponentLookup<SpiritData> cdfeSpiritData;
        //public ComponentLookup<PhysicsMass> cdfePhysicsMass;
        public ComponentLookup<HitBackData> cdfeHitBackData;
        public ComponentLookup<ObstacleTag> cdfeObstacleTag;
        public float fdT;
        public float eT;
        public uint timeTick;
        public BufferLookup<Skill> bfeSkill;
        public DynamicBuffer<Child> bfeChild;
        public GameRandomData gameRandomData;
        public ComponentLookup<SpecialEffectData> cdfeSpecialEffectData;
        public DynamicBuffer<LinkedEntityGroup> linkedEntityGroup;
        public ComponentLookup<WeaponData> cdfeWeaponData;
        public GameTimeData gameTimeData;
        public GameOthersData gameOthersData;
        public GameEnviromentData enviromentData;
        public ComponentLookup<TargetData> cdfeTargetData;
        public ComponentLookup<JiYuSort> cdfeJiYuSort;
        public ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix;
        public ComponentLookup<EntityGroupData> cdfeEntityGroupData;
        
    }

    public struct BuffArgs
    {
        public int args0;
        public int args1;
        public int args2;
        public int args3;
        public int args4;
    }


    public struct BuffArgsNew
    {
        public int4 args1;
        public int4 args2;
        public int4 args3;
        public int4 args4;
    }

    public struct BuffStack
    {
        ///<summary>2
        ///buff堆叠的规则中需要的层数，在这个游戏里只要id和caster相同的buffObj就可以堆叠
        ///激战2里就不同，尽管图标显示堆叠，其实只是统计了有多少个相同id的buffObj作为层数显示了
        ///</summary>
        public int maxStack;

        ///<summary>9
        ///当前层数
        ///</summary>
        public int stack;

        ///<summary>12
        ///上一帧的层数 用来触发由层数变化的Onoccor
        ///</summary>
        public int laststack;

        ///<summary>13
        ///合并时是否保持原有buff持续时间
        ///</summary>
        public bool keepOriginalTime;
    }
    // public struct BuffProperty
    // {
    //     //固定加成
    //     public ChaProperty plus;
    //
    //     //万分比
    //     public ChaProperty addition;
    // }
}