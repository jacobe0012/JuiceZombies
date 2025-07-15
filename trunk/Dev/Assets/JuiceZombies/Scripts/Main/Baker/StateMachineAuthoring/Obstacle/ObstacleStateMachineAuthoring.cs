//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-08-21 18:48:28
//---------------------------------------------------------------------


using Unity.Entities;
using UnityEngine;

namespace Main
{
    /// <summary>
    /// 弃用 通过UnityHelper的AddStateToStatesBuffer添加
    /// </summary>
    [DisallowMultipleComponent]
    public class ObstacleStateMachineAuthoring : MonoBehaviour
    {
        class ObstacleStateMachineAuthoringBaker : Baker<ObstacleStateMachineAuthoring>
        {
            public override void Bake(ObstacleStateMachineAuthoring authoring)
            {
                DynamicBuffer<State> statesBuffer = AddBuffer<State>();

                // Add states in their polymorphic form to the buffer.
                // Each state's index in the buffer will correspond to the int value of their "TypeId" enum
                authoring.AddStateToStatesBuffer(new ObstacleIdle()
                {
                    stateId = 0,
                    timeElapsed = 0,
                    timeScale = 1,
                    duration = 5f,
                    tick = 0,
                }.ToState(), ref statesBuffer);

                authoring.AddStateToStatesBuffer(new ObstacleGetHit()
                {
                    stateId = 1,
                    timeElapsed = 0,
                    timeScale = 1,
                    duration = 2f,
                    tick = 0
                }.ToState(), ref statesBuffer);

                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new StateMachine
                {
                    currentState = statesBuffer[0],
                    speed = 0,
                    startTranslation = default,
                    isInitialized = false,
                    transitionToStateIndex = -1,
                    curAnim = 0,
                    lastAnim = 0,
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