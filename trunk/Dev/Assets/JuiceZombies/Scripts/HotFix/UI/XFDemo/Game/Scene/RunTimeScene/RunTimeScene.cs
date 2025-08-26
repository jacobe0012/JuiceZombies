using System.Collections.Generic;
using cfg.blobstruct;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using HotFix_UI;
using Main;
using ProjectDawn.Navigation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace XFramework
{
    public class RunTimeScene : Scene
    {
        protected override void OnCompleted()
        {
            Log.Debug($"进入RunTime场景");
            UnicornSceneHelper.InitShader();
            InitRunTimeScene().Forget();
            ResourcesSingletonOld.Instance.FromRunTimeScene = true;
        }
        
        async UniTaskVoid InitRunTimeScene()
        {
            Debug.Log($"InitRunTimeScene");
            await InitInputPrefab();
            // 创建黑板实体
            int levelId = ResourcesSingletonOld.Instance.levelInfo.levelId;
            int sceneId = ConfigManager.Instance.Tables.Tblevel.Get(levelId).sceneId;
            UnicornUIHelper.CreateBlackBoardEntity(sceneId);
            UnicornUIHelper.InitSystem();

            AudioManager.Instance.ClearFModBgmAudio();
            AudioManager.Instance.PlayFModAudio(2103);
            var hud = await UIHelper.CreateAsync(UIType.UIPanel_RunTimeHUD);
            hud.GetRectTransform().SetAnchoredPosition(Vector2.zero);
        }

        // private void InitMap()
        // {
        //     // var initSysGroup =
        //     //     World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<InitializationSystemGroup>();
        //
        //
        //     //初始化地图
        //     // var initMapSystem =
        //     //     World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InitMapSystem>();
        //     // ref var state = ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(initMapSystem);
        //     // state.Enabled = true;
        //     //SceneSystem
        //
        //
        //     //TODO:删掉
        //     // var SpawnEnemySystem =
        //     //     World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<GameEventHandleSystem>();
        //     // ref var state0 =
        //     //     ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(SpawnEnemySystem);
        //     //state0.Enabled = true;
        //     // var HitBackAfterSystem =
        //     //     World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<HitBackAfterSystem>();
        //     // ref var state1 =
        //     //     ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(HitBackAfterSystem);
        //     // state1.Enabled = false;
        //
        //     //  var ConsumeDamageInfoSystem =
        //     //      World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConsumeDamageInfoSystem>();
        //     //  ref var state2 =
        //     //      ref World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(ConsumeDamageInfoSystem);
        //     // state2.Enabled = false;
        //     // initSysGroup.AddSystemToUpdateList(initMapSystem);
        //     // var state = World.DefaultGameObjectInjectionWorld.EntityManager.WorldUnmanaged
        //     //     .GetExistingSystemState<InitMapSystem>();
        // }

        /// <summary>
        /// 输入及FPS显示
        /// </summary>
        private async UniTask InitInputPrefab()
        {
            var task0 = await ResourcesManager.InstantiateAsync("Rewired Input Manager");

            //await task0;

            GameObject.DontDestroyOnLoad(task0);

            Debug.Log($"[{GetType().FullName}] {task0.name}InitInputPrefab!");
        }


        protected override void OnDestroy()
        {
            Log.Debug("离开RunTimeScene场景");
            //DestoryWorldBlackBoardEntity();
            base.OnDestroy();
        }

        // public void DestoryWorldBlackBoardEntity()
        // {
        //     var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //     var entityQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag));
        //     var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
        //     var crowdQuery = entityManager.CreateEntityQuery(typeof(CrowdSurface));
        //
        //
        //     if (entityQuery.IsEmpty) return;
        //     var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
        //     var playerData = entityManager.GetComponentData<PlayerData>(player);
        //     playerData.playerOtherData.propsHashMap.Dispose();
        //
        //     var e = entityQuery.ToEntityArray(Allocator.Temp)[0];
        //     var gameOthersData = entityManager.GetComponentData<GameOthersData>(e);
        //     //gameOthersData.animations.Dispose();
        //     gameOthersData.allAudioClips.Dispose();
        //
        //     Debug.LogError($"crowdQuery1 {crowdQuery.IsEmpty}");
        //     if (!crowdQuery.IsEmpty)
        //     {
        //         Debug.LogError($"crowdQuery {crowdQuery.CalculateEntityCount()}");
        //         entityManager.DestroyEntity(crowdQuery);
        //     }
        //
        //     entityManager.DestroyEntity(entityQuery);
        // }
    }
}