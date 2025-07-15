using Cysharp.Threading.Tasks;
using Main;
using System;
using System.Collections.Generic;
using System.Linq;
using cfg.config;
using ProjectDawn.Navigation;
using Spine.Unity;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using XFramework;
using UnityEngine.UIElements;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using Scene = XFramework.Scene;

namespace HotFix_UI
{
    /// <summary>
    /// 注意：热更中foreach不能用WithStructalChanges(失效)
    /// </summary>
    [DisableAutoCreation]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderLast = true)]
    public partial class HybridEventSystem : SystemBase
    {
        EntityQuery bossQuery;

        protected override void OnCreate()
        {
            RequireForUpdate<HybridEventData>();
            RequireForUpdate<PlayerData>();
            bossQuery = SystemAPI.QueryBuilder().WithAll<BossTag>().Build();
        }


        protected override void OnUpdate()
        {
            var cdfeLocalTransform = SystemAPI.GetComponentLookup<LocalTransform>(true);
            var player = SystemAPI.GetSingletonEntity<PlayerData>();

            //Debug.Log($"11");
            Entities.WithoutBurst().WithStructuralChanges().ForEach(
                (Entity e, ref HybridEventData hybridEvnetData) =>
                {
                    if (hybridEvnetData.disAble)
                        return;
                    //Debug.Log($"UpdateMapColliderPosTag1");
                    if (hybridEvnetData.type == 1)
                    {
                        Debug.Log($"UpdateMapColliderPos");
                        var tran = cdfeLocalTransform[player];
                        var global = XFramework.Common.Instance.Get<Global>();
                        var temp = global.CameraBounds.position;
                        temp.y = tran.Position.y;
                        global.CameraBounds.position = temp;
                    }
                    else if (hybridEvnetData.type == 2)
                    {
                        SwitchBossScene(hybridEvnetData);
                    }
                    else if (hybridEvnetData.type == 12)
                    {
                        SpawnHybridSpine(hybridEvnetData);
                    }
                    else if (hybridEvnetData.type == 13)
                    {
                        RecycleHybridSpine(hybridEvnetData);
                    }
                    else if (hybridEvnetData.type == 3)
                    {
                        var spineHybridData =
                            EntityManager.GetComponentData<SpineHybridData>(hybridEvnetData.bossEntity);

                        for (int i = 0; i < spineHybridData.go.Value.transform.childCount; i++)
                        {
                            var child = spineHybridData.go.Value.transform.GetChild(i);
                            Debug.Log($"child.name{child.name}");
                            var chileName = child.name;
                            if (chileName.Contains(hybridEvnetData.strArgs.Value))
                            {
                                Debug.Log($"chileName.Containschild.name{child.name} {hybridEvnetData.args.x}");
                                var scale = hybridEvnetData.args.y / 10000f;
                                var trailRenderer = child.GetComponent<TrailRenderer>();
                                // trailRenderer.startWidth = scale;
                                // trailRenderer.endWidth = scale;

                                child.gameObject.SetActive(hybridEvnetData.args.x == 1);
                                break;
                            }
                        }
                    }
                    else if (hybridEvnetData.type == 4)
                    {
                        switch ((int)hybridEvnetData.args.x)
                        {
                            case 1:
                                OnPlayerDieEvent();
                                break;
                            case 2:
                                OnBossDieEvent();
                                break;
                        }
                    }
                    else if (hybridEvnetData.type == 33)
                    {
                        //TODO:
                        var global = XFramework.Common.Instance.Get<Global>();
                        global.DoCameraFOV(hybridEvnetData.args.x, true);
                    }
                    else if (hybridEvnetData.type == 31)
                    {
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
                        {
                            //JiYuUIHelper.StartStopTime(false);
                            Debug.Log($"uiRuntime {hybridEvnetData.args.x}");
                            UIHelper.CreateAsync(UIType.UIPanel_BattleShop, (int)hybridEvnetData.args.x).Forget();
                        }
                    }
                    else if (hybridEvnetData.type == 32)
                    {
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
                        {
                            //JiYuUIHelper.StartStopTime(false);
                            Debug.Log($"UIPanel_BattleTecnology {hybridEvnetData.args.x}");
                            UIHelper.CreateAsync(UIType.UIPanel_BattleTecnology, (int)hybridEvnetData.args.x).Forget();
                        }
                    }
                    else if (hybridEvnetData.type == 6)
                    {
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
                        {
                            var uis = ui as UIPanel_RunTimeHUD;
                            uis.PlaySpineUIFX(hybridEvnetData.strArgs.Value, hybridEvnetData.args.x).Forget();
                        }
                    }
                    else if (hybridEvnetData.type == 7)
                    {
                        var bindingType = (int)hybridEvnetData.args.x;
                        var expAddtion = (int)hybridEvnetData.args.y;
                        UpdateBindingRelate(bindingType, expAddtion, out int lastBindingLevel, out int curBindingLevel);
                    }
                    else if (hybridEvnetData.type == 8)
                    {
                        var bossStag = (int)hybridEvnetData.args.x;
                        int skillID = (int)hybridEvnetData.args.y;

                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
                        {
                            var uiRuntime = ui as UIPanel_RunTimeHUD;
                            if (uiRuntime.bossState == bossStag + 1 && !hybridEvnetData.disAble)
                            {
                                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                                BuffHelper.AddSkillByDcb(entityManager, skillID, player, 1);
                            }
                        }
                    }
                    else if (hybridEvnetData.type == 9)
                    {
                        UnityEngine.Debug.Log($"GetRandomTieSkill:000000");
                        int needType = (int)hybridEvnetData.args.x;
                        int needQuality = (int)hybridEvnetData.args.y;

                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var uia))
                        {
                            var uis = uia as UIPanel_RunTimeHUD;


                            //int judgeValue = needType * 10 + needQuality;
                            var tbskill = ConfigManager.Instance.Tables.Tbskill;
                            var tbskill_quality = ConfigManager.Instance.Tables.Tbskill_quality;
                            var canSelectedList = new List<skill>();
                            foreach (var a in tbskill.DataList)
                            {
                                bool canBind = true;
                                if (needType != 0)
                                {
                                    canBind = a.skillBindingId[0] == needType;
                                }

                                bool canQuality = true;
                                if (needQuality != 0)
                                {
                                    canQuality = a.skillQualityId == needQuality;
                                }

                                bool canUpperLevel = true;

                                if (uis.skillsDic.ContainsKey(a.id))
                                {
                                    var levelUpperlimit = tbskill_quality.Get(a.skillQualityId).levelUpperlimit;
                                    canUpperLevel = uis.skillsDic[a.id] < levelUpperlimit;
                                }

                                if (a.type == 2 && a.level == 1 && canBind && canQuality && canUpperLevel)
                                {
                                    canSelectedList.Add(a);
                                }
                            }

                            //TODO:random
                            if (canSelectedList.Count > 0)
                            {
                                var skillid = canSelectedList[UnityEngine.Random.Range(0, canSelectedList.Count)].id;
                                int trueSkillId = uis.GetTrueSkillId(skillid);
                                uis.AddSkill(skillid);


                                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                                //var skillIndex = skillList[new Random .NextInt(0, skillList.Length)];
                                UnityEngine.Debug.Log($"GetRandomTieSkill:{skillid}");
                                BuffHelper.AddSkillByDcb(entityManager, trueSkillId, player, 1);
                            }
                        }
                    }
                    else if (hybridEvnetData.type == 10)
                    {
                        var addtion = hybridEvnetData.args.x;
                        var bindingID = (int)hybridEvnetData.args.y;
                        var propID = (int)hybridEvnetData.args.z;
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
                        {
                            var uiRuntime = ui as UIPanel_RunTimeHUD;
                            int exp = 0;
                            if (bindingID == 0)
                            {
                                // 使用 foreach 循环遍历所有值并求和
                                foreach (int value in uiRuntime.bindingsDic.Values)
                                {
                                    exp += value;
                                }
                            }
                            else
                            {
                                if (uiRuntime.bindingsDic.ContainsKey(bindingID))
                                {
                                    exp = uiRuntime.bindingsDic[bindingID];
                                }
                            }

                            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                            var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
                            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
                            var chaStatesRW = entityManager.GetComponentData<ChaStats>(player);
                            var playerDataRW = entityManager.GetComponentData<PlayerData>(player);
                            Log.Debug($"11111:{exp * addtion},propid:{propID}");
                            UnityHelper.ChangeProperty(ref chaStatesRW, ref playerDataRW, propID,
                                exp * addtion);
                        }
                    }
                    else if (hybridEvnetData.type == 11)
                    {
                        int bindingID = (int)hybridEvnetData.args.x;
                        int level = (int)hybridEvnetData.args.y;
                        int skillID = (int)hybridEvnetData.args.z;
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
                        {
                            var uiRuntime = ui as UIPanel_RunTimeHUD;
                            if (!uiRuntime.bindingsDic.TryGetValue(bindingID, out var exp))
                            {
                                Log.Error($"没有该bindingId");
                            }

                            var curLevel = GetBindingIndex(exp);
                            Log.Error($"当前阶级：{curLevel},需要阶级：{level}");
                            if (curLevel >= level)
                            {
                                hybridEvnetData.args.w = 1;
                                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                                var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
                                var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
                                var skills = entityManager.GetBuffer<Skill>(player);
                                BuffHelper.AddSkillByDcb(entityManager, skillID, player, 1);
                            }
                        }
                    }
                    else if (hybridEvnetData.type == 44)
                    {
                        //警报
                        var tblanguage = ConfigManager.Instance.Tables.Tblanguage;
                        string key = default;
                        switch ((int)hybridEvnetData.args.x)
                        {
                            case 1:
                                key = $"battle_alarm_text_1";
                                break;
                            case 2:
                                key = $"battle_alarm_text_2";
                                break;
                        }

                        JiYuUIHelper.ClearCommonResource();
                        UIHelper.CreateAsync(UIType.UICommon_Resource, tblanguage.Get(key).current);
                    }
                    else if (hybridEvnetData.type == 45)
                    {
                        //震屏
                        var global = XFramework.Common.Instance.Get<Global>();
                        global.CameraShake(hybridEvnetData.args.x / 1000f);
                    }
                    else if (hybridEvnetData.type == 46)
                    {
                        if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
                        {
                            var uis = ui as UIPanel_RunTimeHUD;
                            var dic = uis.skillsDic;

                            var list = ConfigManager.Instance.Tables.Tbskill.DataList;
                            int skillCount = 0;
                            for (int i = 0; i < list.Count; i++)
                            {
                                if (list[i].type == 2 && list[i].level == 1)
                                {
                                    if (dic.ContainsKey(list[i].id))
                                    {
                                        skillCount++;
                                    }
                                }
                            }

                            Log.Debug($"skillcount:{skillCount}");
                            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                            var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
                            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
                            var chaStatesRW = entityManager.GetComponentData<ChaStats>(player);
                            var playerDataRW = entityManager.GetComponentData<PlayerData>(player);
                            UnityHelper.ChangeProperty(ref chaStatesRW, ref playerDataRW,
                                (int)hybridEvnetData.args.x,
                                hybridEvnetData.args.y * skillCount);
                        }
                    }
                    //新手引导 提示碰撞伤害
                    else if (hybridEvnetData.type == 999000 + 323)
                    {
                        // int lastTwoDigits = hybridEvnetData.type % 100;
                        // 
                        // Debug.Log($"新手引导 {lastTwoDigits}");
                        // var tbguide = ConfigManager.Instance.Tables.Tbguide;
                        // var guide = tbguide.Get(lastTwoDigits);
                        var tbguide = ConfigManager.Instance.Tables.Tbguide;
                        foreach (var vGuide in tbguide.DataList)
                        {
                            if (vGuide.guideType == 323)
                            {
                                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
                                {
                                    UIHelper.CreateAsync(UIType.UIPanel_GuideTips, vGuide.id);
                                }

                                break;
                            }
                        }
                    }
                    else if (hybridEvnetData.type == 999000 + 313)
                    {
                        int guideType = hybridEvnetData.type - 999000;
                        // int lastTwoDigits = hybridEvnetData.type % 100;
                        // 
                        // Debug.Log($"新手引导 {lastTwoDigits}");
                        // var tbguide = ConfigManager.Instance.Tables.Tbguide;
                        // var guide = tbguide.Get(lastTwoDigits);
                        var tbguide = ConfigManager.Instance.Tables.Tbguide;
                        foreach (var vGuide in tbguide.DataList)
                        {
                            if (vGuide.guideType == guideType)
                            {
                                if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
                                {
                                    OnGuide313(vGuide.id, hybridEvnetData.args.xyz);
                                }

                                break;
                            }
                        }
                    }
                    else if (hybridEvnetData.type == 1015)
                    {
                        var entity = hybridEvnetData.bossEntity;
                        var spineHybridData = EntityManager.GetComponentData<SpineHybridData>(entity);
                        var myHybrid = EntityManager.GetComponentData<TransformHybridUpdateData>(entity);
                        var bone = spineHybridData.skeletonAnimation.Value.skeleton.FindBone("shengti2");

                        if (bone != null)
                        {
                            var tran = myHybrid.go.Value.transform.localPosition;
                            var scale = myHybrid.go.Value.transform.localScale;
                            // 获取骨骼的世界坐标
                            Vector2 boneWorldPosition =
                                new Vector2(tran.x + bone.WorldX * scale.x * 0.5f + hybridEvnetData.args.x,
                                    tran.y + bone.WorldY * scale.y * 0.5f + hybridEvnetData.args.y);

                            var playerTran = EntityManager.GetComponentData<LocalTransform>(player);
                            playerTran.Position.xy = boneWorldPosition;
                            EntityManager.SetComponentData(player, playerTran);
                            // 在控制台输出骨骼的世界位置
                            Debug.Log($"Bone Position: {boneWorldPosition}");
                        }
                    }

                    hybridEvnetData.disAble = true;
                    //事件11需要全局存在 故不禁用
                    if (hybridEvnetData.type == 11 && hybridEvnetData.args.w != 1)
                    {
                        hybridEvnetData.disAble = false;
                    }

                    if (hybridEvnetData.type == 1015)
                    {
                        hybridEvnetData.disAble = false;
                    }

                    if (hybridEvnetData.disAble)
                    {
                        EntityManager.DestroyEntity(e);
                    }
                }).Run();
        }

        private async UniTaskVoid OnPlayerDieEvent()
        {
            await UniTask.Delay(3000);
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var chaStats = entityManager.GetComponentData<ChaStats>(player);
            if (chaStats.chaProperty.rebirthCount1 > 0)
            {
                chaStats.chaProperty.rebirthCount1--;
                entityManager.SetComponentData(player, chaStats);
                JiYuUIHelper.RebirthPlayer();
            }
            else if (chaStats.chaProperty.rebirthCount > 0)
            {
                chaStats.chaProperty.rebirthCount--;
                entityManager.SetComponentData(player, chaStats);
                JiYuUIHelper.RebirthPlayer();
            }
            else
            {
                //JiYuUIHelper.StartStopTime(false);
                UIHelper.Create(UIType.UIPanel_Rebirth);
            }
        }

        private async UniTaskVoid OnGuide313(int id, Vector3 pos)
        {
            var ui = await UIHelper.CreateAsync(UIType.UIPanel_GuideTips, id);

            ui.GetRectTransform().SetAnchoredPosition(pos);
        }

        private async UniTaskVoid OnBossDieEvent()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var wbeQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag));
            var wbe = wbeQuery.ToEntityArray(Allocator.Temp)[0];
            var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var chaStats = entityManager.GetComponentData<ChaStats>(player);
            var playerData = entityManager.GetComponentData<PlayerData>(player);
            var tran = entityManager.GetComponentData<LocalTransform>(player);
            var gameTimeData = entityManager.GetComponentData<GameTimeData>(wbe);
            var gameOthersData = entityManager.GetComponentData<GameOthersData>(wbe);


            AudioManager.Instance.ClearFModBgmAudio();

            //Boss死亡角色无敌
            chaStats.chaImmuneState.immuneDamage = true;
            entityManager.SetComponentData(player, chaStats);


            //角色吸收所有掉落

            //AudioManager.Instance.PlayFModAudio(2103);
            await UniTask.Delay(5000);


            playerData.playerOtherData.pickupRadius = MathHelper.MaxNum;
            entityManager.SetComponentData(player, playerData);


            await UniTask.Delay(1000);
            chaStats.chaImmuneState.immuneDamage = false;
            playerData.playerData.pickUpRadiusRatios = 1;
            entityManager.SetComponentData(player, playerData);
            entityManager.SetComponentData(player, chaStats);

            //Log.Debug($"OnBossDieEvent{1}");
            await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionFXEnter);
            //Log.Debug($"OnBossDieEvent{2}");
            //await UniTask.Delay(2000);
            //Log.Debug($"OnBossDieEvent{3}");

            JiYuUIHelper.SetPlayerMass(true);


            playerData.playerOtherData.isBossFight = false;
            tran.Position = playerData.playerOtherData.bossBeforePos;
            entityManager.SetComponentData(player, tran);
            entityManager.SetComponentData(player, playerData);
            //ecb.DestroyEntity(entityIndexInQuery, crowdSurfaces);
            /*entityManager.SetComponentData(crowdSurfaces, new TimeToDieData
            {
                duration = 5
            });*/
            var global = XFramework.Common.Instance.Get<Global>();
            global.InitCameraBounds(playerData.playerOtherData.bossBeforePos, gameOthersData.mapData.mapType);
            gameTimeData.refreshTime.defaultGameTimeScale = 1;
            gameTimeData.refreshTime.gameTimeScale = gameTimeData.refreshTime.defaultGameTimeScale;
            entityManager.SetComponentData(wbe, gameTimeData);
            //删除boss场景所有entity


            await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionFXExit);
            AudioManager.Instance.ClearFModBgmAudio();
            AudioManager.Instance.PlayFModAudio(2103);
        }


        private async UniTaskVoid SwitchBossScene(HybridEventData hybridEvnetData)
        {
            Log.Debug($"bossEntity{hybridEvnetData.args} {hybridEvnetData.bossEntity}");
            await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionFXEnter);
            //await UniTask.Delay(500, true);

            var tbscene_boss = ConfigManager.Instance.Tables.Tbscene_boss;
            var tbmonster = ConfigManager.Instance.Tables.Tbmonster;
            var tbmonster_attr = ConfigManager.Instance.Tables.Tbmonster_attr;
            var tbmonster_model = ConfigManager.Instance.Tables.Tbmonster_model;
            var global = XFramework.Common.Instance.Get<Global>();
            var sceneBossId = tbmonster.Get((int)hybridEvnetData.args.x).sceneBossId;
            var sceneBoss = tbscene_boss.Get(sceneBossId);
            var monster = tbmonster.Get((int)hybridEvnetData.args.x);
            var monster_attr = tbmonster_attr.Get(monster.monsterAttrId);
            var monster_model = tbmonster_model.Get(monster_attr.bookId);

            float3 playerPos = hybridEvnetData.args.yzw;
            var EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerQuery = EntityManager.CreateEntityQuery(typeof(PlayerData), typeof(ChaStats));
            while (playerQuery.IsEmpty)
            {
                await UniTask.Delay(200);
            }

            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var tran = EntityManager.GetComponentData<LocalTransform>(player);
            playerPos.y -= 50;
            tran.Position = playerPos;
            EntityManager.SetComponentData(player, tran);


            var cameraPos = hybridEvnetData.args.yzw;
            cameraPos.y = 20f;
            global.SetCameraBounds(
                new Vector3(sceneBoss.areaSize[0] / 1000f, 10f, 1),
                cameraPos);
            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
            {
                var uiRuntime = ui as UIPanel_RunTimeHUD;
                uiRuntime.SetBossText(monster.id);
                uiRuntime.SetBossEntity(hybridEvnetData.bossEntity);
            }

            //325011 325012
            if (monster.skillGroup.Contains(325011))
            {
                string prefab = "model_monster_2391_spine";
                for (int i = 0; i < 5; i++)
                {
                    var boss = ResourcesManager.Instantiate(prefab);
                    GameObjectPool.Instance.Recycle(prefab, boss);
                }
            }

            if (monster.skillGroup.Contains(325012))
            {
                string prefab = "model_monster_2392_spine";
                for (int i = 0; i < 5; i++)
                {
                    var boss = ResourcesManager.Instantiate(prefab);
                    GameObjectPool.Instance.Recycle(prefab, boss);
                }
            }

            if (monster.skillGroup.Contains(345022))
            {
                string prefab = "model_monster_4391_spine";
                for (int i = 0; i < 5; i++)
                {
                    var boss = ResourcesManager.Instantiate(prefab);
                    GameObjectPool.Instance.Recycle(prefab, boss);
                }
            }

            // if (EntityManager.HasComponent<MyHybridMono>(hybridEvnetData.bossEntity))
            // {
            //     var hybridMono = EntityManager.GetComponentData<TransformHybridUpdateData>(hybridEvnetData.bossEntity);
            //     hybridMono.go.Value =ResourcesManager.Instantiate($"{monster_model.model}_spine");
            //     
            //     // var boss = ResourcesManager.Instantiate($"{monster_model.model}_spine",
            //     //     hybridMono.go.Value.GetComponent<Transform>());
            //
            //     EntityManager.SetComponentData(hybridEvnetData.bossEntity, new SpineHybridData
            //     {
            //         go =  hybridMono.go.Value,
            //         skeletonAnimation =  hybridMono.go.Value.GetComponent<SkeletonAnimation>()
            //     });
            //     Debug.Log($"bossIns { hybridMono.go.Value.name}");
            // }

            // if (monster_attr.type == 5)
            // {
            //     //var hybridMono = EntityManager.GetComponentObject<MyHybridMono>(hybridEvnetData.bossEntity);
            //
            //  
            // }
            JiYuUIHelper.EnableGuide(false);
            // JiYuUIHelper.EnableISystem<TriggerSystem>(false);
            // var playerQuery = EntityManager.CreateEntityQuery(typeof(PlayerData), typeof(ChaStats));
            // var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            // var chaStats = EntityManager.GetComponentData<ChaStats>(player);
            // chaStats.chaControlState.cantMove = true;
            // chaStats.chaControlState.cantWeaponAttack = true;
            // EntityManager.SetComponentData(player, chaStats);

            var states = EntityManager.GetBuffer<State>(hybridEvnetData.bossEntity);
            var stateMachine = EntityManager.GetComponentData<StateMachine>(hybridEvnetData.bossEntity);

            int idleId =
                BuffHelper.GetStateIndex(states, State.TypeId.CommonBossIdle);

            int moveId =
                BuffHelper.GetStateIndex(states, State.TypeId.CommonBossMove);
            stateMachine.transitionToStateIndex = idleId;

            EntityManager.SetComponentData(hybridEvnetData.bossEntity, stateMachine);


            await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionFXExit);

            await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionShattersEnter);
            //TODO:摄像头 boss待机动画

            AudioManager.Instance.ClearFModBgmAudio();

            int audioId = monster_model.bookType == 2 ? 2102 : 2101;
            AudioManager.Instance.PlayFModAudio(audioId);

            await UniTask.Delay(3000);
            await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionShattersExit);
            stateMachine.transitionToStateIndex = moveId;
            EntityManager.SetComponentData(hybridEvnetData.bossEntity, stateMachine);

            JiYuUIHelper.SetPlayerMass(false);
            //var chaStats = EntityManager.GetComponentData<ChaStats>(player);
            // chaStats.chaControlState.cantMove = false;
            // chaStats.chaControlState.cantWeaponAttack = false;
            // EntityManager.SetComponentData(player, chaStats);
            // JiYuUIHelper.EnableISystem<TriggerSystem>(true);
            JiYuUIHelper.EnableGuide(true);
        }

        private async UniTaskVoid RecycleHybridSpine(HybridEventData hybridEvnetData)
        {
            //var hybridMono = EntityManager.GetComponentData<TransformHybridUpdateData>(hybridEvnetData.bossEntity);

            var name = hybridEvnetData.go.Value.name.Replace("(Clone)", "");
            GameObjectPool.Instance.Recycle(name, hybridEvnetData.go.Value);
        }

        private async UniTaskVoid SpawnHybridSpine(HybridEventData hybridEvnetData)
        {
            Log.Debug($"SpawnHybridSpine {hybridEvnetData.args} {hybridEvnetData.bossEntity}");

            var tbmonster = ConfigManager.Instance.Tables.Tbmonster;
            var tbmonster_attr = ConfigManager.Instance.Tables.Tbmonster_attr;
            var tbmonster_model = ConfigManager.Instance.Tables.Tbmonster_model;

            var sceneBossId = tbmonster.Get((int)hybridEvnetData.args.x).sceneBossId;
            var monster = tbmonster.Get((int)hybridEvnetData.args.x);
            var monster_attr = tbmonster_attr.Get(monster.monsterAttrId);
            var monster_model = tbmonster_model.Get(monster_attr.bookId);

            var hybridMono = EntityManager.GetComponentData<TransformHybridUpdateData>(hybridEvnetData.bossEntity);
            var prefab = $"{monster_model.model}_spine";
            GameObject ins = GameObjectPool.Instance.Fetch(prefab);
            //GameObject ins = null;
            if (ins != null)
            {
                //ins.transform.SetParent(hybridMono.go.Value.GetComponent<Transform>());
                hybridMono.go.Value = ins;
                hybridMono.go.Value.transform.SetParent(null); // 移除父节点
                // 获取当前活动场景
                var currentScene = SceneManager.GetActiveScene();

                // 将 GameObject 移动到当前场景
                SceneManager.MoveGameObjectToScene(hybridMono.go.Value, currentScene);
                // 现在你可以安全地分离其父级了（如果需要）
                //hybridMono.go.Value.transform.SetParent(null);
                var  skeletonAnimation=hybridMono.go.Value.GetComponent<SkeletonAnimation>();
                skeletonAnimation.Initialize(true);
                EntityManager.SetComponentData(hybridEvnetData.bossEntity, new SpineHybridData
                {
                    go = hybridMono.go.Value,
                    skeletonAnimation = skeletonAnimation
                });
                EntityManager.SetComponentData(hybridEvnetData.bossEntity, new TransformHybridUpdateData
                {
                    go = hybridMono.go.Value
                });

                Debug.Log($"SpawnHybridSpine {ins.name}");
            }
            else
            {
                hybridMono.go.Value = ResourcesManager.Instantiate($"{monster_model.model}_spine");
                hybridMono.go.Value.transform.SetParent(null); // 移除父节点
                // var boss = ResourcesManager.Instantiate(prefab,
                //     hybridMono.go.Value.GetComponent<Transform>());

                EntityManager.SetComponentData(hybridEvnetData.bossEntity, new SpineHybridData
                {
                    go = hybridMono.go.Value,
                    skeletonAnimation = hybridMono.go.Value.GetComponent<SkeletonAnimation>()
                });
                EntityManager.SetComponentData(hybridEvnetData.bossEntity, new TransformHybridUpdateData
                {
                    go = hybridMono.go.Value
                });
                Debug.Log($"SpawnHybridSpine {hybridMono.go.Value.name}");
            }
        }


        private void UpdateBindingRelate(int bindingID, int expAddtion,
            out int lastBindingLevel, out int curBindingLevel)
        {
            lastBindingLevel = default;
            curBindingLevel = default;

            if (bindingID == 0)
            {
                var skillBinding = ConfigManager.Instance.Tables.Tbskill_binding;
                var randNum = UnityEngine.Random.Range(0, skillBinding.DataList.Count);
                bindingID = skillBinding[randNum].id;
            }

            if (JiYuUIHelper.TryGetUI(UIType.UIPanel_RunTimeHUD, out var ui))
            {
                var uis = ui as UIPanel_RunTimeHUD;
                if (uis.bindingsDic.TryGetValue(bindingID, out int exp))
                {
                    lastBindingLevel = JiYuUIHelper.GetBindingLevel(exp);
                    uis.bindingsDic[bindingID] += expAddtion;
                    curBindingLevel = JiYuUIHelper.GetBindingLevel(exp + expAddtion);
                }

                ChangeWeaponSkillID(bindingID, lastBindingLevel, curBindingLevel, ui);
            }
        }

        int GetBindingIndex(int exp)
        {
            var player_skill_binding_rank = ConfigManager.Instance.Tables.Tbskill_binding_rank;

            int index = -1;
            foreach (var item in player_skill_binding_rank.DataList)
            {
                if (exp < item.exp)
                {
                    index = item.id;
                    break;
                }
            }

            return index;
        }

        private void ChangeWeaponSkillID(int bindingId, int lastLevel, int curLevel, UI panelRunTimeHUD)
        {
            var player_skill_binding = ConfigManager.Instance.Tables.Tbskill_binding;
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var playerData = entityManager.GetComponentData<PlayerData>(player);
            var weaponID = playerData.playerOtherData.weaponSkillId;
            var weaponAddtionID = playerData.playerOtherData.weaponAdditionID;
            var parentUI = panelRunTimeHUD as UIPanel_RunTimeHUD;
            if (!parentUI.bindingsDic.TryGetValue(bindingId, out var exp))
            {
                Log.Error($"没有该bindingId");
            }

            if (curLevel - lastLevel > 0)
            {
                if (lastLevel == 0)
                {
                    var weaponIdAddition = player_skill_binding.Get(bindingId).skillId;
                    BuffHelper.AddSkillByDcb(entityManager, weaponIdAddition, player, 0);
                    switch (bindingId)
                    {
                        case 1:
                            weaponAddtionID.x = weaponIdAddition;
                            break;
                        case 2:
                            weaponAddtionID.y = weaponIdAddition;
                            break;
                        case 3:
                            weaponAddtionID.z = weaponIdAddition;
                            break;
                        case 4:
                            weaponAddtionID.w = weaponIdAddition;
                            break;
                    }


                    Log.Error($"lastLevel == 0,weaponID:{weaponIdAddition}");
                }
                else if (lastLevel > 0 && lastLevel < 3)
                {
                    int weaponIdAddition = 0;
                    switch (bindingId)
                    {
                        case 1:
                            weaponIdAddition = weaponAddtionID.x;
                            break;
                        case 2:
                            weaponIdAddition = weaponAddtionID.y;
                            break;
                        case 3:
                            weaponIdAddition = weaponAddtionID.z;
                            break;
                        case 4:
                            weaponIdAddition = weaponAddtionID.w;
                            break;
                    }

                    var newWeaponBuffID = weaponIdAddition + 1;
                    BuffHelper.RemoveOldSkill(weaponIdAddition, entityManager, player);
                    BuffHelper.AddSkillByDcb(entityManager, newWeaponBuffID, player, 0);
                    switch (bindingId)
                    {
                        case 1:
                            weaponAddtionID.x = newWeaponBuffID;
                            break;
                        case 2:
                            weaponAddtionID.y = newWeaponBuffID;
                            break;
                        case 3:
                            weaponAddtionID.z = newWeaponBuffID;
                            break;
                        case 4:
                            weaponAddtionID.w = newWeaponBuffID;
                            break;
                    }

                    Log.Error($"lastLevel > 0 && lastLevel < 3,weaponID:{newWeaponBuffID}");
                }
                else
                {
                    if (!parentUI.isEvolve)
                    {
                        var newWeaponSkillID = weaponID + 4;
                        BuffHelper.RemoveOldSkill(weaponID, entityManager, player);
                        BuffHelper.AddSkillByDcb(entityManager, newWeaponSkillID, player, 0);
                        weaponID = newWeaponSkillID;
                        parentUI.isEvolve = true;
                        Log.Error($"lastLevel > 3,weaponID:{newWeaponSkillID}");
                    }
                }


                playerData = entityManager.GetComponentData<PlayerData>(player);
                playerData.playerOtherData.weaponSkillId = weaponID;
                entityManager.SetComponentData(player, playerData);
            }
        }
    }
}