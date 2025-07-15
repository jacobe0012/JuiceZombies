//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-07-18 10:13:45
//---------------------------------------------------------------------

using FMOD.Studio;
using Rewired;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Main
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial class HandleInputSystem : SystemBase
    {
        private int playerId = 0;

        private Player player;

        //EventInstance eventInstance;

        protected override void OnCreate()
        {
            RequireForUpdate<WorldBlackBoardTag>();
            RequireForUpdate<InputData>();
        }

        protected override void OnStartRunning()
        {
            player = ReInput.players.GetPlayer(playerId);
        }


        protected override void OnUpdate()
        {
            var wbe = SystemAPI.GetSingletonEntity<WorldBlackBoardTag>();
            var globalConfigData = SystemAPI.GetComponent<GlobalConfigData>(wbe);
            var gameOthersData = SystemAPI.GetComponent<GameOthersData>(wbe);
            float2 moveRawVector = player.GetAxis2D("MoveHorizontal", "MoveVertical");
            float2 movenNormalize = math.length(moveRawVector) < math.EPSILON ? 0 : math.normalizesafe(moveRawVector);


            Entities
                .WithBurst().ForEach((ref InputData inputs) => { inputs.Move = movenNormalize; }).Schedule();

            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     UnityHelper.TryCreateAudioClip(globalConfigData, gameOthersData, 0, out var instance);
            //     eventInstance = instance;
            // }
            //
            // if (Input.GetKeyDown(KeyCode.P))
            // {
            //     UnityHelper.DestroyAudioClip(eventInstance);
            // }
        }
    }
}