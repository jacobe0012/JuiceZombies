using ProjectDawn.Navigation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Main
{
    [PolymorphicStruct]
    public interface IGameEvent
    {
        /// <summary>
        /// 一开始执行 只执行一次
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnOccur(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData);

        /// <summary>
        /// 事件被销毁时
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnEventRemove(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData);


        /// <summary>
        /// 每多少秒执行一次
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnPerGap(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData);


        /// <summary>
        /// 单次执行
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnOnceAct(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData);


        /// <summary>
        /// 进入时触发 只触发一次
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnCharacterEnter(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData);


        /// <summary>
        /// 离开地形 单次触发
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnCharacterExit(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData);


        /// <summary>
        /// 碰撞触发 colliderScale<碰撞盒半径时表示碰撞 否则为靠近
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnCollider(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData);

        /// <summary>
        /// 死亡触发
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnBeDie(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData);

        /// <summary>
        /// 受伤后触发
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnBeHurt(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData);


        public void OnUpdate(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData);

        /// <summary>
        /// 一开始执行 只执行一次
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnRandom(ref GameEventData_ReadWrite refData, in GameEventData_ReadOnly inData);
    }

    //[InternalBufferCapacity(0)]
    public partial struct GameEvent : IBufferElementData
    {
    }


    /// <summary>
    ///事件触发类型
    /// </summary>
    public enum GameEventTriggerType
    {
        ImmediateEffect = 0,
        AppointedTime = 1,
        IntervalTime = 2,
        IsEnvTrigger=3
    }

    public struct GameEventData_ReadWrite
    {
        public ComponentLookup<PhysicsVelocity> cdfePhysicsVelocity;
        public BufferLookup<Buff> cdfeBuffs;
        public BufferLookup<Skill> cdfeSkills;
        public BufferLookup<Trigger> cdfeTriggers;
        public EntityCommandBuffer.ParallelWriter ecb;
        public ComponentLookup<GameEnviromentData> cdfeEnviromentData;
        public ComponentLookup<GameTimeData> cdfeGameTimeData;
        public ComponentLookup<PlayerData> cdfePlayerData;
        public ComponentLookup<ChaStats> cdfeChaStats;
        public GameRandomData randomData;
        public ComponentLookup<PhysicsMass> cdfePhysicsMass;
        public DynamicBuffer<GameEvent> cdfeGameEvent;
        public EntityStorageInfoLookup StorageInfoLookup;
    }

    public struct GameEventData_ReadOnly
    {
        public Entity wbe;
        public int sortKey;
        public GlobalConfigData config;
        public NativeArray<Entity> entities;
        public Entity selefEntity;
        public PrefabMapData prefabMapData;
        public GameOthersData gameOthersData;
        public GameTimeData gameTimeData;
        public PlayerData playerData;
        public Entity player;
        public float fDT;
        public int timeTick;
        public ComponentLookup<LocalTransform> cdfeLocalTransform;
        public ComponentLookup<WeaponData> cdfeWeaponData;
        public EntityStorageInfoLookup storageInfoFromEntity;
        public ComponentLookup<EnemyData> cdfeEnemyData;
        public float3 currentMapStartPos;
        public ComponentLookup<MapElementData> cdfeMapElementData;
        public NativeArray<Entity> mapModels;
        public ComponentLookup<TargetData> cdfeTargetData;
        public ComponentLookup<AgentLocomotion> cdfeAgentLocomotion;
        public GameCameraSizeData gameCameraSizeData;
        public GameRandomData gameRandomData;
        public ComponentLookup<TimeToDieData> cdfeTimeToDieData;
        public ComponentLookup<PostTransformMatrix> cdfePostTransformMatrix;
    }
}