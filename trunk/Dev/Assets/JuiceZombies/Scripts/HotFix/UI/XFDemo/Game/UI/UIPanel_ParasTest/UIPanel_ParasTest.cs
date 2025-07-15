//---------------------------------------------------------------------
// JiYuStudio
// Author: xxx
// Time: #CreateTime#
//---------------------------------------------------------------------

using System.Collections.Generic;
using cfg.config;
using Common;
using Cysharp.Threading.Tasks;
using HotFix_UI;
using Main;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Material = UnityEngine.Material;

namespace XFramework
{
    [UIEvent(UIType.UIPanel_ParasTest)]
    internal sealed class UIPanel_ParasTestEvent : AUIEvent, IUILayer
    {
        public override string Key => UIPathSet.UIPanel_ParasTest;

        public override bool IsFromPool => false;

        public override bool AllowManagement => true;

        public UILayer Layer => UILayer.Low;

        public override UI OnCreate()
        {
            return UI.Create<UIPanel_ParasTest>();
        }
    }

    public partial class UIPanel_ParasTest : UI, IAwake
    {
        private long timerId;
        private Tbattr_variable attr_varibles;
        private Tblanguage language;
        private List<UISubPanel_ParasItem> parasItemsList0 = new List<UISubPanel_ParasItem>();
        private List<UISubPanel_ParasItem> parasItemsList1 = new List<UISubPanel_ParasItem>();
        private List<UISubPanel_ParasItem> parasItemsList2 = new List<UISubPanel_ParasItem>();
        private int btnIndex;

        private EntityManager entityManager;
        EntityQuery playerQuery;
        EntityQuery wbeQuery;
        private EntityQuery boardQuery;

        public async void Initialize()
        {
            await JiYuUIHelper.InitBlur(this);
            attr_varibles = ConfigManager.Instance.Tables.Tbattr_variable;
            language = ConfigManager.Instance.Tables.Tblanguage;
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));

            wbeQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag));
            boardQuery = entityManager.CreateEntityQuery(typeof(DropsData));

            var closeBtn = GetFromReference(KBtn_Close);
            var closeMaskBtn = GetFromReference(KMaskBtn);
            var btn_Confirm = GetFromReference(KBtn_Confirm);
            var btn_Restore0 = GetFromReference(KBtn_Restore0);
            var btn_Restore1 = GetFromReference(KBtn_Restore1);

            var btn_0 = GetFromReference(KBtn_0);
            var btn_1 = GetFromReference(KBtn_1);
            var btn_2 = GetFromReference(KBtn_2);
            var btn_3 = GetFromReference(KBtn_3);
            var btn_4 = GetFromReference(KBtn_4);
            btn_Confirm.GetRectTransform().SetScale(Vector2.one);
            btn_Restore0.GetRectTransform().SetScale(Vector2.one);
            btn_Restore1.GetRectTransform().SetScale(Vector2.one);
            closeBtn.GetRectTransform().SetScale(Vector2.one);
            closeMaskBtn.GetRectTransform().SetScale(Vector2.one);
            btn_0.GetRectTransform().SetScale(Vector2.one);
            btn_1.GetRectTransform().SetScale(Vector2.one);
            btn_2.GetRectTransform().SetScale(Vector2.one);
            btn_3.GetRectTransform().SetScale(Vector2.one);
            btn_4.GetRectTransform().SetScale(Vector2.one);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn_0, () =>
            {
                InitItem(0);
                InitParasData(btnIndex);
                SetBtnAlpha(btnIndex);
            });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn_1, () =>
            {
                InitItem(1);
                InitParasData(btnIndex);
                SetBtnAlpha(btnIndex);
            });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn_2, () =>
            {
                InitItem(2);
                InitParasData(btnIndex);
                SetBtnAlpha(btnIndex);
            });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn_3, () =>
            {
                InitItem(3);
                InitParasData(btnIndex);
                SetBtnAlpha(btnIndex);
            });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn_4, () =>
            {
                InitItem(4);
                InitParasData(btnIndex);
                SetBtnAlpha(btnIndex);
            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(closeBtn, Close);
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(closeMaskBtn, Close);

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn_Confirm, async () =>
            {
                SetPlayerProperty(0);
                UnityHelper.BeginTime();
                await UniTask.Delay(200);
                UnityHelper.StopTime();
                //InitParasData(btnIndex);
                Close();
            });

            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn_Restore0, () =>
            {
                SetPlayerProperty(1);
                //InitParasData(btnIndex);
                Close();
            });
            JiYuTweenHelper.DoScaleTweenOnClickAndLongPress(btn_Restore1, () =>
            {
                SetPlayerProperty(2);
                //InitParasData(btnIndex);
                Close();
            });
            btnIndex = -1;
            InitItem(0);
            InitParasData(btnIndex);
            SetBtnAlpha(btnIndex);
        }

        void SetBtnAlpha(int btnIndex)
        {
            var btn_0 = GetFromReference(KBtn_0);
            var btn_1 = GetFromReference(KBtn_1);
            var btn_2 = GetFromReference(KBtn_2);
            var btn_3 = GetFromReference(KBtn_3);
            var btn_4 = GetFromReference(KBtn_4);

            if (btnIndex == 0)
            {
                btn_0.GetImage().SetAlpha(0.5f);
                btn_1.GetImage().SetAlpha(1);
                btn_2.GetImage().SetAlpha(1);
                btn_3.GetImage().SetAlpha(1);
                btn_4.GetImage().SetAlpha(1);
            }
            else if (btnIndex == 1)
            {
                btn_0.GetImage().SetAlpha(1);
                btn_1.GetImage().SetAlpha(0.5f);
                btn_2.GetImage().SetAlpha(1);
                btn_3.GetImage().SetAlpha(1);
                btn_4.GetImage().SetAlpha(1);
            }
            else if (btnIndex == 2)
            {
                btn_0.GetImage().SetAlpha(1);
                btn_1.GetImage().SetAlpha(1);
                btn_2.GetImage().SetAlpha(0.5f);
                btn_3.GetImage().SetAlpha(1);
                btn_4.GetImage().SetAlpha(1);
            }
            else if (btnIndex == 3)
            {
                btn_0.GetImage().SetAlpha(1);
                btn_1.GetImage().SetAlpha(1);
                btn_2.GetImage().SetAlpha(1);
                btn_3.GetImage().SetAlpha(0.5f);
                btn_4.GetImage().SetAlpha(1);
            }
            else if (btnIndex == 4)
            {
                btn_0.GetImage().SetAlpha(1);
                btn_1.GetImage().SetAlpha(1);
                btn_2.GetImage().SetAlpha(1);
                btn_3.GetImage().SetAlpha(1);
                btn_4.GetImage().SetAlpha(0.5f);
            }
            else if (btnIndex == 5)
            {
            }
        }

        private void InitParasData(int btnIndex)
        {
            if (btnIndex == 0)
            {
                if (playerQuery.IsEmpty) return;
                var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
                var chaStats = entityManager.GetComponentData<ChaStats>(player);
                var playerData = entityManager.GetComponentData<PlayerData>(player);

                //----------------------参数面板------------------------


                parasItemsList0[0].SetInputTxt(playerData.playerData.level.ToString());


                parasItemsList0[1].SetInputTxt(playerData.playerData.exp.ToString());


                parasItemsList0[2].SetInputTxt(playerData.playerData.expRatios.ToString());


                parasItemsList0[3].SetInputTxt(playerData.playerData.gold.ToString());


                parasItemsList0[4].SetInputTxt(playerData.playerData.goldRatios.ToString());


                parasItemsList0[5].SetInputTxt(playerData.playerData.paper.ToString());


                parasItemsList0[6].SetInputTxt(playerData.playerData.paperRatios.ToString());


                parasItemsList0[7].SetInputTxt(playerData.playerData.equip.ToString());


                parasItemsList0[8].SetInputTxt(playerData.playerData.equipRatios.ToString());


                parasItemsList0[9].SetInputTxt(playerData.playerData.pickUpRadiusRatios.ToString());


                parasItemsList0[10].SetInputTxt(playerData.playerData.killEnemy.ToString());

                parasItemsList0[11].SetInputTxt(chaStats.chaProperty.maxHp.ToString());

                parasItemsList0[12].SetInputTxt(chaStats.chaProperty.defaultMaxHp.ToString());

                parasItemsList0[13].SetInputTxt(chaStats.chaProperty.hpRatios.ToString());

                parasItemsList0[14].SetInputTxt(chaStats.chaProperty.hpAdd.ToString());

                parasItemsList0[15].SetInputTxt(chaStats.chaProperty.curHpRatios.ToString());

                parasItemsList0[16].SetInputTxt(chaStats.chaProperty.hpRecovery.ToString());

                parasItemsList0[17].SetInputTxt(chaStats.chaProperty.defaultHpRecovery.ToString());

                parasItemsList0[18].SetInputTxt(chaStats.chaProperty.hpRecoveryRatios.ToString());

                parasItemsList0[19].SetInputTxt(chaStats.chaProperty.hpRecoveryAdd.ToString());


                parasItemsList0[20].SetInputTxt(playerData.playerData.propsRecoveryRatios.ToString());


                parasItemsList0[21].SetInputTxt(playerData.playerData.propsRecoveryAdd.ToString());

                parasItemsList0[22].SetInputTxt(chaStats.chaProperty.atk.ToString());

                parasItemsList0[23].SetInputTxt(chaStats.chaProperty.defaultAtk.ToString());

                parasItemsList0[24].SetInputTxt(chaStats.chaProperty.atkRatios.ToString());

                parasItemsList0[25].SetInputTxt(chaStats.chaProperty.atkAdd.ToString());

                parasItemsList0[26].SetInputTxt(chaStats.chaProperty.rebirthCount.ToString());

                parasItemsList0[27].SetInputTxt(chaStats.chaProperty.rebirthCount1.ToString());

                parasItemsList0[28].SetInputTxt(chaStats.chaProperty.critical.ToString());

                parasItemsList0[29].SetInputTxt(chaStats.chaProperty.tmpCritical.ToString());

                parasItemsList0[30].SetInputTxt(chaStats.chaProperty.criticalDamageRatios.ToString());

                parasItemsList0[31].SetInputTxt(chaStats.chaProperty.damageRatios.ToString());

                parasItemsList0[32].SetInputTxt(chaStats.chaProperty.damageAdd.ToString());

                parasItemsList0[33].SetInputTxt(chaStats.chaProperty.reduceHurtRatios.ToString());

                parasItemsList0[34].SetInputTxt(chaStats.chaProperty.reduceHurtAdd.ToString());

                parasItemsList0[35].SetInputTxt(chaStats.chaProperty.reduceBulletRatios.ToString());

                parasItemsList0[36].SetInputTxt(chaStats.chaProperty.changedFromPlayerDamage.ToString());

                parasItemsList0[37].SetInputTxt(chaStats.chaProperty.maxMoveSpeed.ToString());

                parasItemsList0[38].SetInputTxt(chaStats.chaProperty.defaultMaxMoveSpeed.ToString());

                parasItemsList0[39].SetInputTxt(chaStats.chaProperty.maxMoveSpeedRatios.ToString());

                parasItemsList0[40].SetInputTxt(chaStats.chaProperty.maxMoveSpeedAdd.ToString());

                parasItemsList0[41].SetInputTxt(chaStats.chaProperty.speedRecoveryTime.ToString());

                parasItemsList0[42].SetInputTxt(chaStats.chaProperty.mass.ToString());

                parasItemsList0[43].SetInputTxt(chaStats.chaProperty.defaultMass.ToString());

                parasItemsList0[44].SetInputTxt(chaStats.chaProperty.massRatios.ToString());

                parasItemsList0[45].SetInputTxt(chaStats.chaProperty.pushForce.ToString());

                parasItemsList0[46].SetInputTxt(chaStats.chaProperty.defaultPushForce.ToString());

                parasItemsList0[47].SetInputTxt(chaStats.chaProperty.pushForceRatios.ToString());

                parasItemsList0[48].SetInputTxt(chaStats.chaProperty.pushForceAdd.ToString());

                parasItemsList0[49].SetInputTxt(chaStats.chaProperty.reduceHitBackRatios.ToString());

                parasItemsList0[50].SetInputTxt(chaStats.chaProperty.dodge.ToString());

                parasItemsList0[51].SetInputTxt(chaStats.chaProperty.shieldCount.ToString());

                parasItemsList0[52].SetInputTxt(chaStats.chaProperty.defaultcoolDown.ToString());


                parasItemsList0[53].SetInputTxt(playerData.playerData.skillFreeBuyCount.ToString());


                parasItemsList0[54].SetInputTxt(playerData.playerData.buySkillRatios.ToString());


                parasItemsList0[55].SetInputTxt(playerData.playerData.refreshShopRatios.ToString());


                parasItemsList0[56].SetInputTxt(playerData.playerData.skillRefreshCount.ToString());


                parasItemsList0[57].SetInputTxt(playerData.playerData.skillWeightIncrease1.ToString());


                parasItemsList0[58].SetInputTxt(playerData.playerData.skillWeightIncrease2.ToString());


                parasItemsList0[59].SetInputTxt(playerData.playerData.skillWeightIncrease3.ToString());


                parasItemsList0[60].SetInputTxt(playerData.playerData.skillTempRefreshCount.ToString());

                parasItemsList0[61].SetInputTxt(chaStats.chaProperty.defaultBulletRangeRatios.ToString());

                parasItemsList0[62].SetInputTxt(chaStats.chaProperty.collideDamageRatios.ToString());

                parasItemsList0[63].SetInputTxt(chaStats.chaProperty.continuousCollideDamageRatios.ToString());


                parasItemsList0[64].SetInputTxt(playerData.playerData.superPushForceChance.ToString());


                parasItemsList0[65].SetInputTxt(playerData.playerData.maxPushForceChance.ToString());


                parasItemsList0[66].SetInputTxt(playerData.playerData.normalMonsterDamageRatios.ToString());


                parasItemsList0[67].SetInputTxt(playerData.playerData.specialMonsterDamageRatios.ToString());


                parasItemsList0[68].SetInputTxt(playerData.playerData.bossMonsterDamageRatios.ToString());


                parasItemsList0[69].SetInputTxt(playerData.playerData.weaponSkillExtraCount.ToString());

                parasItemsList0[70].SetInputTxt(chaStats.chaProperty.scaleRatios.ToString());
            }
            else if (btnIndex == 1)
            {
                var wbe = wbeQuery.ToEntityArray(Allocator.Temp)[0];
                var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
                //var board = boardQuery.ToEntityArray(Allocator.Temp)[0];


                var gameOthersData = entityManager.GetComponentData<GameOthersData>(wbe);
                var gameTimeData = entityManager.GetComponentData<GameTimeData>(wbe);
                var env = entityManager.GetComponentData<GameEnviromentData>(wbe);
                var set = entityManager.GetComponentData<GameSetUpData>(wbe);
                //var collider = entityManager.GetComponentData<PhysicsCollider>(player);
                //var boardcollider = entityManager.GetComponentData<PhysicsCollider>(board);


                // var global = Common.Instance.Get<Global>();
                // var cameraUpdateMono = global.MainCamera?.gameObject?.GetComponent<CameraUpdateMono>();

                parasItemsList1[0].SetInputTxt(gameTimeData.refreshTime.elapsedTime.ToString());
                parasItemsList1[1].SetInputTxt(0.ToString());
                parasItemsList1[2].SetInputTxt(env.env.weather.ToString());
                parasItemsList1[3].SetInputTxt(env.env.time.ToString());

                parasItemsList1[4].SetInputTxt(0.ToString());
                parasItemsList1[5].SetInputTxt(0.ToString());
                var dam = set.enableDamageNumber ? 1 : 0;

                parasItemsList1[6].SetInputTxt(dam.ToString());

                parasItemsList1[7].SetInputTxt(0.ToString());

                var getHitDuration = gameOthersData.gameOtherParas.getHitDuration;
                var getHitOffset = gameOthersData.gameOtherParas.getHitOffset;
                parasItemsList1[8].SetInputTxt(getHitDuration.ToString());
                parasItemsList1[9].SetInputTxt(getHitOffset.ToString());
                parasItemsList1[10].SetInputTxt(gameOthersData.gameOtherParas.dissolveSpeed.ToString());
                parasItemsList1[11].SetInputTxt(gameOthersData.gameOtherParas.alphaSpeed.ToString());
                parasItemsList1[12].SetInputTxt(gameOthersData.gameOtherParas.dropAnimedDuration.ToString());
                parasItemsList1[13].SetInputTxt(gameOthersData.gameOtherParas.dropAnimedHeight.ToString());

                // parasItemsList1[7].SetInputTxt(0.ToString());
                //
                // parasItemsList1[8].SetInputTxt(0.ToString());
                // parasItemsList1[9].SetInputTxt(0.ToString());
                //
                // parasItemsList1[10].SetInputTxt(0.ToString());
                // parasItemsList1[11].SetInputTxt(0.ToString());
                // parasItemsList1[5].SetInputTxt(collider.Value.Value.GetRestitution().ToString());
                // parasItemsList1[6].SetInputTxt(boardcollider.Value.Value.GetRestitution().ToString());
            }
            else if (btnIndex == 2)
            {
                //黄金国修改
                string materialPath =
                    "Assets/JuiceZombies/Art_Resources/UI_Effects/BattleNumber/DamageNumber/NumberMaterial01.mat";
                //Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                Material material = default;
                if (material == null)
                {
                    Debug.Log("material is null");
                }

                parasItemsList2[0].SetInputTxt(material.GetFloat("_InitialSize").ToString());
                parasItemsList2[1].SetInputTxt(material.GetFloat("_MaxSize").ToString());
                parasItemsList2[2].SetInputTxt(material.GetFloat("_DisappearSize").ToString());
                parasItemsList2[3].SetInputTxt(material.GetFloat("_ToMaxTime").ToString());
                parasItemsList2[4].SetInputTxt(material.GetFloat("_HoldMaxTime").ToString());
                parasItemsList2[5].SetInputTxt(material.GetFloat("_ToDisappearTime").ToString());
                parasItemsList2[6].SetInputTxt(material.GetFloat("_AppearAlpha").ToString());
                parasItemsList2[7].SetInputTxt(material.GetFloat("_MaxAlpha").ToString());
                parasItemsList2[8].SetInputTxt(material.GetFloat("_DisappearAlpha").ToString());
                parasItemsList2[9].SetInputTxt(material.GetFloat("_Radius").ToString());
                parasItemsList2[10].SetInputTxt(material.GetFloat("_XScale").ToString());
                parasItemsList2[11].SetInputTxt(material.GetFloat("_YScale").ToString());
                parasItemsList2[12].SetInputTxt(material.GetFloat("_CharacterProportion").ToString());
            }
            else if (btnIndex == 3)
            {
            }
            else if (btnIndex == 4)
            {
            }
            else if (btnIndex == 5)
            {
            }
        }


        void InitItem(int btnIndex)
        {
            if (this.btnIndex == btnIndex)
            {
                return;
            }

            //SetPlayerProperty(0);
            //InitParasData(btnIndex);
            ClearParasList();
            var scroller_MailList = GetFromReference(KScroller_MailList);
            var scrollRect = scroller_MailList.GetScrollRect();
            var list = scrollRect.Content.GetList();
            list.Clear();
            if (btnIndex == 0)
            {
                var language = ConfigManager.Instance.Tables.Tblanguage;
                var attr_variable = ConfigManager.Instance.Tables.Tbattr_variable;


                this.btnIndex = 0;
                attr_varibles.DataList.Sort((a, b) => { return a.id.CompareTo(b.id); });

                for (int i = 0; i < attr_varibles.DataList.Count; i++)
                {
                    if (attr_varibles.DataList[i].id / 100000 == 2)
                    {
                        //Log.Error($"{attr_varibles.DataList[i].id}");
                        var ui = list.CreateWithUIType(UIType.UISubPanel_ParasItem,
                            attr_varibles.DataList[i].id, false) as UISubPanel_ParasItem;

                        // var ui = UIHelper.Create(UIType.UISubPanel_ParasItem, attr_varibles.DataList[i].id,
                        //     scrollRect.Content.GameObject.transform) as UISubPanel_ParasItem;
                        var txt_ItemName = ui.GetFromReference(UISubPanel_ParasItem.KTxt_ItemName);
                        txt_ItemName.GetTextMeshPro()
                            .SetTMPText(language.Get(attr_variable.DataList[i].name).current);
                        parasItemsList0.Add(ui);
                    }
                }
            }
            else if (btnIndex == 1)
            {
                this.btnIndex = 1;
                for (int i = 0; i < 14; i++)
                {
                    var ui = list.CreateWithUIType(UIType.UISubPanel_ParasItem,
                        i, false) as UISubPanel_ParasItem;
                    parasItemsList1.Add(ui);

                    //0:游戏时间  1:摄像机旋转角度 2:是否立即通关:1
                    string text = default;
                    switch (i)
                    {
                        case 0:
                            text = "游戏时间";
                            break;
                        case 1:
                            text = "是否立即通关:1";
                            break;
                        case 2:
                            text = "当前环境天气:";
                            break;
                        case 3:
                            text = "当前环境时间:";
                            break;
                        case 4:
                            text = "添加全局事件id(给全局单例加):";
                            break;
                        case 5:
                            text = "添加角色技能id(给玩家加):";
                            break;
                        case 6:
                            text = "是否开启伤害数字(0/1):";
                            break;
                        case 7:
                            text = "是否清除玩家所有技能:";
                            break;
                        case 8:
                            text = "受击持续时间:";
                            break;
                        case 9:
                            text = "受击形变幅度:";
                            break;
                        case 10:
                            text = "消融速度:";
                            break;
                        case 11:
                            text = "变透明速度:";
                            break;
                        case 12:
                            text = "掉落动画持续时间:";
                            break;
                        case 13:
                            text = "掉落动画高度";
                            break;
                    }

                    ui.GetFromReference(UISubPanel_ParasItem.KTxt_ItemName).GetTextMeshPro().SetTMPText(text);
                }
            }
            else if (btnIndex == 2)
            {
                this.btnIndex = 2;
                //黄金国
                for (int i = 101; i < 114; i++)
                {
                    var ui = list.CreateWithUIType(UIType.UISubPanel_ParasItem,
                        i, false) as UISubPanel_ParasItem;
                    parasItemsList2.Add(ui);
                }
            }
            else if (btnIndex == 3)
            {
                this.btnIndex = 3;
            }
            else if (btnIndex == 4)
            {
                this.btnIndex = 4;
            }
            else if (btnIndex == 5)
            {
                this.btnIndex = 5;
            }
        }

        float FloatTryPrase(UISubPanel_ParasItem ui)
        {
            if (!float.TryParse(ui.GetInputTxt(), out float result))
            {
                Log.Error(
                    $" 此数据类型有误,请检查!");
                return default;
            }

            return result;
        }

        public void SetPlayerProperty(int index)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var playerQuery = entityManager.CreateEntityQuery(typeof(PlayerData));
            var wbeQuery = entityManager.CreateEntityQuery(typeof(WorldBlackBoardTag));
            var player = playerQuery.ToEntityArray(Allocator.Temp)[0];
            var playerData = entityManager.GetComponentData<PlayerData>(player);
            var chaStats = entityManager.GetComponentData<ChaStats>(player);
            //var physicsMass = entityManager.GetComponentData<PhysicsMass>(player);

            switch (index)
            {
                case 0:

                    switch (btnIndex)
                    {
                        case 0:
                            attr_varibles.DataList.Sort((a, b) => { return a.id.CompareTo(b.id); });

                            int index1 = attr_varibles.DataList.FindIndex(item => item.id / 100000 == 2);
                            for (int i = 0; i < parasItemsList0.Count; i++)
                            {
                                var txt = parasItemsList0[i].GetInputTxt();

                                if (float.TryParse(txt, out float result))
                                {
                                    UnityHelper.InitProperty(ref playerData, ref chaStats,
                                        attr_varibles.DataList[index1 + i].id,
                                        result);
                                }
                                else
                                {
                                    Log.Error(
                                        $"{language.Get(attr_varibles.DataList[i].name).current}:{txt} 此数据类型有误,请检查!");
                                }
                            }


                            //chaStats.chaProperty = playerData.chaProperty;
                            //playerData.playerData = playerData.playerData;
                            chaStats.chaResource.hp = chaStats.chaProperty.maxHp;

                            //var mass = chaStats.chaProperty.mass;

                            //physicsMass.InverseMass = 1f / mass;
                            entityManager.SetComponentData(player, chaStats);
                            entityManager.SetComponentData(player, playerData);
                            //entityManager.SetComponentData(player, physicsMass);


                            break;
                        case 1:


                            //0:游戏时间  1:摄像机旋转角度
                            var wbe = wbeQuery.ToEntityArray(Allocator.Temp)[0];
                            var gameOthersData = entityManager.GetComponentData<GameOthersData>(wbe);
                            var gameTimeData = entityManager.GetComponentData<GameTimeData>(wbe);
                            var setUpData = entityManager.GetComponentData<GameSetUpData>(wbe);
                            var time = FloatTryPrase(parasItemsList1[0]);
                            gameTimeData.refreshTime.tick = (int)(time / 0.02f);
                            gameTimeData.refreshTime.elapsedTime = time;
                            setUpData.enableDamageNumber = FloatTryPrase(parasItemsList1[6]) > 0;
                            entityManager.SetComponentData(wbe, gameTimeData);
                            entityManager.SetComponentData(wbe, gameTimeData);
                            entityManager.SetComponentData(wbe, setUpData);
                            var isSucceed = FloatTryPrase(parasItemsList1[1]);
                            if (isSucceed > 0)
                            {
                                JiYuUIHelper.QuickSucceed();
                            }

                            var envWeather = FloatTryPrase(parasItemsList1[2]);
                            var envTime = FloatTryPrase(parasItemsList1[3]);

                            entityManager.SetComponentData(wbe, new GameEnviromentData
                            {
                                env = new Enviroment
                                {
                                    weather = (int)envWeather,
                                    time = (int)envTime
                                }
                            });

                            //TODO:
                            var eventId = FloatTryPrase(parasItemsList1[4]);

                            //var skillId = FloatTryPrase(parasItemsList1[5]);
                            var skillIdList = parasItemsList1[5].GetInputTxt().Split(";");


                            var events = entityManager.GetBuffer<GameEvent>(wbe);
                            if ((int)eventId > 0)
                            {
                                events.Add(new GameEvent
                                {
                                    Int32_0 = (int)eventId,
                                    Boolean_6 = true
                                });
                            }


                            var skills = entityManager.GetBuffer<Skill>(player);


                            foreach (var skillId in skillIdList)
                            {
                                if (int.TryParse(skillId, out var skillId0))
                                {
                                    if (skillId0 > 0)
                                    {
                                        skills.Add(new Skill
                                        {
                                            CurrentTypeId = (Skill.TypeId)1,
                                            Int32_0 = skillId0,
                                            Entity_5 = player,
                                        });
                                    }
                                }
                            }


                            var web6 = FloatTryPrase(parasItemsList1[6]);
                            var web7 = FloatTryPrase(parasItemsList1[7]);
                            if (web7 > 0)
                            {
                                var triggers = entityManager.GetBuffer<Trigger>(player);
                                triggers.Clear();
                                var buffs = entityManager.GetBuffer<Buff>(player);
                                for (int i = 0; i < buffs.Length; i++)
                                {
                                    var buff = buffs[i];
                                    buff.Boolean_4 = false;
                                    buff.Single_3 = 0;
                                    buffs[i] = buff;
                                }

                                //buffs.Clear();
                                var skills1 = entityManager.GetBuffer<Skill>(player);
                                skills1.Clear();
                                var events1 = entityManager.GetBuffer<GameEvent>(player);
                                events1.Clear();
                            }

                            var web8 = FloatTryPrase(parasItemsList1[8]);
                            var web9 = FloatTryPrase(parasItemsList1[9]);
                            
                            var web10 = FloatTryPrase(parasItemsList1[10]);
                            var web11 = FloatTryPrase(parasItemsList1[11]);
                            var web12 = FloatTryPrase(parasItemsList1[12]);
                            var web13 = FloatTryPrase(parasItemsList1[13]);
                            gameOthersData.gameOtherParas.getHitDuration = web8;
                            gameOthersData.gameOtherParas.getHitOffset = web9;
                            gameOthersData.gameOtherParas.dissolveSpeed = web10;
                            gameOthersData.gameOtherParas.alphaSpeed = web11;
                            gameOthersData.gameOtherParas.dropAnimedDuration = web12;
                            gameOthersData.gameOtherParas.dropAnimedHeight = web13;
                            
                            entityManager.SetComponentData(wbe, gameOthersData);

                            // var web7 = FloatTryPrase(parasItemsList1[7]);
                            // var web8 = FloatTryPrase(parasItemsList1[8]);
                            // var web9 = FloatTryPrase(parasItemsList1[9]);
                            // var web10 = FloatTryPrase(parasItemsList1[10]);
                            // var web11 = FloatTryPrase(parasItemsList1[11]);


                            // var enableEnemyWeapon = FloatTryPrase(parasItemsList1[3]);
                            // if (enableEnemyWeapon == 0)
                            // {
                            //     gameOthersData.enableEnemyWeapon = false;
                            // }
                            // else
                            // {
                            //     gameOthersData.enableEnemyWeapon = true;
                            // }
                            //
                            // entityManager.SetComponentData(wbe, gameOthersData);
                            break;
                        case 2:
                            //黄金国
                            string materialPath =
                                "Assets/JuiceZombies/Art_Resources/UI_Effects/BattleNumber/DamageNumber/NumberMaterial01.mat";
                            //Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                            Material material = default;
                            if (material == null)
                            {
                                Debug.Log("material is null");
                            }

                            material.SetFloat("_InitialSize", float.Parse(parasItemsList2[0].GetInputTxt()));
                            material.SetFloat("_MaxSize", float.Parse(parasItemsList2[1].GetInputTxt()));
                            material.SetFloat("_DisappearSize", float.Parse(parasItemsList2[2].GetInputTxt()));
                            material.SetFloat("_ToMaxTime", float.Parse(parasItemsList2[3].GetInputTxt()));
                            material.SetFloat("_HoldMaxTime", float.Parse(parasItemsList2[4].GetInputTxt()));
                            material.SetFloat("_ToDisappearTime", float.Parse(parasItemsList2[5].GetInputTxt()));
                            material.SetFloat("_AppearAlpha", float.Parse(parasItemsList2[6].GetInputTxt()));
                            material.SetFloat("_MaxAlpha", float.Parse(parasItemsList2[7].GetInputTxt()));
                            material.SetFloat("_DisappearAlpha", float.Parse(parasItemsList2[8].GetInputTxt()));
                            material.SetFloat("_Radius", float.Parse(parasItemsList2[9].GetInputTxt()));
                            material.SetFloat("_XScale", float.Parse(parasItemsList2[10].GetInputTxt()));
                            material.SetFloat("_YScale", float.Parse(parasItemsList2[11].GetInputTxt()));
                            material.SetFloat("_CharacterProportion", float.Parse(parasItemsList2[12].GetInputTxt()));
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                    }


                    break;
                case 1:
                    switch (btnIndex)
                    {
                        case 0:

                            // foreach (var paras in attr_varibles.DataList)
                            // {
                            //     UnityHelper.InitPlayerProperty(ref playerData, paras.id, paras.base0);
                            // }
                            //
                            // chaStats.chaProperty = chaProperty;
                            // entityManager.SetComponentData(player, chaStats);
                            // entityManager.SetComponentData(player, playerData);
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                    }


                    break;
                case 2:
                    switch (btnIndex)
                    {
                        case 0:
                            //TODO:
                            //chaProperty = ResourcesSingleton.Instance.playerProperty.chaProperty;
                            playerData = ResourcesSingleton.Instance.playerProperty.playerData;
                            chaStats.chaProperty = ResourcesSingleton.Instance.playerProperty.chaProperty;
                            entityManager.SetComponentData(player, chaStats);
                            entityManager.SetComponentData(player, playerData);
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                    }

                    break;
            }
        }

        void ClearParasList()
        {
            parasItemsList0.Clear();
            parasItemsList1.Clear();
            parasItemsList2.Clear();
        }


        protected override void OnClose()
        {
            ClearParasList();
            base.OnClose();
        }
    }
}