//---------------------------------------------------------------------
// UnicornStudio
// Author: 迅捷蟹
// Time: 2023-07-28 15:58:28
//---------------------------------------------------------------------


using Unity.Entities;
using UnityEngine;

namespace Main
{
    [DisallowMultipleComponent]
    public class PlayerStateMachineAuthoring : MonoBehaviour
    {
        class PlayerStateMachineBaker : Baker<PlayerStateMachineAuthoring>
        {
            public override void Bake(PlayerStateMachineAuthoring authoring)
            {
                DynamicBuffer<State> statesBuffer = AddBuffer<State>();

                // Add states in their polymorphic form to the buffer.
                // Each state's index in the buffer will correspond to the int value of their "TypeId" enum
                // authoring.AddStateToStatesBuffer(new PlayerMove()
                // {
                //     animId = Animator.StringToHash("Boss_3001_Move"), "Boss"+_COmponment_id+"_Move"
                //     timeElapsed = 0,
                //     timeScale = 1,
                //     duration = 1f,
                //     tick = 0
                // }.ToState(), ref statesBuffer);
                authoring.AddStateToStatesBuffer(new PlayerMove
                {
                    timeScale = 1,
                    duration = 1f,
                    stateId = 0
                }.ToState(), ref statesBuffer);
                authoring.AddStateToStatesBuffer(new PlayerGetHit()
                {
                    stateId = 1,
                    timeElapsed = 0,
                    timeScale = 1,
                    duration = 0.4f,
                    tick = 0
                }.ToState(), ref statesBuffer);
                authoring.AddStateToStatesBuffer(new PlayerIdle()
                {
                    stateId = 2,
                    timeElapsed = 0,
                    timeScale = 1,
                    duration = 1f,
                    tick = 0
                }.ToState(), ref statesBuffer);
                authoring.AddStateToStatesBuffer(new PlayerDie()
                {
                    stateId = 3,
                    timeElapsed = 0,
                    timeScale = 1,
                    duration = 10f,
                    tick = 0
                }.ToState(), ref statesBuffer);
                authoring.AddStateToStatesBuffer(new PlayerBeController()
                {
                    stateId = 4,
                    timeElapsed = 0,
                    timeScale = 1,
                    duration = 10f,
                    tick = 0
                }.ToState(), ref statesBuffer);

                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new StateMachine
                {
                    currentState = statesBuffer[0],
                    speed = 0,
                    startTranslation = default,
                    isInitialized = false,
                    transitionToStateIndex = -1,
                    curAnim = AnimationEnum.Idle,
                    lastAnim = AnimationEnum.Idle,
                    animSpeedScale = 1
                });
            }
        }

        private void AddStateToStatesBuffer(State state, ref DynamicBuffer<State> statesBuffer)
        {
            //Debug.LogError($"length{statesBuffer.Length} state {state.CurrentTypeId}");
            state.Int32_0 = statesBuffer.Length;
            statesBuffer.Add(state);

            // int stateIndex = (int)state.CurrentTypeId;
            //
            // //Resize buffer if needed
            // if (stateIndex >= statesBuffer.Length)
            // {
            //     int stateTypesCount = Enum.GetValues(typeof(State.TypeId)).Length;
            //     for (int i = statesBuffer.Length; i < stateTypesCount; i++)
            //     {
            //         statesBuffer.Add(default);
            //     }
            // }
            //
            // statesBuffer[stateIndex] = state;
        }
    }
}