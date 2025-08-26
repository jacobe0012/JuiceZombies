//---------------------------------------------------------------------
// UnicornStudio
// Author: 迅捷蟹
// Time: 2023-07-28 15:58:18
//---------------------------------------------------------------------


//using NSprites;

using GPUECSAnimationBaker.Engine.AnimatorSystem;
using ProjectDawn.Navigation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Main
{
    //[InternalBufferCapacity(0)]
    public partial struct State : IBufferElementData
    {
    }

    [PolymorphicStruct]
    public interface IState
    {
        //模拟构造函数,选择状态的AI主体
        //public void Init(AIType type);

        //进入状态
        public void OnStateEnter(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData);

        //维持状态
        public void OnStateUpdate(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData);

        //退出状态
        public void OnStateExit(ref StateUpdateData_ReadWrite refData, in StateUpdateData_ReadOnly inData);
    }


    /// <summary>
    /// 一些需要修改的数据
    /// </summary>
    public struct StateUpdateData_ReadWrite
    {
        public StateMachine stateMachine;

        public ChaStats chaStats;
        public EntityCommandBuffer.ParallelWriter ecb;
        public EntityStorageInfoLookup storageInfoFromEntity;
        public AgentBody agentBody;
        public AgentShape agentShape;
        public LocalTransform localTransform;
        public Random rand;

        public PhysicsVelocity physicsVelocity;

        //public Flip flip;
        public PhysicsDamping physicsDamping;

        public PhysicsCollider physicsCollider;
        public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
        public ComponentLookup<PlayerData> cdfePlayerData;
        public ComponentLookup<PhysicsVelocity> cdfePhysicsVelocity;
        public ComponentLookup<PhysicsMass> cdfePhysicsMass;
        public AgentLocomotion AgentLocomotion;
        public PlayerData playerData;
        public EnemyData enemyData;
        public ComponentLookup<WeaponData> cdfeWeaponData;
        public GpuEcsAnimatorControlComponent animatorData;
        public DynamicBuffer<Child> bfeChild;
        public ComponentLookup<JiYuFlip> cdfeFlip;
        public DynamicBuffer<Skill> Skills;
    }

    public struct StateUpdateData_ReadOnly
    {
        public uint timeTick;
        public float eT;
        public float fdT;
        public Entity entity;
        public Entity player;
        public Entity wbe;
        public int sortkey;
        public InputData inputs;
        public PrefabMapData prefabMapData;
        public GameSetUpData gameSetUpData;
        public GameOthersData gameOthersData;
        public GameTimeData gameTimeData;
        public GameRandomData gameRandomData;
        public GlobalConfigData configData;
        public GameEnviromentData gameEnviromentData;
        public DynamicBuffer<DropsBuffer> dropsBuffer;

        public ComponentLookup<LocalTransform> cdfeLocalTransform;
        public ComponentLookup<EntityGroupData> cdfeEntityGroupData;

        public NativeArray<Entity> allEntities;
        public NativeArray<Entity> mapModules;
        public ComponentLookup<ChaStats> cdfeChaStats;

        public ComponentLookup<EnemyData> cdfeEnemyData;
        public ComponentLookup<TargetData> cdfeTargetData;
        public ComponentLookup<HitBackData> cdfeHitBackData;

        public ComponentLookup<TimeToDieData> cdfeTimeToDieData;
        public ComponentLookup<SpiritData> cdfeSpiritData;
        public ComponentLookup<ObstacleTag> cdfeObstacleTag;
        public DynamicBuffer<State> states;
        public ComponentLookup<MapElementData> cdfeMapElementData;
        public ComponentLookup<JiYuSort> cdfeJiYuSort;
        
        public BufferLookup<Buff> bfeBuff;
        public BufferLookup<GameEvent> bfeGameEvent;
        public BufferLookup<LinkedEntityGroup> bfeLinkedEntityGroup;
        
    }
}