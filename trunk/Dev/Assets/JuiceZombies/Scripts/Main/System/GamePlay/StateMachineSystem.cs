//---------------------------------------------------------------------
// UnicornStudio
// Author: 迅捷蟹
// Time: 2023-07-28 15:58:25
//---------------------------------------------------------------------


using GPUECSAnimationBaker.Engine.AnimatorSystem;
using ProjectDawn.Navigation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

//using NSprites;


namespace Main
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial struct StateMachineSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<State, StateMachine>().Build());
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

            var singleton = SystemAPI
                .GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();

            var ecb = singleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();


            var canHurtQuery = SystemAPI.QueryBuilder().WithAll<ChaStats>().Build();
            //var stateQuery = SystemAPI.QueryBuilder().WithAll<State, StateMachine>().Build();


            new StateMachineJob
            {
                timeTick = (uint)(SystemAPI.Time.ElapsedTime * 1000f),
                eT = (float)SystemAPI.Time.ElapsedTime,
                fdT = SystemAPI.Time.fixedDeltaTime,
                bfeChild = SystemAPI.GetBufferLookup<Child>(),
                prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe),
                gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe),
                configData = SystemAPI.GetComponent<GlobalConfigData>(wbe),
                gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe),
                gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe),
                gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe),
                cdfeChaStats = SystemAPI.GetComponentLookup<ChaStats>(),
                bfeDropsBuffer = SystemAPI.GetBufferLookup<DropsBuffer>(true),
                bfeBuff = SystemAPI.GetBufferLookup<Buff>(true),
                bfeSkill = SystemAPI.GetBufferLookup<Skill>(),
                cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(),
                cdfeAgentShape = SystemAPI.GetComponentLookup<AgentShape>(),
                ecb = ecb,
                cdfePlayerData = SystemAPI.GetComponentLookup<PlayerData>(),
                cdfePhysicsVelocity = SystemAPI.GetComponentLookup<PhysicsVelocity>(),
                cdfeFlip = SystemAPI.GetComponentLookup<JiYuFlip>(),
                cdfePhysicsDamping = SystemAPI.GetComponentLookup<PhysicsDamping>(),
                cdfePhysicsCollider = SystemAPI.GetComponentLookup<PhysicsCollider>(),
                cdfeAgentLocomotion = SystemAPI.GetComponentLookup<AgentLocomotion>(),
                cdfeWeaponData = SystemAPI.GetComponentLookup<WeaponData>(),
                cdfeEnemyData = SystemAPI.GetComponentLookup<EnemyData>(),
                cdfeGpuEcsAnimatorControlComponent = SystemAPI.GetComponentLookup<GpuEcsAnimatorControlComponent>(),
                cdfeHitBackData = SystemAPI.GetComponentLookup<HitBackData>(true),
                cdfeTargetData = SystemAPI.GetComponentLookup<TargetData>(true),
                cdfeInputData = SystemAPI.GetComponentLookup<InputData>(true),
                cdfeSpiritData = SystemAPI.GetComponentLookup<SpiritData>(true),
                cdfeTimeToDieData = SystemAPI.GetComponentLookup<TimeToDieData>(true),
                cdfeObstacleTag = SystemAPI.GetComponentLookup<ObstacleTag>(true),
                cdfeJiYuSort = SystemAPI.GetComponentLookup<JiYuSort>(true),
                bfeGameEvent = SystemAPI.GetBufferLookup<GameEvent>(true),
                bfeLinkedEntityGroup = SystemAPI.GetBufferLookup<LinkedEntityGroup>(true),
                storageInfoFromEntity = SystemAPI.GetEntityStorageInfoLookup(),
                allEntities = canHurtQuery.ToEntityArray(Allocator.TempJob),
                player = SystemAPI.GetSingletonEntity<PlayerData>(),
                wbe = wbe,
                mapModules =
                    SystemAPI.QueryBuilder()
                        .WithAny<ObstacleTag, AreaTag>()
                        .Build()
                        .ToEntityArray(Allocator.TempJob),
                cdfeMapElementData = SystemAPI.GetComponentLookup<MapElementData>(true),
                cdfeEntityGroupData = SystemAPI.GetComponentLookup<EntityGroupData>(true),
                cdfePhysicsMass = SystemAPI.GetComponentLookup<PhysicsMass>(),
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct StateMachineJob : IJobEntity
    {
        [ReadOnly] public uint timeTick;
        [ReadOnly] public float fdT;
        [ReadOnly] public float eT;
        [ReadOnly] public PrefabMapData prefabMapData;
        [ReadOnly] public GameSetUpData gameSetUpData;
        [ReadOnly] public GlobalConfigData configData;
        [ReadOnly] public GameOthersData gameOthersData;
        [ReadOnly] public GameTimeData gameTimeData;

         public GameRandomData gameRandomData;

        [ReadOnly] public BufferLookup<DropsBuffer> bfeDropsBuffer;
        [ReadOnly] public BufferLookup<Buff> bfeBuff;
        [NativeDisableParallelForRestriction] public BufferLookup<Child> bfeChild;
        [NativeDisableParallelForRestriction] public BufferLookup<Skill> bfeSkill;

        [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> cdfeLocalTransform;

        [NativeDisableParallelForRestriction] public ComponentLookup<AgentShape> cdfeAgentShape;
        [NativeDisableParallelForRestriction] public ComponentLookup<ChaStats> cdfeChaStats;

        [NativeDisableParallelForRestriction] public ComponentLookup<PlayerData> cdfePlayerData;

        [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsVelocity> cdfePhysicsVelocity;

        [NativeDisableParallelForRestriction] public ComponentLookup<JiYuFlip> cdfeFlip;

        [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsDamping> cdfePhysicsDamping;

        [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsCollider> cdfePhysicsCollider;

        [NativeDisableParallelForRestriction] public ComponentLookup<AgentLocomotion> cdfeAgentLocomotion;

        //[ReadOnly] public BufferLookup<LinkedEntityGroup> bfeChild;
        [NativeDisableParallelForRestriction] public ComponentLookup<WeaponData> cdfeWeaponData;
        [NativeDisableParallelForRestriction] public ComponentLookup<EnemyData> cdfeEnemyData;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<GpuEcsAnimatorControlComponent> cdfeGpuEcsAnimatorControlComponent;

        [NativeDisableParallelForRestriction] public ComponentLookup<PhysicsMass> cdfePhysicsMass;
        [ReadOnly] public ComponentLookup<HitBackData> cdfeHitBackData;
        [ReadOnly] public ComponentLookup<TargetData> cdfeTargetData;
        [ReadOnly] public ComponentLookup<InputData> cdfeInputData;
        [ReadOnly] public ComponentLookup<SpiritData> cdfeSpiritData;
        [ReadOnly] public ComponentLookup<MapElementData> cdfeMapElementData;
        [ReadOnly] public ComponentLookup<TimeToDieData> cdfeTimeToDieData;
        [ReadOnly] public ComponentLookup<ObstacleTag> cdfeObstacleTag;
        [ReadOnly] public ComponentLookup<JiYuSort> cdfeJiYuSort;
        [ReadOnly] public ComponentLookup<EntityGroupData> cdfeEntityGroupData;
        [ReadOnly] public BufferLookup<GameEvent> bfeGameEvent;
        [ReadOnly] public BufferLookup<LinkedEntityGroup> bfeLinkedEntityGroup;

        public EntityCommandBuffer.ParallelWriter ecb;

        public EntityStorageInfoLookup storageInfoFromEntity;

        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> allEntities;
        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> mapModules;
        [ReadOnly] public Entity player;
        [ReadOnly] public Entity wbe;


        public void Execute([EntityIndexInQuery] int index, Entity entity, ref DynamicBuffer<State> statesBuffer,
            ref StateMachine stateMachine, ref AgentBody agentBody)
        {
            // var inputs = batchInChunk.GetNativeArray(cthinputs);
            // var weaponDatas = batchInChunk.GetNativeArray(cthweaponData);

            // var chaStats = chunk.GetNativeArray(cthchaStats);
            // var entities = chunk.GetNativeArray(cthEntity);
            // var stateMachines = chunk.GetNativeArray(cthStateMachine);
            //
            // var states = chunk.GetBufferAccessor(bthState);
            // var agentBodys = chunk.GetNativeArray(cthAgentBody);


            // var chaStat = chaStats[i];
            // var entity = entities[i];
            // var stateMachine = stateMachines[i];
            // var statesBuffer = states[i];
            // var agentBody = agentBodys[i];
            // Prepare state update data

            StateUpdateData_ReadWrite refData = new StateUpdateData_ReadWrite
            {
                stateMachine = stateMachine,

                chaStats = cdfeChaStats.HasComponent(entity)
                    ? cdfeChaStats[entity]
                    : default,
                ecb = ecb,
                storageInfoFromEntity = storageInfoFromEntity,
                agentBody = agentBody,
                agentShape = cdfeAgentShape.HasComponent(entity)
                    ? cdfeAgentShape[entity]
                    : default,
                localTransform = cdfeLocalTransform.HasComponent(entity)
                    ? cdfeLocalTransform[entity]
                    : default,
                rand = gameRandomData.rand,
                physicsVelocity = cdfePhysicsVelocity.HasComponent(entity)
                    ? cdfePhysicsVelocity[entity]
                    : default,
                cdfeFlip = cdfeFlip,
                Skills = bfeSkill.HasComponent(entity)
                    ? bfeSkill[entity]
                    : default,
                physicsDamping = cdfePhysicsDamping.HasComponent(entity)
                    ? cdfePhysicsDamping[entity]
                    : default,
                physicsCollider = cdfePhysicsCollider.HasComponent(entity)
                    ? cdfePhysicsCollider[entity]
                    : default,
                cdfePlayerData = cdfePlayerData,
                cdfePhysicsVelocity = cdfePhysicsVelocity,
                cdfePhysicsCollider = cdfePhysicsCollider,
                playerData = cdfePlayerData.HasComponent(entity)
                    ? cdfePlayerData[entity]
                    : default,
                enemyData = cdfeEnemyData.HasComponent(entity)
                    ? cdfeEnemyData[entity]
                    : default,
                cdfeWeaponData = cdfeWeaponData,
                animatorData = cdfeGpuEcsAnimatorControlComponent.HasComponent(entity)
                    ? cdfeGpuEcsAnimatorControlComponent[entity]
                    : default,
                AgentLocomotion = cdfeAgentLocomotion.HasComponent(entity)
                    ? cdfeAgentLocomotion[entity]
                    : default,
                bfeChild = bfeChild.HasComponent(entity)
                    ? bfeChild[entity]
                    : default,
                cdfePhysicsMass = cdfePhysicsMass
            };
            StateUpdateData_ReadOnly inData = new StateUpdateData_ReadOnly
            {
                timeTick = timeTick,
                eT = eT,
                fdT = fdT,
                entity = entity,
                player = player,
                wbe = wbe,
                sortkey = index,
                inputs = cdfeInputData.HasComponent(entity)
                    ? cdfeInputData[entity]
                    : default,
                prefabMapData = prefabMapData,
                gameSetUpData = gameSetUpData,
                gameOthersData = gameOthersData,
                gameTimeData = gameTimeData,
                configData = configData,
                dropsBuffer = bfeDropsBuffer.HasComponent(entity)
                    ? bfeDropsBuffer[entity]
                    : default,
                cdfeLocalTransform = cdfeLocalTransform,
                allEntities = allEntities,
                cdfeChaStats = cdfeChaStats,
                cdfeEnemyData = cdfeEnemyData,
                cdfeTargetData = cdfeTargetData,
                cdfeHitBackData = cdfeHitBackData,
                cdfeTimeToDieData = cdfeTimeToDieData,
                cdfeSpiritData = cdfeSpiritData,
                cdfeObstacleTag = cdfeObstacleTag,
                states = statesBuffer,
                mapModules = mapModules,
                cdfeMapElementData = cdfeMapElementData,
                bfeBuff = bfeBuff,
                bfeGameEvent = bfeGameEvent,
                cdfeJiYuSort = cdfeJiYuSort,
                cdfeEntityGroupData = cdfeEntityGroupData,
                bfeLinkedEntityGroup = bfeLinkedEntityGroup,
                gameRandomData = gameRandomData
            };

            // Handle entering the first state
            if (!refData.stateMachine.isInitialized)
            {
                //TODO：动画
                if (cdfeGpuEcsAnimatorControlComponent.HasComponent(entity))
                {
                    var temp = cdfeGpuEcsAnimatorControlComponent[entity];
                    temp.animatorInfo.blendFactor = 0.5f;
                    cdfeGpuEcsAnimatorControlComponent[entity] = temp;
                }


                refData.stateMachine.isInitialized = true;

                refData.stateMachine.currentState.Single_2 =
                    refData.chaStats.chaResource.actionSpeed < math.EPSILON
                        ? 1f
                        : refData.chaStats.chaResource.actionSpeed;

                refData.stateMachine.currentState.OnStateEnter(ref refData, in inData);
            }


            // State update
            refData.stateMachine.currentState.OnStateUpdate(ref refData, in inData);
            refData.stateMachine.currentState.Int32_4++;
            refData.stateMachine.currentState.Single_1 += inData.fdT * refData.stateMachine.currentState.Single_2;

            // Handle Transitions
            if (refData.stateMachine.transitionToStateIndex >= 0)
            {
                // Exit current state
                refData.stateMachine.currentState.Single_1 = 0;
                refData.stateMachine.currentState.Int32_4 = 0;
                refData.stateMachine.currentState.OnStateExit(ref refData, in inData);
                //Debug.Log($"entity {entity.Index}");
                // Write current state data back into states buffer
                statesBuffer[refData.stateMachine.currentState.Int32_0] =
                    refData.stateMachine.currentState;

                // Enter next state

                refData.stateMachine.currentState = statesBuffer[refData.stateMachine.transitionToStateIndex];

                refData.stateMachine.currentState.Single_2 =
                    refData.chaStats.chaResource.actionSpeed < math.EPSILON
                        ? 1f
                        : refData.chaStats.chaResource.actionSpeed;

                //refData.animatorData.animatorInfo.animationID = refData.stateMachine.currentState.Int32_0;
                refData.stateMachine.transitionToStateIndex = -1;
                refData.stateMachine.currentState.OnStateEnter(ref refData, in inData);
            }

            // Write back data
            if (cdfeChaStats.HasComponent(entity))
            {
                cdfeChaStats[entity] = refData.chaStats;
            }

            if (cdfeLocalTransform.HasComponent(entity))
            {
                cdfeLocalTransform[entity] = refData.localTransform;
            }

            if (cdfePhysicsVelocity.HasComponent(entity))
            {
                cdfePhysicsVelocity[entity] = refData.physicsVelocity;
            }

            if (cdfeEnemyData.HasComponent(entity))
            {
                cdfeEnemyData[entity] = refData.enemyData;
            }

            if (cdfeAgentShape.HasComponent(entity))
            {
                cdfeAgentShape[entity] = refData.agentShape;
            }

            // if (cdfeFlip.HasComponent(entity))
            // {
            //     cdfeFlip[entity] = refData.flip;
            // }

            if (cdfePhysicsDamping.HasComponent(entity))
            {
                cdfePhysicsDamping[entity] = refData.physicsDamping;
            }

            if (cdfePhysicsCollider.HasComponent(entity))
            {
                cdfePhysicsCollider[entity] = refData.physicsCollider;
            }

            if (cdfePlayerData.HasComponent(entity))
            {
                cdfePlayerData[entity] = refData.playerData;
            }

            if (cdfeAgentLocomotion.HasComponent(entity))
            {
                cdfeAgentLocomotion[entity] = refData.AgentLocomotion;
            }

            cdfeFlip = refData.cdfeFlip;
            gameRandomData.rand = refData.rand;
            //Debug.Log($"{gameRandomData.rand.state}");
            ecb.SetComponent(index, wbe, gameRandomData);

            stateMachine = refData.stateMachine;

            agentBody = refData.agentBody;


            // stateMachines[i] = refData.stateMachine;
            //
            // agentBodys[i] = refData.agentBody;
        }
    }
}