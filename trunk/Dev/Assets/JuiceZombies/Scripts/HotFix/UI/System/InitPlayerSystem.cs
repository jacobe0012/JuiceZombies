//---------------------------------------------------------------------
// JiYuStudio
// Author: 格伦
// Time: 2023-09-27 12:06:32
//---------------------------------------------------------------------

using cfg.config;
using Common;
using Google.Protobuf;
using Main;
using ProjectDawn.Navigation;
using Spine.Unity;
using System;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using XFramework;

namespace HotFix_UI
{
    //初始化玩家系统 
    //初始化完成才会添加WorldBlackBoardTag
    [DisableAutoCreation]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(HandleInputSystem))]
    public partial class InitPlayerSystem : SystemBase
    {
        private BattleProperty mybattleProperty;

        protected override void OnCreate()
        {
            RequireForUpdate(SystemAPI.QueryBuilder().WithAll<PrefabMapData, GlobalConfigData>()
                .WithNone<WorldBlackBoardTag>().Build());
        }


        protected override void OnUpdate()
        {
            var global = XFramework.Common.Instance.Get<Global>();
            var constConfig = ConfigManager.Instance.Tables.Tbconstant;
            var tbbattle_constant = ConfigManager.Instance.Tables.Tbbattle_constant;


            int equip_default_weapon_id = constConfig.Get("equip_default_weapon_id").constantValue;
            int equip_default_weapon_skill = constConfig.Get("equip_default_weapon_skill").constantValue;

            if (global.isStandAlone || global.isIntroGuide)
            {
                mybattleProperty = new BattleProperty
                {
                    Properties =
                    {
                        "103030;0", "104000;0", "102020;0", "103010;10", "106220;0", "106120;0", "102000;100",
                        "102120;0", "110000;0", "102100;0", "108000;10", "107010;28000", "117000;0", "115000;0",
                        "101420;0", "101520;0", "111000;0", "113000;0", "101600;0", "105000;0", "105100;20000",
                        "102130;0", "107100;1500", "102030;0", "103020;0", "106230;0", "107000;29400", "102110;0",
                        "102010;100", "103000;10", "101220;0", "101320;0", "109000;2000", "109010;2000", "106130;0",
                        "107020;0", "118200;0", "116000;0", "118100;0", "114000;0", "112000;0", "115000;0",
                        "122100;10000", "114100;10000", "114200;10000", "108010;10"
                    },
                    WeaponId = equip_default_weapon_id,
                    Skills =
                    {
                        //105110
                        //145114,
                        //4,
                        //2,
                        //435014,
                        //432035,
                        //432034,
                        //433024,
                        //100110
                        equip_default_weapon_skill,
                        //100140
                        //110114
                        //130314,
                        //300022,
                        //436015
                        //100110
                        //300022
                        //300012
                        //120001
                        //100110
                        //100130
                        //100260
                        //130414
                        //105110
                    }
                };
            }

            if (mybattleProperty == null) return;
            var sbe = SystemAPI.GetSingletonEntity<PrefabMapData>();
            var prefabMapData = SystemAPI.GetComponent<PrefabMapData>(sbe);
            var config = SystemAPI.GetComponent<GlobalConfigData>(sbe);

            var player = EntityManager.Instantiate(prefabMapData.prefabHashMap["player"]);

            var chaStats = new ChaStats();
            var playerData = new PlayerData();

            Log.Debug($"前端处理后端数据>>>", Color.green);

            var skillConfig = ConfigManager.Instance.Tables.Tbskill;

            var skills = EntityManager.GetBuffer<Skill>(player);

            Log.Debug($"后端传入数据Properties", Color.cyan);
            Log.Debug($"{mybattleProperty.Properties}", Color.green);
            //前端处理数据：

            JiYuUIHelper.InitPlayerProperty(ref playerData, ref chaStats, mybattleProperty);

            Log.Debug($"后端传入数据Skills", Color.cyan);
            //武器技能 1 00000开头
            //装备词条 4 00000开头
            //天赋技能 7 00000开头
            Log.Debug($"技能:{mybattleProperty.Skills}", Color.green);

            foreach (var skill in mybattleProperty.Skills)
            {
                var tbskill = skillConfig.GetOrDefault(skill);
                if (tbskill == null)
                {
                    Debug.LogError($"技能id:{skill} 表里不存在");
                    continue;
                }

                var events = tbskill.skillEventArray;
                if (events.Count > 0)
                {
                    var tbEvent = ConfigManager.Instance.Tables.Tbevent_0;
                    for (int i = 0; i < events.Count; i++)
                    {
                        var eventType = tbEvent.Get(events[i]).type;
                        var gameEvent = new GameEvent
                        {
                            Int32_0 = events[i],
                            Boolean_6 = true
                        };
                        var eventBuff = EntityManager.GetBuffer<GameEvent>(sbe);
                        eventBuff.Add(gameEvent);
                    }
                }
                else
                {
                    if (tbskill.type == 1)
                    {
                        playerData.playerOtherData.weaponSkillId = skill;
                        skills.Add(new Skill
                        {
                            CurrentTypeId = (Skill.TypeId)1,
                            Int32_0 = skill,
                            Entity_5 = player,
                        });
                    }
                    else
                    {
                        skills.Add(new Skill
                        {
                            CurrentTypeId = (Skill.TypeId)1,
                            Int32_0 = skill,
                            Entity_5 = player,
                            Int32_10 = 1
                        });
                    }
                }
            }


            Log.Debug($"后端传入WeaponId:{mybattleProperty.WeaponId}", Color.green);
            //playerData.playerOtherData.weaponId = mybattleProperty.WeaponId * 100 + mybattleProperty.WeaponQuality;
            playerData.playerOtherData.weaponId = mybattleProperty.WeaponId;
            chaStats.chaResource.actionSpeed = 1;
            chaStats.chaResource.hp = chaStats.chaProperty.maxHp;
            chaStats.chaResource.direction = math.up();


            var mass = EntityManager.GetComponentData<PhysicsMass>(player);
            mass.InverseMass = 1f / (float)chaStats.chaProperty.mass;
            //mass.InverseMass = 1000f;
            //新手引导数据
            bool hasCollideTipsGuide = ResourcesSingleton.Instance.settingData.GuideList.Contains(2);
            if (hasCollideTipsGuide)
            {
                playerData.playerOtherData.guideList.Add(323);
            }

            bool hasCollideTipsGuide1 = ResourcesSingleton.Instance.settingData.GuideList.Contains(7);
            if (hasCollideTipsGuide1)
            {
                playerData.playerOtherData.guideList.Add(313);
            }
            // foreach (var guide in ResourcesSingleton.Instance.settingData.GuideList)
            // {
            //     playerData.playerOtherData.guideList.Add(guide);
            // }


            ResourcesSingleton.Instance.playerProperty.playerData = playerData;


            var tran = EntityManager.GetComponentData<LocalTransform>(player);


            //5.555*6459

            tran.Scale = tbbattle_constant.Get($"player_module_size").constantValue / 10000f;

            var agentShape = EntityManager.GetComponentData<AgentShape>(player);
            agentShape.Radius = tran.Scale / 2f;

            EntityManager.SetComponentData(player, tran);
            EntityManager.SetComponentData(player, mass);
            EntityManager.SetComponentData(player, agentShape);

            EntityManager.AddBuffer<GameEvent>(player);

            #region Weapon

            var tbweapon = ConfigManager.Instance.Tables.Tbweapon;
            var tbplayer_weapon_index = ConfigManager.Instance.Tables.Tbplayer_weapon_index;
            int equipId = playerData.playerOtherData.weaponId;

            var playerWeapon = tbweapon.GetOrDefault(equipId);

            var player_weapon_index = tbplayer_weapon_index.Get(1, equipId);

            if (playerWeapon != null)
            {
                if (!prefabMapData.prefabHashMap.TryGetValue(playerWeapon.model,
                        out var weaponPrefab))
                {
                    Debug.Log($"没有{playerWeapon.model}预制件");
                    //continue;
                }


                var weaponData = EntityManager.GetComponentData<WeaponData>(weaponPrefab);

                var weaponIns = EntityManager.Instantiate(weaponPrefab);
                var linkedEntityGroups = EntityManager.GetBuffer<LinkedEntityGroup>(player);
                linkedEntityGroups.Add(new LinkedEntityGroup()
                {
                    Value = weaponIns
                });

                EntityManager.AddComponentData(weaponIns, new Parent()
                {
                    Value = player
                });

                weaponData.weaponId = equipId;

                var animPara = playerWeapon.displayPara1;
                var diedDisplayPara = playerWeapon.displayPara2;
                BuffHelper.SetWeaponData(ref weaponData, ref animPara, playerWeapon.displayType);
                BuffHelper.SetWeaponData(ref weaponData, ref diedDisplayPara, 5);

                weaponData.offset = new float3(player_weapon_index.monsterWeaponIndex[0] / 1000f,
                    player_weapon_index.monsterWeaponIndex[1] / 1000f, 0);
                weaponData.scale = player_weapon_index.monsterWeaponIndex[2] / 10000f;
                weaponData.rotation = quaternion.RotateZ(math.radians(player_weapon_index.monsterWeaponIndex[3]));

                EntityManager.SetComponentData(weaponIns, weaponData);
                var entityGroupData = EntityManager.GetComponentData<EntityGroupData>(player);
                entityGroupData.weaponEntity = weaponIns;
                var linkedEntity = EntityManager.GetBuffer<LinkedEntityGroup>(player);
                for (int i = 0; i < linkedEntity.Length; i++)
                {
                    if (EntityManager.HasComponent<RenderingEntityTag>(linkedEntity[i].Value))
                    {
                        entityGroupData.renderingEntity = linkedEntity[i].Value;
                        break;
                    }
                }


                EntityManager.SetComponentData(player, entityGroupData);
            }
            else
            {
                Log.Error($"weapon表里没有这个数据id:{equipId}");
            }

            #endregion

            var playerGO = new GameObject("playerGO");
            global.GameObjects.TargetCamera = playerGO;
            
            EntityManager.AddComponentData(player, new TransformHybridUpdateData
            {
                go = playerGO
            });

            //var mySkeleton = EntityManager.GetComponentData<TransformHybridUpdateData>(player);
            // mySkeleton.go.Value = playerGO;
            var trans = playerGO.GetComponent<Transform>();


            EntityManager.SetComponentData(player, chaStats);
            EntityManager.SetComponentData(player, playerData);

            global?.SetCameraTarget(trans);

            EntityManager.AddComponent<WorldBlackBoardTag>(sbe);
            Enabled = false;

            global.GameObjects?.Cover?.SetViewActive(false);

            LoadingAnim().Forget();
        }

        async UniTaskVoid LoadingAnim()
        {
            await JiYuTweenHelper.EnableLoading(true, UIManager.LoadingType.TranstionFXExit);
            JiYuUIHelper.EnableISystem<SpawnEnemySystem>(true);
            Log.Debug($"EnableISystem<SpawnEnemySystem>");
        }

        void OnClickPlayBtnFinishResponse(object sender, WebMessageHandlerOld.Execute e)
        {
            WebMessageHandlerOld.Instance.RemoveHandler(CMD.QUERYPROPERTY, OnClickPlayBtnFinishResponse);

            var battleProperty = new BattleProperty();
            battleProperty.MergeFrom(e.data);
            Log.Debug($"battleProperty:{battleProperty}");
            if (e.data.IsEmpty)
            {
                Log.Debug("e.data.IsEmpty", Color.red);
                return;
            }

            mybattleProperty = battleProperty;
        }

        protected override void OnStartRunning()
        {
            mybattleProperty = null;
            Log.Debug($"InitPlayerSystem Start", Color.cyan);
            var global = XFramework.Common.Instance.Get<Global>();
            if (!global.isIntroGuide)
            {
                WebMessageHandlerOld.Instance.AddHandler(CMD.QUERYPROPERTY, OnClickPlayBtnFinishResponse);
                NetWorkManager.Instance.SendMessage(CMD.QUERYPROPERTY, new StringValue()
                {
                    Value =
                        $"{ResourcesSingleton.Instance.battleData.battleId};{ResourcesSingleton.Instance.levelInfo.levelId}"
                });
            }
        }

        protected override void OnStopRunning()
        {
            Log.Debug($"InitPlayerSystem Stop", Color.cyan);
            mybattleProperty = null;
        }
    }
}