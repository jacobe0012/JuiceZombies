//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-07-17 12:11:46
//---------------------------------------------------------------------


using ProjectDawn.Navigation;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Main
{
    public enum TriggerConditionType
    {
        /// <summary>
        ///0.无条件
        /// </summary>
        None = 0,

        /// <summary>
        /// 1.效果id(来源
        /// </summary>
        SkillEffectIdFrom = 1,

        /// <summary>
        /// 2.效果id(目标 用buff
        /// </summary>
        SkillEffectIdTo = 2,

        /// <summary>
        /// 3. 属性id
        /// </summary>
        PropertyId = 3,

        /// <summary>
        ///41.击杀目标
        /// </summary>
        KillEnemy = 41,

        /// <summary>
        /// 42 受伤时
        /// </summary>
        OnBeHert = 42,

        /// <summary>
        ///43 闪避时
        /// </summary>
        Dodging = 43,

        /// <summary>
        ///44 移动时
        /// </summary>
        Moving = 44,

        /// <summary>
        ///45 复活
        /// </summary>
        AfterRebirth = 45,
        /// <summary>
        ///45 拾取道具
        /// </summary>
        PickUpProp = 46,
        //47 死亡
        Deading=47



    }

    public enum TriggerType
    {
        /// <summary>
        /// 1.每次数触发
        /// </summary>
        PerNum = 1,

        /// <summary>
        /// 2.光环类
        /// </summary>
        Halo = 2,

        /// <summary>
        /// 3.武器攻击
        /// </summary>
        WeaponAttack,

        /// <summary>
        /// 4.每时间触发
        /// </summary>
        PerTime = 4,
    }

    //[InternalBufferCapacity(0)]
    public partial struct Trigger : IBufferElementData
    {
    }

    [PolymorphicStruct]
    public interface ITrigger
    {
        public void OnUpdate(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData);

        /// <summary>
        /// 触发时回调 触发完isTrigger =false
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnTrigger(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData);

        /// <summary>
        /// trigger tick不归零 start为第一帧执行 仅执行一次
        /// </summary>
        /// <param name="refData"></param>
        /// <param name="inData"></param>
        public void OnStart(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData);
        // public void OnOccur(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData);
        // public void OnRemoved(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData);
        // public void OnTick(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData);
        // public void OnCast(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData);
        // public void OnHit(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData);
        // public void OnBeHurt(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData);
        // public void OnKill(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData);
        // public void OnBeforeBeKilled(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData);
        // public void OnBeKilled(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData);
        //
        // public void OnPerUnitMove(ref TriggerData_ReadWrite refData, in TriggerData_ReadOnly inData);
        // //public void On(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);
        //
        // /// <summary>
        // /// 升级时回调点,目前玩家专用
        // /// </summary>
        // /// <param name="refData"></param>
        // /// <param name="inData"></param>
        // public void OnLevelUp(ref BuffOldData_ReadWrite refData, in BuffOldData_ReadOnly inData);
    }

    public struct TriggerData_ReadWrite
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
        public BufferLookup<Skill> cdfeSkill;
        public int subDamage;
        public ComponentLookup<PhysicsVelocity> cdfePhysicsVolocity;
    }

    public struct TriggerData_ReadOnly
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
        public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;
        public float fdT;
    }
}