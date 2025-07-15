using Main;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using XFramework;

namespace HotFix_UI
{
    /// <summary>
    /// 注意：热更中foreach不能用WithStructalChanges(失效)
    /// </summary>
    //[DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class UpdatePlayerGOSystem : SystemBase
    {
        //private GameObject playerGO;
        //private Transform playerTran;

        protected override void OnCreate()
        {
            //Enabled = false;
            //RequireForUpdate<PlayerData>();
            RequireForUpdate<WorldBlackBoardTag>();
        }

        protected override void OnStartRunning()
        {
            
        }

        protected override void OnUpdate()
        {
            var cdfeTran = SystemAPI.GetComponentLookup<LocalTransform>(true);
            var strogInfo = SystemAPI.GetEntityStorageInfoLookup();
            
            foreach (var (gameOthersData, e) in SystemAPI
                         .Query<RefRO<GameOthersData>>()
                         .WithEntityAccess())
            {
                if (strogInfo.Exists(gameOthersData.ValueRO.CameraTarget))
                {
                    if (cdfeTran.HasComponent(gameOthersData.ValueRO.CameraTarget))
                    {
                        var global = XFramework.Common.Instance.Get<Global>();
                        global.GameObjects.TargetCamera.transform.position =
                            cdfeTran[gameOthersData.ValueRO.CameraTarget].Position;
                    }
                }
            }
        }
    }
}