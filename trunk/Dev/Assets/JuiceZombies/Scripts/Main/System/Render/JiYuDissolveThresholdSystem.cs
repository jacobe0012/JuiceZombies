//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-08-11 10:30:51
//---------------------------------------------------------------------

using GPUECSAnimationBaker.Engine.AnimatorSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Main
{

    //[WorldSystemFilter(WorldSystemFilterFlags.Editor, WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    //[UpdateAfter(typeof(StateMachineSystem))]
    public partial struct JiYuDissolveThresholdSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
#if !UNITY_EDITOR
            state.RequireForUpdate<WorldBlackBoardTag>();
#endif
            state.RequireForUpdate<WorldBlackBoardTag>();
            state.RequireForUpdate<JiYuDissolveThreshold>();
            //state.RequireForUpdate(SystemAPI.QueryBuilder().WithAll<StateMachine, AnimatorAspect>().Build());
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();

            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(wbe);
            var gameSetUpData = SystemAPI.GetComponent<GameSetUpData>(wbe);
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var gameRandomData = SystemAPI.GetComponent<GameRandomData>(wbe);
            var gameTimeData = SystemAPI.GetComponent<GameTimeData>(wbe);
            var gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe);
            new JiYuDissolveThresholdJob
            {
                //gameTimeData = gameTimeData,
                dT = SystemAPI.Time.DeltaTime,
                gameOthersData = gameOthersData
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct JiYuDissolveThresholdJob : IJobEntity
        {
            [ReadOnly] public float dT;
            [ReadOnly] public GameOthersData gameOthersData;
            //[ReadOnly] public GameTimeData gameTimeData;

            private void Execute(ref JiYuDissolveThreshold jiYuDissolveThreshold)
            {
                jiYuDissolveThreshold.value += gameOthersData.gameOtherParas.dissolveSpeed * dT;
                //color.value.w -= gameOthersData.gameOtherParas.alphaSpeed * dT;
            }
        }
    }
}